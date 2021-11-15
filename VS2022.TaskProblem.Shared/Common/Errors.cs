using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace VS2022.TaskProblem.Shared
{
    public static class Errors
    {
        private static readonly List<string> errors = new();

        public static void Add(string error) { errors.Add(error); }

        public static int Exit(string prefix)
        {
            string directory = Path.Combine(Utilities.SolutionDirectory, "output");
            string file = Path.Combine(directory, $"{prefix}-errors.txt");

            Directory.CreateDirectory(directory);
            File.WriteAllLines(file, errors);

            foreach (string error in errors)
            {
                Console.WriteLine(error);
                Debug.WriteLine(error);
            }

            return errors.Count;
        }
    }
}
