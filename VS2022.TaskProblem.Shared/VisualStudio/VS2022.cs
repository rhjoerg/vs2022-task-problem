using Microsoft.VisualStudio.Setup.Configuration;
using System.IO;

namespace VS2022.TaskProblem.Shared.VisualStudio
{
    public static class VS2022
    {
        public static string InstallDirectory
        {
            get
            {
                SetupConfiguration setupConfiguration = new();
                IEnumSetupInstances allInstances = setupConfiguration.EnumAllInstances();
                ISetupInstance[] rgelt = new ISetupInstance[1];

                while (true)
                {
                    allInstances.Next(1, rgelt, out int pceltFetched);
                    if (pceltFetched == 0) break;

                    if (rgelt[0] is ISetupInstance2 setupInstance)
                    {
                        if (setupInstance.GetInstallationName().StartsWith("VisualStudio/17."))
                        {
                            if (Path.GetDirectoryName(setupInstance.GetProductPath()) is string directory)
                                return directory;
                        }
                    }
                }

                throw new DirectoryNotFoundException("VS2022 not found.");
            }
        }

        public static string MSBuildAmd64Exe { get => Path.Combine(InstallDirectory, "Msbuild", "Current", "Bin", "amd64", "MSBuild.exe"); }
    }
}
