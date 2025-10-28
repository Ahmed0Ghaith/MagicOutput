﻿using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace MagicOutput
{
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
            ForegroundColor = Color.FromRgb(200, 150, 255);
        }
    }

}