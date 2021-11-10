using Microsoft.Build.Framework;

namespace VS2022.TaskProblem.Fail.Tasks
{
    public class HelloTask : Microsoft.Build.Utilities.Task
    {
        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, "Hello from Fail");

            return true;
        }
    }
}