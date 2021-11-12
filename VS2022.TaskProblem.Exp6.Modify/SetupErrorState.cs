using Microsoft.VisualStudio.Setup.Configuration;

namespace VS2022.TaskProblem.Exp6.Modify
{
    public class SetupErrorState
    {
        private readonly ISetupErrorState setupErrorState;

        public SetupErrorState(ISetupErrorState setupErrorState) { this.setupErrorState = setupErrorState; }
    }
}
