[CmdletBinding()]
param (
    [Parameter(Mandatory=$true, Position=0)]
    [string]
    $Project
)

./unlock-assemblies.ps1 -Project $Project

$repositoryPath = Join-Path (Get-Location) "repository"

Get-ChildItem -Path $repositoryPath -Filter $Project -ErrorAction Ignore | ForEach-Object { Remove-Item $_ -Recurse }
