using System;

namespace MagicOutput
{
    internal static class GuidList
    {
        // Package GUID (matches the Guid attribute on the AsyncPackage)
        public const string guidMagicOutputPkgString = "d6b7f4b6-4d9e-4e6f-9c8a-3f6b6b2a1a7e";

        // Command set GUID (for menu commands)
        public const string guidMagicOutputCmdSetString = "b3c9b7d9-7f3a-4e5d-9c6b-8a7b9c0d1e2f";

        public static readonly Guid guidMagicOutputCmdSet = new Guid(guidMagicOutputCmdSetString);
    }
}
