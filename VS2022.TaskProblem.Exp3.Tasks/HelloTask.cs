using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using VS2022.TaskProblem.Shared;

namespace VS2022.TaskProblem.Exp3.Tasks
{
    public class HelloTask : TaskBase
    {
        public class NameVersionsLocation
        {
            public string Name { get; }
            public string AssemblyVersion { get; }
            public string FileVersion { get; }
            public string Location { get; }

            public NameVersionsLocation(string name, string assemblyVersion, string fileVersion, string location)
            {
                Name = name;
                AssemblyVersion = assemblyVersion;
                FileVersion = fileVersion;
                Location = location;
            }

            public static NameVersionsLocation Create(Assembly assembly)
            {
                string name = GetName(assembly);
                string assemblyVersion = GetAssemblyVersion(assembly);
                string fileVersion = GetFileVersion(assembly);

                return new(name, assemblyVersion, fileVersion, assembly.Location);
            }

            public override string ToString()
            {
                return $"{Name} | {AssemblyVersion} | {FileVersion} | {Location}";
            }

            public static IEnumerable<NameVersionsLocation> Header()
            {
                yield return new NameVersionsLocation("Assembly", "A.V.", "F.V.", "Location");
                yield return new NameVersionsLocation("---", "---", "---", "---");
            }

            private static string GetName(Assembly assembly)
            {
                string fullName = assembly.FullName ?? string.Empty;
                int commaPos = fullName.IndexOf(',');

                return commaPos < 0 ? fullName : fullName.Substring(0, commaPos);
            }

            private static string GetAssemblyVersion(Assembly assembly)
            {
                string fullName = assembly.FullName ?? string.Empty;
                int firstCommaPos = fullName.IndexOf(',');
                int secondCommaPos = fullName.IndexOf(',', firstCommaPos + 1);

                return secondCommaPos < 0 ? fullName : fullName.Substring(firstCommaPos + 1, secondCommaPos - firstCommaPos - 1).Trim().Substring(8);
            }

            private static string GetFileVersion(Assembly assembly)
            {
                return FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion ?? "?";
            }
        }

        protected override bool InternalExecute()
        {
            Message($"Hello from Exp3 ({TargetFramework})");
            Message(Environment.CurrentDirectory);

            List<string> lines = new();

            Prologue(lines);
            Host(lines);
            Assemblies(lines);
            Save(lines);

            return true;
        }

        protected virtual void Prologue(List<string> lines)
        {
            lines.Add("# Experiment 3 - Output");
            lines.Add("");
        }

        protected virtual void Host(List<string> lines)
        {
            lines.Add("## Host");
            lines.Add("");

            Assembly assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

            lines.AddRange(NameVersionsLocation.Header().Select(nvl => nvl.ToString()));
            lines.Add(NameVersionsLocation.Create(assembly).ToString());
        }

        protected virtual void Assemblies(List<string> lines)
        {
            lines.Add("## Assemblies");
            lines.Add("");

            IEnumerable<NameVersionsLocation> nvls = AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(NameVersionsLocation.Create)
                .OrderBy(nvl => nvl.Name);

            lines.AddRange(NameVersionsLocation.Header().Select(nvl => nvl.ToString()));
            lines.AddRange(nvls.Select(nvl => nvl.ToString()));
        }

        protected virtual void Save(List<string> lines)
        {
            string directory = Path.Combine(Environment.CurrentDirectory, "..", "output");
            string file = Path.Combine(directory, $"exp3-{TargetFramework}.md");

            Directory.CreateDirectory(directory);
            File.WriteAllLines(file, lines);
        }
    }
}
