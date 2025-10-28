using Microsoft.VisualStudio.Shell;
using System.ComponentModel;

namespace MagicOutput
{
		// Options Page for Color Customization
		public class ColorOptions : DialogPage
    {
        [Category("Error Messages")]
        [DisplayName("Error Color")]
        [Description("Color for error messages (default: #FF6464 - Light Red)")]
        public string ErrorColor { get; set; } = "#FF6464";

        [Category("Error Messages")]
        [DisplayName("Error Keywords")]
        [Description("Keywords that trigger error coloring (comma-separated)")]
        public string ErrorKeywords { get; set; } = "error,exception,failed,failure,fatal,critical,fail";

        [Category("Warning Messages")]
        [DisplayName("Warning Color")]
        [Description("Color for warning messages (default: #FFC864 - Orange)")]
        public string WarningColor { get; set; } = "#FFC864";

        [Category("Warning Messages")]
        [DisplayName("Warning Keywords")]
        [Description("Keywords that trigger warning coloring (comma-separated)")]
        public string WarningKeywords { get; set; } = "warning,warn,caution,deprecated";

        [Category("Success Messages")]
        [DisplayName("Success Color")]
        [Description("Color for success messages (default: #64FF64 - Light Green)")]
        public string SuccessColor { get; set; } = "#64FF64";

        [Category("Success Messages")]
        [DisplayName("Success Keywords")]
        [Description("Keywords that trigger success coloring (comma-separated)")]
        public string SuccessKeywords { get; set; } = "success,succeeded,completed,passed,done,ok,build succeeded";

        [Category("Info Messages")]
        [DisplayName("Info Color")]
        [Description("Color for info messages (default: #64B4FF - Light Blue)")]
        public string InfoColor { get; set; } = "#64B4FF";

        [Category("Info Messages")]
        [DisplayName("Info Keywords")]
        [Description("Keywords that trigger info coloring (comma-separated)")]
        public string InfoKeywords { get; set; } = "info,information,note,starting,building";

        [Category("Debug Messages")]
        [DisplayName("Debug Color")]
        [Description("Color for debug messages (default: #B4B4B4 - Gray)")]
        public string DebugColor { get; set; } = "#B4B4B4";

        [Category("Debug Messages")]
        [DisplayName("Debug Keywords")]
        [Description("Keywords that trigger debug coloring (comma-separated)")]
        public string DebugKeywords { get; set; } = "debug,verbose";

        [Category("Trace Messages")]
        [DisplayName("Trace Color")]
        [Description("Color for trace messages (default: #C896FF - Purple)")]
        public string TraceColor { get; set; } = "#C896FF";

        [Category("Trace Messages")]
        [DisplayName("Trace Keywords")]
        [Description("Keywords that trigger trace coloring (comma-separated)")]
        public string TraceKeywords { get; set; } = "trace,tracing";

        [Category("General")]
        [DisplayName("Enable Colorization")]
        [Description("Enable or disable output colorization")]
        public bool Enabled { get; set; } = true;

        [Category("General")]
        [DisplayName("Case Sensitive")]
        [Description("Make keyword matching case-sensitive")]
        public bool CaseSensitive { get; set; } = false;
    }

}