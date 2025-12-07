using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel.Design;

namespace MagicOutput
{
    // Register the package so Tools -> Options will show the page and add menu resources
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("Magic Output", "Colorizes Visual studio Output window", "1.0.0")]
    [ProvideOptionPage(typeof(OutputClassificationOptions), "Magic Output", "General", 0, 0, true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidMagicOutputPkgString)]
    public sealed class MagicOutputPackage : AsyncPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // Ensure we're on UI thread for command registration
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            
            // Call base initialization to ensure package is properly registered
            await base.InitializeAsync(cancellationToken, progress);

            OleMenuCommandService mcs = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (mcs != null)
            {
                var menuCommandID = new CommandID(GuidList.guidMagicOutputCmdSet, PkgCmdID.cmdidOpenOptions);
                var menuItem = new OleMenuCommand(OnOpenOptions, menuCommandID);
                mcs.AddCommand(menuItem);
            }
        }

        private void OnOpenOptions(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            // Opens the registered options page directly
            this.ShowOptionPage(typeof(OutputClassificationOptions));
        }


        // Define content type for the Output classifier (ensures classification types are registered)
        [Export(typeof(ContentTypeDefinition))]
        [Name("output")]
        [BaseDefinition("text")]
        internal static ContentTypeDefinition OutputContentType = null;

        // Export classification type definitions so registry.GetClassificationType(...) returns non-null
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("output.error")]
        internal static ClassificationTypeDefinition OutputErrorType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("output.warning")]
        internal static ClassificationTypeDefinition OutputWarningType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("output.success")]
        internal static ClassificationTypeDefinition OutputSuccessType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("output.info")]
        internal static ClassificationTypeDefinition OutputInfoType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("output.debug")]
        internal static ClassificationTypeDefinition OutputDebugType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("output.trace")]
        internal static ClassificationTypeDefinition OutputTraceType = null;

        // Classification formats (colors)
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "output.error")]
        [Name("output.error")]
        [UserVisible(true)]
        [Order(Before = Priority.Default)]
        internal sealed class ErrorFormat : ClassificationFormatDefinition
        {
            public ErrorFormat()
            {
                DisplayName = "Output Error";
                ForegroundColor = Color.FromRgb(255, 100, 100); // Light red
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "output.warning")]
        [Name("output.warning")]
        [UserVisible(true)]
        [Order(Before = Priority.Default)]
        internal sealed class WarningFormat : ClassificationFormatDefinition
        {
            public WarningFormat()
            {
                DisplayName = "Output Warning";
                ForegroundColor = Colors.Yellow;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "output.success")]
        [Name("output.success")]
        [UserVisible(true)]
        [Order(Before = Priority.Default)]
        internal sealed class SuccessFormat : ClassificationFormatDefinition
        {
            public SuccessFormat()
            {
                DisplayName = "Output Success";
                ForegroundColor = Colors.LightGreen;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "output.info")]
        [Name("output.info")]
        [UserVisible(true)]
        [Order(Before = Priority.Default)]
        internal sealed class InfoFormat : ClassificationFormatDefinition
        {
            public InfoFormat()
            {
                DisplayName = "Output Info";
                ForegroundColor = Colors.Brown;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "output.debug")]
        [Name("output.debug")]
        [UserVisible(true)]
        [Order(Before = Priority.Default)]
        internal sealed class DebugFormat : ClassificationFormatDefinition
        {
            public DebugFormat()
            {
                DisplayName = "Output Debug";
                ForegroundColor = Colors.Gray;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "output.trace")]
        [Name("output.trace")]
        [UserVisible(true)]
        [Order(Before = Priority.Default)]
        internal sealed class TraceFormat : ClassificationFormatDefinition
        {
            public TraceFormat()
            {
                DisplayName = "Output Trace";
                ForegroundColor = Colors.Purple;
            }
        }

        // Classifier provider
        [Export(typeof(IClassifierProvider))]
        [ContentType("output")]
        internal class OutputClassifierProvider : IClassifierProvider
        {
            [Import]
            internal IClassificationTypeRegistryService ClassificationRegistry = null;

            public IClassifier GetClassifier(ITextBuffer buffer)
            {
                return buffer.Properties.GetOrCreateSingletonProperty(() =>
                    new OutputClassifier(ClassificationRegistry));
            }
        }

        internal class OutputClassifier : IClassifier
        {
            private readonly IClassificationType _errorType;
            private readonly IClassificationType _warningType;
            private readonly IClassificationType _successType;
            private readonly IClassificationType _infoType;
            private readonly IClassificationType _debugType;
            private readonly IClassificationType _traceType;

            private Regex _dynamicWarningPattern;
            private Regex _dynamicErrorPattern;

            private readonly OutputClassificationOptions _options;

            private static readonly Regex SuccessPattern = new Regex(
                @"(success|succeeded|completed|passed|done|\bok\b|build succeeded)",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            private static readonly Regex InfoPattern = new Regex(
                @"(info|information|note|starting|building)",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            private static readonly Regex DebugPattern = new Regex(
                @"(debug|verbose)",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            private static readonly Regex TracePattern = new Regex(
                @"(trace|tracing)",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            public OutputClassifier(IClassificationTypeRegistryService registry)
            {
                // Try to obtain the options page; fall back to defaults if unavailable.
                OutputClassificationOptions options = null;
                try
                {
                    options = Package.GetGlobalService(typeof(OutputClassificationOptions)) as OutputClassificationOptions;
                }
                catch
                {
                    options = null;
                }
                _options = options ?? new OutputClassificationOptions();

                // Registry should be present because we exported the ClassificationTypeDefinitions above.
                _errorType = registry?.GetClassificationType("output.error");
                _warningType = registry?.GetClassificationType("output.warning");
                _successType = registry?.GetClassificationType("output.success");
                _infoType = registry?.GetClassificationType("output.info");
                _debugType = registry?.GetClassificationType("output.debug");
                _traceType = registry?.GetClassificationType("output.trace");

                BuildDynamicRegexPatterns();
            }

            private void BuildDynamicRegexPatterns()
            {
                string extraWarnings = _options?.ExtraWarningPatterns ?? "";
                string extraErrors = _options?.ExtraErrorPatterns ?? "";

                string warningRegex = @"(warning|warn|caution|deprecated";
                string errorRegex = @"(error|exception|failed|failure|fatal|critical";

                if (!string.IsNullOrWhiteSpace(extraWarnings))
                    warningRegex += "|" + Regex.Escape(extraWarnings).Replace("\\,", "|").Replace(" ", "");

                if (!string.IsNullOrWhiteSpace(extraErrors))
                    errorRegex += "|" + Regex.Escape(extraErrors).Replace("\\,", "|").Replace(" ", "");

                warningRegex += ")";
                errorRegex += ")";

                // Use compiled patterns for performance and be resilient if patterns are invalid
                try
                {
                    _dynamicWarningPattern = new Regex(warningRegex, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                }
                catch
                {
                    _dynamicWarningPattern = new Regex(@"(warning|warn|caution|deprecated)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                }

                try
                {
                    _dynamicErrorPattern = new Regex(errorRegex, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                }
                catch
                {
                    _dynamicErrorPattern = new Regex(@"(error|exception|failed|failure|fatal|critical)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                }
            }

            public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

            public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
            {
                var classifications = new List<ClassificationSpan>();
                string text = span.GetText();

                // Priority order: Error > Warning > Success > Debug > Trace > Info
                if (_dynamicErrorPattern != null && _dynamicErrorPattern.IsMatch(text) && _errorType != null)
                {
                    classifications.Add(new ClassificationSpan(span, _errorType));
                }
                else if (_dynamicWarningPattern != null && _dynamicWarningPattern.IsMatch(text) && _warningType != null)
                {
                    classifications.Add(new ClassificationSpan(span, _warningType));
                }
                else if (SuccessPattern.IsMatch(text) && _successType != null)
                {
                    classifications.Add(new ClassificationSpan(span, _successType));
                }
                else if (DebugPattern.IsMatch(text) && _debugType != null)
                {
                    classifications.Add(new ClassificationSpan(span, _debugType));
                }
                else if (TracePattern.IsMatch(text) && _traceType != null)
                {
                    classifications.Add(new ClassificationSpan(span, _traceType));
                }
                else if (InfoPattern.IsMatch(text))
                {
                    classifications.Add(new ClassificationSpan(span, _infoType));
                }

                return classifications;
            }
        }
    }
}