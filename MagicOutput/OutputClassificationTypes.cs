using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

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

}