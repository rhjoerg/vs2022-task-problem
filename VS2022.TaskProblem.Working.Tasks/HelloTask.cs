using Microsoft.Build.Framework;
using System.Linq;
using System.Reflection;
using VS2022.TaskProblem.Shared;

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
