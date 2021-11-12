using Microsoft.VisualStudio.Setup.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VS2022.TaskProblem.Exp6.Modify
{
    public class Program
    {
        public static int Main(string[] args)
        {
            SetupConfiguration setupConfiguration = new();
            string? path = args.Length > 0 ? args[0] : null;
            SetupInstances setupInstances = SetupInstances.CreateVisualStudio2022Instances(setupConfiguration, path);

            if (!ValidateInstanceCount(setupInstances))
                return Errors.Write();

            SetupInstance setupInstance = setupInstances.First();

            if (!ValidateMSBuildInstalled(setupInstance))
                return Errors.Write();

            return 0;
        }

        private static bool ValidateInstanceCount(IEnumerable<SetupInstance> setupInstances)
        {
            if (setupInstances.Count() == 1) return true;

            Errors.Add("Multiple Visual Studio 2022 instances found.");
            Errors.Add("Start this program with a parameter denoting one of the instance paths like follows:");

            string exe = Assembly.GetExecutingAssembly().Location;

            foreach (SetupInstance setupInstance in setupInstances)
            {
                Errors.Add($"{exe} \"{setupInstance.ProductPath}\"");
            }

            return false;
        }

        private static bool ValidateMSBuildInstalled(SetupInstance setupInstance)
        {
            SetupPackageReferences packages = new(setupInstance);

            if (packages[SetupPackageReferences.MSBuild] is null)
            {
                Errors.Add($"MSBuild not installed in Visual Studio 2022 instance '{setupInstance.Directory}'");
                return false;
            }

            return true;
        }
    }
}
