using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using System;
using System.Runtime.InteropServices;

namespace MagicOutput
{

    // Package Registration
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideOptionPage(typeof(ColorOptions), "Magic Output", "Colors", 0, 0, true)]
    [Guid("12345678-1234-1234-1234-123456789ABC")]
    public sealed class MagicOutputPackage : AsyncPackage
    {
				public MagicOutputPackage()
				{
				}

				public static ColorOptions Options { get; private set; }

        protected override async System.Threading.Tasks.Task InitializeAsync(System.Threading.CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            Options = (ColorOptions)GetDialogPage(typeof(ColorOptions));
        }
    }

}