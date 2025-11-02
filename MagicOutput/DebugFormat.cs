using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace MagicOutput
{
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
}