using System.IO;
using System.Net.Http;
using VS2022.TaskProblem.Shared;

namespace VS2022.TaskProblem.Exp7.Build
{
    public static class Program
    {
        public static int Main(string[] _)
        {
            if (!Download(out string zipPath))
                return Errors.Exit("exp7");

            if (!Extract(zipPath))
                return Errors.Exit("exp7");

            return 0;
        }

        private static bool Download(out string zipPath)
        {
            string directory = Path.Combine(Utilities.SolutionDirectory, "downloads");

            zipPath = Path.Combine(directory, "msbuild-17.0.0.zip");

            Directory.CreateDirectory(directory);
            if (File.Exists(zipPath)) return true;

            HttpClient httpClient = new();
            byte[] buffer = httpClient.GetByteArrayAsync("https://github.com/dotnet/msbuild/archive/refs/tags/v17.0.0.zip").Result;

            File.WriteAllBytes(zipPath, buffer);
            return true;
        }

        private static bool Extract(string zipPath)
        {
            return true;
        }
    }
}
