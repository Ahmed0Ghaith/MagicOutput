using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace MagicOutput
{
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
}