using Microsoft.VisualStudio.Setup.Configuration;

namespace VS2022.TaskProblem.Exp6.Modify
{
    public class SetupPackageReference
    {
        private readonly ISetupPackageReference setupPackageReference;

        public string Id { get => setupPackageReference.GetId(); }
        public string Version { get => setupPackageReference.GetVersion(); }
        public string Chip { get => setupPackageReference.GetChip(); }
        public string Language { get => setupPackageReference.GetLanguage(); }
        public string Branch { get => setupPackageReference.GetBranch(); }
        public string Type { get => setupPackageReference.GetType(); }
        public string UniqueId { get => setupPackageReference.GetUniqueId(); }
        public bool IsExtension { get => setupPackageReference.GetIsExtension(); }

        public SetupPackageReference(ISetupPackageReference setupPackageReference) { this.setupPackageReference = setupPackageReference;}
    }
}
