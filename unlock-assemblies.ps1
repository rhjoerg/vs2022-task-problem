[CmdletBinding()]
param
(
    [Parameter(Mandatory=$true, Position=0)]
    [string]
    $Project,

    [Parameter(Mandatory=$false)]
    [string[]]
    $ProcessNames = @("dotnet", "msbuild")
)

$processesToKill = @()
$assemblyDllName = $Project + ".dll"

$ProcessNames | ForEach-Object {

    $processName = $_

    Get-Process $processName -ErrorAction Ignore | ForEach-Object {

        $process = $_

        $process.Modules | ForEach-Object {

            [System.Diagnostics.ProcessModule]$module = $_
            $modulePath = $module.FileName

            if ($modulePath.EndsWith($assemblyDllName))
            {
                $processesToKill = $processesToKill + $process
            }
        }
    }
}

$processesToKill | ForEach-Object {

    [System.Diagnostics.Process]$process = $_
    $message = "Killing " + $process.Name + " (" + $process.Id + ")"
    Write-Host $message
    $process.Kill()
}
