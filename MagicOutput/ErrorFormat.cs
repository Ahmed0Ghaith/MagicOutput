using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

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
            ForegroundColor = GetColorFromSettings("ErrorColor", Color.FromRgb(255, 100, 100));
        }

        private Color GetColorFromSettings(string property, Color defaultColor)
        {
            try
            {
                var options = MagicOutputPackage.Options;
                if (options != null)
                {
                    var colorHex = options.GetType().GetProperty(property)?.GetValue(options) as string;
                    if (!string.IsNullOrEmpty(colorHex))
                        return (Color)ColorConverter.ConvertFromString(colorHex);
                }
            }
            catch { }
            return defaultColor;
        }
    }

}