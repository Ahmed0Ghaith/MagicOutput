using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using System;
using System.Text.RegularExpressions;

namespace MagicOutput
{
		internal class OutputClassifier : IClassifier
    {
        private readonly IClassificationType _errorType;
        private readonly IClassificationType _warningType;
        private readonly IClassificationType _successType;
        private readonly IClassificationType _infoType;
        private readonly IClassificationType _debugType;
        private readonly IClassificationType _traceType;

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

            var options = MagicOutputPackage.Options;
            if (options == null || !options.Enabled)
                return classifications;

            string text = span.GetText();
            var regexOptions = options.CaseSensitive ? RegexOptions.Compiled : RegexOptions.IgnoreCase | RegexOptions.Compiled;

            // Priority order: Error > Warning > Success > Debug > Trace > Info
            if (MatchesKeywords(text, options.ErrorKeywords, regexOptions))
            {
                classifications.Add(new ClassificationSpan(span, _errorType));
            }
            else if (MatchesKeywords(text, options.WarningKeywords, regexOptions))
            {
                classifications.Add(new ClassificationSpan(span, _warningType));
            }
            else if (MatchesKeywords(text, options.SuccessKeywords, regexOptions))
            {
                classifications.Add(new ClassificationSpan(span, _successType));
            }
            else if (MatchesKeywords(text, options.DebugKeywords, regexOptions))
            {
                classifications.Add(new ClassificationSpan(span, _debugType));
            }
            else if (MatchesKeywords(text, options.TraceKeywords, regexOptions))
            {
                classifications.Add(new ClassificationSpan(span, _traceType));
            }
            else if (MatchesKeywords(text, options.InfoKeywords, regexOptions))
            {
                classifications.Add(new ClassificationSpan(span, _infoType));
            }

            return classifications;
        }

        private bool MatchesKeywords(string text, string keywords, RegexOptions options)
        {
            if (string.IsNullOrWhiteSpace(keywords))
                return false;

            var keywordList = keywords.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var keyword in keywordList)
            {
                var pattern = $@"\b{Regex.Escape(keyword.Trim())}\b";
                if (Regex.IsMatch(text, pattern, options))
                    return true;
            }
            return false;
        }
    }

}