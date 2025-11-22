using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace MagicOutput
{
    // Classification type definitions
    internal static class OutputClassificationTypes
    {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("output.error")]
        internal static ClassificationTypeDefinition ErrorType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("output.warning")]
        internal static ClassificationTypeDefinition WarningType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("output.success")]
        internal static ClassificationTypeDefinition SuccessType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("output.info")]
        internal static ClassificationTypeDefinition InfoType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("output.debug")]
        internal static ClassificationTypeDefinition DebugType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("output.trace")]
        internal static ClassificationTypeDefinition TraceType = null;
    }

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
            ForegroundColor = Color.FromRgb(255, 200, 100); // Orange/Yellow
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
            ForegroundColor = Color.FromRgb(100, 255, 100); // Light green
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
            ForegroundColor = Color.FromRgb(100, 180, 255); // Light blue
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
            ForegroundColor = Color.FromRgb(180, 180, 180); // Gray
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
            ForegroundColor = Color.FromRgb(200, 150, 255); // Purple
        }
    }

    // Classifier
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

        // Regex patterns for matching
        private static readonly Regex ErrorPattern = new Regex(
            @"(error|exception|(?<!0\s)failed|failure|fatal|critical|\bfail\b)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex WarningPattern = new Regex(
            @"(warning|warn|caution|deprecated)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

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
            _errorType = registry.GetClassificationType("output.error");
            _warningType = registry.GetClassificationType("output.warning");
            _successType = registry.GetClassificationType("output.success");
            _infoType = registry.GetClassificationType("output.info");
            _debugType = registry.GetClassificationType("output.debug");
            _traceType = registry.GetClassificationType("output.trace");
        }

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        public System.Collections.Generic.IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var classifications = new System.Collections.Generic.List<ClassificationSpan>();
            string text = span.GetText();

            // Priority order: Error > Warning > Success > Debug > Trace > Info
            if (ErrorPattern.IsMatch(text))
            {
                classifications.Add(new ClassificationSpan(span, _errorType));
            }
            else if (WarningPattern.IsMatch(text))
            {
                classifications.Add(new ClassificationSpan(span, _warningType));
            }
            else if (SuccessPattern.IsMatch(text))
            {
                classifications.Add(new ClassificationSpan(span, _successType));
            }
            else if (DebugPattern.IsMatch(text))
            {
                classifications.Add(new ClassificationSpan(span, _debugType));
            }
            else if (TracePattern.IsMatch(text))
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