using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace VS2022.TaskProblem.Shared.Common
{
    public static class Errors
    {
        private static readonly List<string> errors = new();

        public static void Add(string error) { errors.Add(error); }

        public static int Exit(string prefix)
        {
            string file = Path.Combine(Paths.OutputDirectory, $"{prefix}-errors.txt");

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
