using VS2022.TaskProblem.Shared;

namespace VS2022.TaskProblem.Fail.Tasks
{
    public class HelloTask : TaskBase
    {
        protected override bool InternalExecute()
        {
            Message($"Hello from Fail ({TargetFramework})");

            return true;
        }
    }
}