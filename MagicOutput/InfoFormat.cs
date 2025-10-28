using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace MagicOutput
{
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
            ForegroundColor = Color.FromRgb(100, 180, 255);
        }
    }

}