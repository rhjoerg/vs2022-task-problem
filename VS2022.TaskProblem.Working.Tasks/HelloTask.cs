using VS2022.TaskProblem.Shared.Tasks;

namespace VS2022.TaskProblem.Working.Tasks
{
    public class HelloTask : TaskBase
    {
        protected override bool InternalExecute()
        {
            Message($"Hello from Working ({TargetFramework})");

            return true;
        }
    }
}
