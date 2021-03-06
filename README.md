# vs2022-task-problem

[repository]: https://github.com/rhjoerg/vs2022-task-problem
[uninst]: https://github.com/rhjoerg/vs2022-task-problem/blob/main/uninstall-package.ps1
[unlock]: https://github.com/rhjoerg/vs2022-task-problem/blob/main/unlock-assemblies.ps1
[dirbuildprops]: https://github.com/rhjoerg/vs2022-task-problem/blob/main/Directory.Build.props

- [Reproducing the Failure](#reproducing-the-failure)
- [The Workaround](#the-workaround)
- [Experiment 1 - Explicit ToolsVersion](#experiment-1---explicit-toolsversion)
- [Experiment 2 - MSBuildToolsPath](#experiment-2---msbuildtoolspath)
- [Experiment 3 - More Information Required](#experiment-3---more-information-required)
- [Experiment 4 - Remove MSBuild](#experiment-4---remove-msbuild)
- [Experiment 5 - net48](#experiment-5---net48)
- [Stop Right Now](#stop-right-now)

I upgraded to Visual Studio 2022. I upgraded some of my projects to ```<TargetFramework>net6.0</TargetFramework>```. Among those projects
is a custom build task assembly. Building projects that "use" (```<UsingTask TaskName="...```) this assembly works fine when builing from the command line (```dotnet build ...```) but fails when building from within Visual Studio.

To isolate the problem and reproduce the failure, I created a (this) [GitHub repository][repository] containing a (not-so-minimal)
Visual Studio solution with several small projects.

## Reproducing the Failure

[failtaskproj]: https://github.com/rhjoerg/vs2022-task-problem/blob/main/VS2022.TaskProblem.Fail.Tasks/VS2022.TaskProblem.Fail.Tasks.csproj
[failtasktask]: https://github.com/rhjoerg/vs2022-task-problem/blob/main/VS2022.TaskProblem.Fail.Tasks/HelloTask.cs
[failuseproj]: https://github.com/rhjoerg/vs2022-task-problem/blob/main/VS2022.TaskProblem.Fail.Using/VS2022.TaskProblem.Fail.Using.csproj

### Prerequisites

- Visual Studio 2022 with:
  - "MSBuild" component
  - ".Net 6.0 Runtime" component
  - "Visual Studio extension development" workflow
- dotnet 6.0.100
- PowerShell 7.1+
- Build/run on Windows 10

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
@("msbuild", "dotnet") | ForEach-Object { Get-Process $_ -ErrorAction Ignore | ForEach-Object { $_.Kill($true) } }
```

## The Workaround

[worktaskproj]: https://github.com/rhjoerg/vs2022-task-problem/blob/main/VS2022.TaskProblem.Working.Tasks/VS2022.TaskProblem.Working.Tasks.csproj
[worktargets]: https://github.com/rhjoerg/vs2022-task-problem/blob/main/VS2022.TaskProblem.Working.Tasks/build/VS2022.TaskProblem.Working.Tasks.targets
[workuseproj]: https://github.com/rhjoerg/vs2022-task-problem/blob/main/VS2022.TaskProblem.Working.Using/VS2022.TaskProblem.Working.Using.csproj

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

### Side-Note

The project failed to build at first due to some "implicit usings". Who the heck added this feature without warning. I ended up
disabling it on the solution level in the [Directory.Build.props][dirbuildprops] file:

```xml
  <PropertyGroup>
    <LangVersion>10.0</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
  </PropertyGroup>
```

This of course required some additional ```using``` statements in the [Task][failtasktask] of the failing project.

## Experiment 1 - Explicit ToolsVersion

[exp1proj]: https://github.com/rhjoerg/vs2022-task-problem/blob/main/VS2022.TaskProblem.Exp1.Using/VS2022.TaskProblem.Exp1.Using.csproj

The [VS2022.TaskProblem.Exp1.Using][exp1proj] project is a copy of the failing project above, explicitely stating the toolset
to use:

```xml
<Project Sdk="Microsoft.NET.Sdk" ToolsVersion="Current">
...
```

To no avail: this project as well fails to build witin Visual Studio.

## Experiment 2 - MSBuildToolsPath

[exp2proj]: https://github.com/rhjoerg/vs2022-task-problem/blob/main/VS2022.TaskProblem.Exp2/VS2022.TaskProblem.Exp2.csproj

This [project][exp2proj] simply shows the value of the ```MSBuildToolsPath``` property. Building within Visual Studio gives the following result:

```
  MSBuildToolsPath = 'C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\amd64'
```

This looks wrong. I have an "Intel(R) Core(TM) i7-9750H CPU @ 2.60 GHz" machine and not an AMD.

Scanning ```C:\Program Files\Microsoft Visual Studio\2022``` for ```MSBuildToolsPath``` shows that the property is set
in the ```MSBuild.exe.config``` file:

```xml
      <property name="MSBuildToolsPath" value="$([MSBuild]::GetCurrentToolsDirectory())" />
      <property name="MSBuildToolsPath32" value="$([MSBuild]::GetToolsDirectory32())" />
      <property name="MSBuildToolsPath64" value="$([MSBuild]::GetToolsDirectory64())" />
```

The following target...

```xml
  <Target Name="Messages2" AfterTargets="Compile" DependsOnTargets="Messages1">
    <Message Text="GetCurrentToolsDirectory = '$([MSBuild]::GetCurrentToolsDirectory())'" Importance="high" />
    <Message Text="GetToolsDirectory32 = '$([MSBuild]::GetToolsDirectory32())'" Importance="high" />
    <Message Text="GetToolsDirectory64 = '$([MSBuild]::GetToolsDirectory64())'" Importance="high" />
  </Target>
```

... gives the following confusing(!) output

```
  GetCurrentToolsDirectory = 'C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\amd64'
  GetToolsDirectory32 = 'C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin'
  GetToolsDirectory64 = 'C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\amd64'
```

Further investigation of the config file shows that mostly outdated versions of tools are used (excerpt):

```xml
      <dependentAssembly>
        <assemblyIdentity name="Microsoft.Build.Framework" culture="neutral" publicKeyToken="b03f5f7f11d50a3a" />
        <bindingRedirect oldVersion="0.0.0.0-99.9.9.9" newVersion="15.1.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="Microsoft.Build" culture="neutral" publicKeyToken="b03f5f7f11d50a3a" />
        <bindingRedirect oldVersion="0.0.0.0-99.9.9.9" newVersion="15.1.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="Microsoft.Build.Conversion.Core" culture="neutral" publicKeyToken="b03f5f7f11d50a3a" />
        <bindingRedirect oldVersion="0.0.0.0-99.9.9.9" newVersion="15.1.0.0" />
      </dependentAssembly>
```

The actual DLLs in the ```Bin``` directory have version 17.0.0.52104 &ndash; more confusion.

## Experiment 3 - More Information Required

[exp3taskproj]: https://github.com/rhjoerg/vs2022-task-problem/blob/main/VS2022.TaskProblem.Exp3.Tasks/VS2022.TaskProblem.Exp3.Tasks.csproj
[exp3useproj]: https://github.com/rhjoerg/vs2022-task-problem/blob/main/VS2022.TaskProblem.Exp3.Using/VS2022.TaskProblem.Exp3.Using.csproj

This experiment (projects [VS2022.TaskProblem.Exp3.Tasks][exp3taskproj] and [VS2022.TaskProblem.Exp3.Using][exp3useproj]) has a more
interesting custom task that create markdown files containing information about the running MSBuild instance and the loaded assemblies.

The complete outputs are [net6.0 output](output/exp3-net6.0.md) and [net472 output](output/exp3-net472.md).

One of the significant differences: net6.0 uses "netstandard 2.1" and all the net6.0 goodies whereas net472 uses "netstandard 2.0"
and net4.0.

## Experiment 4 - Remove MSBuild

Since dotnet has its own MSBuild, I launched the Visual Studio Installer to remove its MSBuild component. But
there are dependants to be removed as well &ndash; mainly the whole .Net development. Therefore: FAILURE.

## Experiment 5 - net48

[exp5taskproj]: https://github.com/rhjoerg/vs2022-task-problem/blob/main/VS2022.TaskProblem.Exp5.Tasks/VS2022.TaskProblem.Exp5.Tasks.csproj

Whilst reading various articles on stackoverflow and other sites I stumbled over the fact, that "net48" is supported.

This [experiment][exp5taskproj] is therefore a copy of the experiment 3 but targeting "net6.0" and "net48". The [output](output/exp5-net48.md)
still shows "netstandard 2.0" and "System 4.0".

## Stop Right Now

[appcfgdoc]: https://docs.microsoft.com/en-us/dotnet/framework/configure-apps/

After inspection of the ```MSBuild.exe.config``` and reading of the [documentation][appcfgdoc] about these files, I realized that I probably need my very
own build of MSBuild.

I stop this project right now. The next steps would be:

1. Build my own x64 version of MSBuild.exe.
2. Convince Visual Studio to us it by creating a special WSIX.
3. Distribute this unsigned WSIX to developers.
4. Most probably stumble over the same problem that lead to the "downgrade" of the existing MSBuild.exe in the first place.

