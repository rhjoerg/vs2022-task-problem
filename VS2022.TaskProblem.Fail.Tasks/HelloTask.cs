﻿using Microsoft.Build.Framework;
using System;
using System.Linq;
using System.Reflection;

namespace VS2022.TaskProblem.Fail.Tasks
{
    public class HelloTask : Microsoft.Build.Utilities.Task
    {
        protected string TargetFramework
        {
            get => typeof(HelloTask).Assembly
                .GetCustomAttributes<AssemblyMetadataAttribute>()
                .Where(a => "TargetFramework".Equals(a.Key))
                .FirstOrDefault()?.Value ?? "Unknown";
        }

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, $"Hello from Fail ({TargetFramework})");

            return true;
        }
    }
}