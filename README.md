# vs2022-task-problem

[repository]: https://github.com/rhjoerg/vs2022-task-problem
[uninst]: https://github.com/rhjoerg/vs2022-task-problem/blob/main/uninstall-package.ps1
[unlock]: https://github.com/rhjoerg/vs2022-task-problem/blob/main/unlock-assemblies.ps1
[failtaskproj]: https://github.com/rhjoerg/vs2022-task-problem/blob/main/VS2022.TaskProblem.Fail.Tasks/VS2022.TaskProblem.Fail.Tasks.csproj
[failtasktask]: https://github.com/rhjoerg/vs2022-task-problem/blob/main/VS2022.TaskProblem.Fail.Tasks/HelloTask.cs
[failuseproj]: https://github.com/rhjoerg/vs2022-task-problem/blob/main/VS2022.TaskProblem.Fail.Using/VS2022.TaskProblem.Fail.Using.csproj
[worktaskproj]: https://github.com/rhjoerg/vs2022-task-problem/blob/main/VS2022.TaskProblem.Working.Tasks/VS2022.TaskProblem.Working.Tasks.csproj
[worktargets]: https://github.com/rhjoerg/vs2022-task-problem/blob/main/VS2022.TaskProblem.Working.Tasks/build/VS2022.TaskProblem.Working.Tasks.targets
[workuseproj]: https://github.com/rhjoerg/vs2022-task-problem/blob/main/VS2022.TaskProblem.Working.Using/VS2022.TaskProblem.Working.Using.csproj

Minimal project to investigate VS2022 and .net6.0 custom task problem.

I upgraded to Visual Studio 2022. I upgraded some of my projects to ```<TargetFramework>net6.0</TargetFramework>```. Among those projects
is a custom build task assembly. Building projects that "use" (```<UsingTask TaskName=" ...```) this assembly works fine when builing from the command line (```dotnet build ...```) but fails when building from within Visual Studio.

To isolate the problem and reproduce the failure, I created a (this) [GitHub repository][repository] containing a (not-so-minimal)
Visual Studio solution with several small projects.

## Reproducing the Failure

### Prerequisites

- Visual Studio 2022
- dotnet 6.0.100
- PowerShell 7.1+

### The code

The project [VS2022.TaskProblem.Fail.Tasks][failtaskproj] contains a simple Task ([Source][failtasktask]):

```c#
    using Microsoft.Build.Framework;
    using System.Reflection;

    public class HelloTask : Microsoft.Build.Utilities.Task
    {
        protected string TargetFramework
        {
            get => typeof(HelloTask).Assembly
                .GetCustomAttributes<AssemblyMetadataAttribute>()
                .Where(a => "TargetFramework".Equals(a.Key))
                .FirstOrDefault()?.Value ?? "Unknown";
        }

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, $"Hello from Fail ({TargetFramework})");

            return true;
        }
    }
```

The project [VS2022.TaskProblem.Fail.Using][failuseproj] then uses the task:

```xml
  <Import Project="$(FailTasksTargets)"/>

  <Target Name="SayHello" AfterTargets="Compile">
    <HelloTask />
  </Target>
```

Building the solution from the command line (```dotnet build vs2022-task-problem.sln```) shows the desired output:

```
Hello from Fail (net6.0)
VS2022.TaskProblem.Fail.Using -> C:\source\vs2022-task-problem\VS2022.TaskProblem.Fail.Using\bin\Debug\net6.0\VS2022.TaskProblem.Fail.Using.dll
```

Building from within Visual Studio 2022 fails:

```
...
Could not load file or assembly 'System.Runtime, Version=6.0.0.0, ...
...
```

### Side-Note

Whilst cleaning, building or rebuilding, the PowerShell script [uninstall-package.ps1][uninst] fails, as the installed
assembly is locked by a dangling "msbuild" process. The indirectly invoked [unlock-assemblies.ps1][unlock] doesn't find
(yet) the locking process.

The following PowerShell one-liner gets rid of this/these process(es):

```powershell
Get-Process "msbuild" -ErrorAction Ignore | ForEach-Object { $_.Kill($true) }
```

## The Workaround

The project [VS2022.TaskProblem.Working.Tasks][worktaskproj] contains a possible workaround. It targets ```net6.0``` and ```net472```:

```xml
  <PropertyGroup>
    <TargetFrameworks>net6.0;net472</TargetFrameworks>
  </PropertyGroup>
```

The appropriate assembly to use is selected in the [build/VS2022.TaskProblem.Working.Tasks.targets][worktargets] file:

```xml
  <PropertyGroup>
    <HelloTaskBaseDir>$([MSBuild]::NormalizeDirectory( $(MSBuildThisFileDirectory), "..", "lib" ))</HelloTaskBaseDir>
    <HelloTaskAssemblyDir Condition="'$(MSBuildRuntimeType)'=='Core'">$([MSBuild]::NormalizeDirectory($(HelloTaskBaseDir),"net6.0"))</HelloTaskAssemblyDir>
    <HelloTaskAssemblyDir Condition="'$(MSBuildRuntimeType)'!='Core'">$([MSBuild]::NormalizeDirectory($(HelloTaskBaseDir),"net472"))</HelloTaskAssemblyDir>
    <HelloTaskAssemblyFile>$([MSBuild]::NormalizePath($(HelloTaskAssemblyDir),"VS2022.TaskProblem.Working.Tasks.dll"))</HelloTaskAssemblyFile>
  </PropertyGroup>

  <UsingTask TaskName="VS2022.TaskProblem.Working.Tasks.HelloTask" AssemblyFile="$(HelloTaskAssemblyFile)" />
```

The associated project [VS2022.TaskProblem.Working.Using][workuseproj] builds just fine:

Output from command-line build:

```
Hello from Working (net6.0)
```

Output from within Visual Studio:

```
Hello from Working (net472)
```

This is of course not an acceptable workaround, since the greatest common denominator between ```net6.0``` and ```net472``` is
```netstandard2.0``` and I don't want this restriction in my custom tasks!

