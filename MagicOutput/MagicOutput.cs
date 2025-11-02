using System;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace MagicOutput
{

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
            @"(error|exception|failed|failure|fatal|critical|\bfail\b)",
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