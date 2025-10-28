using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace MagicOutput
{
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
            ForegroundColor = Color.FromRgb(255, 200, 100);
        }
    }

}