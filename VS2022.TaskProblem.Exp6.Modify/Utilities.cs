using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace VS2022.TaskProblem.Exp6.Modify
{
    public static class Utilities
    {
        public static DateTime ToDateTime(this FILETIME fileTime)
        {
            long value = ((long)fileTime.dwHighDateTime) << 32 | ((uint)fileTime.dwLowDateTime);

            return DateTime.FromFileTimeUtc(value);
        }

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
