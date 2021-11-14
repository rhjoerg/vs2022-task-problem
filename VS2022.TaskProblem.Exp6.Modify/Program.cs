using Microsoft.Build.Utilities;
using Microsoft.VisualStudio.Setup.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace VS2022.TaskProblem.Exp6.Modify
{
    public class Program
    {
        public static int Main(string[] _)
        {
            if (!VS2022.TryGetLocation(out DirectoryInfo? installDir))
                return Errors.Exit();

            return 0;
        }
    }
}
