using Microsoft.VisualStudio.Setup.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace VS2022.TaskProblem.Exp6.Modify
{
    public static class VS2022
    {
        public static bool TryGetLocation([NotNullWhen(true)] out DirectoryInfo? directory)
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
                        if (Path.GetDirectoryName(setupInstance.GetProductPath()) is string directoryPath)
                        {
                            directory = new DirectoryInfo(directoryPath);
                            return true;
                        }
                    }
                }
            }

            Errors.Add("VS2022 not found");
            directory = null;

            return false;
        }
    }
}
