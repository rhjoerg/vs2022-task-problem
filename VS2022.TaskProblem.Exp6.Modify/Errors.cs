using System.Collections.Generic;
using System.IO;

namespace VS2022.TaskProblem.Exp6.Modify
{
    public static class Errors
    {
        private static readonly List<string> errors = new();

        public static void Add(string error)
        {
            errors.Add(error);
        }

        public static int Write()
        {
            string directory = Path.Combine(Utilities.SolutionDirectory, "output");
            string file = Path.Combine(directory, "Exp6.Errors.txt");

            Directory.CreateDirectory(directory);
            File.WriteAllLines(file, errors);

            return errors.Count;
        }
    }
}
