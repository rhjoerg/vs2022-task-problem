using System;
using System.IO;
using System.Linq;

namespace VS2022.TaskProblem.Exp6.Modify
{
    public static class Utilities
    {
        public static string SolutionDirectory
        {
            get
            {
                DirectoryInfo? directory = new(Environment.CurrentDirectory);

                while (directory is not null)
                {
                    if (directory.EnumerateFiles("*.sln", SearchOption.TopDirectoryOnly).Any())
                        return directory.FullName;

                    directory = directory.Parent;
                }

                throw new FileNotFoundException("Solution not found");
            }
        }
    }
}
