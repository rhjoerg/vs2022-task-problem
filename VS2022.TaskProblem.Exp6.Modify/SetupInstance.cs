using Microsoft.VisualStudio.Setup.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VS2022.TaskProblem.Exp6.Modify
{
    public class SetupInstance
    {
        private readonly ISetupInstance2 setupInstance;

        public string InstanceId { get => setupInstance.GetInstanceId(); }
        public DateTime GetInstallDate { get => setupInstance.GetInstallDate().ToDateTime(); }
        public string InstallationName { get => setupInstance.GetInstallationName(); }
        public string InstallationVersion { get => setupInstance.GetInstallationVersion(); }
        public string DisplayName { get => setupInstance.GetDisplayName(); }
        public string InstallationDescription { get => setupInstance.GetDescription(); }
        public InstanceState State { get => setupInstance.GetState(); }
        public IEnumerable<SetupPackageReference> Packages { get => setupInstance.GetPackages().Select(p => new SetupPackageReference(p)); }
        public SetupPackageReference Product { get => new(setupInstance.GetProduct()); }
        public string ProductPath { get => setupInstance.GetProductPath(); }
        public string Directory { get => Path.GetDirectoryName(ProductPath) ?? throw new NotSupportedException(); }
        public SetupErrorState Errors { get => new(setupInstance.GetErrors()); }
        public bool IsLaunchable { get => setupInstance.IsLaunchable(); }
        public bool IsComplete { get => setupInstance.IsComplete(); }
        public SetupPropertyStore Properties { get => new (setupInstance.GetProperties()); }
        public string EnginePath { get => setupInstance.GetEnginePath(); }
        public bool IsVisualStudio2022 { get => InstallationName.StartsWith("VisualStudio/17."); }

        public SetupInstance(ISetupInstance2 setupInstance) { this.setupInstance = setupInstance; }

        public string ResolvePath(string? relativePath = null)
            => setupInstance.ResolvePath(relativePath);
    }
}
