using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace VS2022.TaskProblem.Shared.Tasks
{
    public abstract class InfoTask : TaskBase
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

            public override string ToString()
            {
                return $"{Name} | {AssemblyVersion} | {FileVersion} | {Location}";
            }

            public static NameVersionsLocation Create(Assembly assembly)
            {
                string name = GetName(assembly);
                string assemblyVersion = GetAssemblyVersion(assembly);
                string fileVersion = GetFileVersion(assembly);

                return new(name, assemblyVersion, fileVersion, assembly.Location);
            }

            public static IEnumerable<NameVersionsLocation> Header()
            {
                yield return new NameVersionsLocation("Assembly", "Assembly Version", "File Version", "Location");
                yield return new NameVersionsLocation("---", "---", "---", "---");
            }

            public static IEnumerable<string> CreateTable(string title, IEnumerable<Assembly> assemblies)
            {
                yield return title;
                yield return string.Empty;

                foreach (string line in Header().Select(nvl => nvl.ToString()))
                    yield return line;

                foreach (string line in assemblies.Select(Create).OrderBy(nvl => nvl.Name).Select(nvl => nvl.ToString()))
                    yield return line;
            }

            public static IEnumerable<string> CreateTable(string title, Assembly assembly)
                => CreateTable(title, new [] { assembly });

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
                return assembly.IsDynamic ? "?" : FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion ?? "?";
            }
        }

        protected abstract string Hello { get; }
        protected abstract string Title { get; }
        protected abstract string FilePrefix { get; }

        protected override bool InternalExecute()
        {
            Message($"Hello from {Hello} ({TargetFramework})");
            Message(Environment.CurrentDirectory);

            List<string> lines = new();

            lines.Add($"# {Title} ({TargetFramework})");
            lines.Add("");

            Assembly assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            lines.AddRange(NameVersionsLocation.CreateTable("## Host", assembly));
            lines.AddRange(NameVersionsLocation.CreateTable("## Assemblies", AppDomain.CurrentDomain.GetAssemblies()));

            string directory = Path.Combine(Environment.CurrentDirectory, "..", "output");
            string file = Path.Combine(directory, $"{FilePrefix}-{TargetFramework}.md");

            Directory.CreateDirectory(directory);
            File.WriteAllLines(file, lines);

            return true;
        }
    }
}
