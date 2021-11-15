using System;
using System.IO;
using System.Linq;

namespace VS2022.TaskProblem.Shared.Common
{
    public static class Paths
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

        public static string OutputDirectory { get => CreateDirectory(Path.Combine(SolutionDirectory, "output")); }

        public static string DownloadsDirectory { get => CreateDirectory(Path.Combine(SolutionDirectory, "downloads")); }

        private static string CreateDirectory(string directory)
        {
            Directory.CreateDirectory(directory);
            return directory;
        }
    }
}
