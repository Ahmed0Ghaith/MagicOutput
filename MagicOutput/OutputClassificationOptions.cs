using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace MagicOutput
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class OutputClassificationOptions : DialogPage
    {
        [Category("Regex Settings")]
        [DisplayName("Extra Warning Patterns")]
        [Description("Comma separated regex terms added to the default warning pattern. Example: deprecated, obsolete")]
        public string ExtraWarningPatterns { get; set; } = "";

        [Category("Regex Settings")]
        [DisplayName("Extra Error Patterns")]
        [Description("Comma separated regex terms added to the default error pattern. Example: critical, severe")]
        public string ExtraErrorPatterns { get; set; } = "";

        protected override void OnApply(PageApplyEventArgs e)
        {
            base.OnApply(e);
        }
    }
}
