using Microsoft.VisualStudio.Setup.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VS2022.TaskProblem.Exp6.Modify
{
    public class SetupInstances : IEnumerable<SetupInstance>
    {
        private List<SetupInstance> setupInstances = new();

        public SetupInstances(IEnumerable<SetupInstance> setupInstances)
        {
            this.setupInstances.AddRange(setupInstances);
        }

        private static IEnumerable<SetupInstance> GetInstance(SetupConfiguration setupConfiguration, string path)
        {
            if (setupConfiguration.GetInstanceForPath(path) is ISetupInstance2 setupInstance)
                return new SetupInstance[] { new(setupInstance) };

            return Array.Empty<SetupInstance>();
        }

        private static IEnumerable<SetupInstance> GetAllInstances(SetupConfiguration setupConfiguration)
        {
            List<SetupInstance> result = new();
            IEnumSetupInstances allInstances = setupConfiguration.EnumAllInstances();
            ISetupInstance[] rgelt = new ISetupInstance[1];

            while (true)
            {
                allInstances.Next(1, rgelt, out int pceltFetched);

                if (pceltFetched == 0) break;

                if (rgelt[0] is ISetupInstance2 setupInstance)
                    result.Add(new(setupInstance));
            }

            return result;
        }

        private static IEnumerable<SetupInstance> GetInstances(SetupConfiguration setupConfiguration, string? path)
            => path is null ? GetAllInstances(setupConfiguration) : GetInstance(setupConfiguration, path);

        public IEnumerator<SetupInstance> GetEnumerator() => setupInstances.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => setupInstances.GetEnumerator();

        public static SetupInstances Create(SetupConfiguration setupConfiguration, string? path = null)
            => new(GetInstances(setupConfiguration, path));

        public static SetupInstances CreateVisualStudio2022Instances(SetupConfiguration setupConfiguration, string? path = null)
            => new (Create(setupConfiguration, path).Where(i => i.IsVisualStudio2022));
    }
}
