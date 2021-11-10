# vs2022-task-problem

Minimal project to investigate VS2022 and .net6.0 custom task problem.

I upgraded to Visual Studio 2022. I upgraded some of my projects to ```<TargetFramework>net6.0</TargetFramework>```. Among those projects
is a custom build task assembly. Building projects that "use" (```<UsingTask TaskName=" ...```) this assembly works fine when builing from the command line (```dotnet build ...```) but fails when building from within Visual Studio.

To isolate the problem and reproduce the failure, I created a [GitHub repository](https://github.com/rhjoerg/vs2022-task-problem) containing some (not-so-minimal) solution with several small projects.

## The failure reproduction

The project [VS2022.TaskProblem.Fail.Tasks]()