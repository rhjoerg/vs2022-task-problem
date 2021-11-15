using Microsoft.Build.Framework;
using System;
using System.Linq;
using System.Reflection;

namespace VS2022.TaskProblem.Shared
{
    public abstract class TaskBase : Microsoft.Build.Utilities.Task
    {
        protected string TargetFramework
        {
            get => GetType().Assembly
                .GetCustomAttributes<AssemblyMetadataAttribute>()
                .Where(a => "TargetFramework".Equals(a.Key))
                .FirstOrDefault()?.Value ?? "Unknown";
        }

        public override bool Execute()
        {
            try
            {
                return InternalExecute();
            }
            catch (Exception ex)
            {
                Error(ex);
                return false;
            }
        }

        protected abstract bool InternalExecute();

        protected void Message(string message) { Log.LogMessage(MessageImportance.High, message); }

        protected void Error(string message) { Log.LogError(message); }

        protected virtual void Error(Exception exception) { Error(exception.ToString()); }
    }
}
