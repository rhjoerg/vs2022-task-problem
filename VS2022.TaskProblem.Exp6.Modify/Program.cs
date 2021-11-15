using System.IO;
using VS2022.TaskProblem.Shared;

namespace VS2022.TaskProblem.Exp6.Modify
{
    public class Program
    {
        public static int Main(string[] _)
        {
            if (!VS2022.TryGetLocation(out DirectoryInfo? installDir))
                return Errors.Exit("exp6");

            return 0;
        }
    }
}
