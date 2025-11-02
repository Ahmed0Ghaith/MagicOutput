using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace MagicOutput
{
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
}