
Get-Process "msbuild" -ErrorAction Ignore | ForEach-Object { $_.Kill($true) }
Get-Process "dotnet" -ErrorAction Ignore | ForEach-Object { $_.Kill($true) }

dotnet clean vs2022-task-problem.sln
dotnet build vs2022-task-problem.sln
