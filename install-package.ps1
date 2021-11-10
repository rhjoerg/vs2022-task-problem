[CmdletBinding()]
param (
    [Parameter(Mandatory=$true, Position=0)]
    [string]
    $Project,
    
    [Parameter(Mandatory=$true, Position=1)]
    [string]
    $Configuration
)

./uninstall-package.ps1 -Project $Project

$objConfigDir = Join-Path (Get-Location) $Project "obj" $Configuration

Get-ChildItem -Path $objConfigDir -Filter "*.nuspec" | ForEach-Object {

    $nuspecPath = $_
    [xml]$nuspecXml = Get-Content -Path $nuspecPath
    $version = $nuspecXml.package.metadata.version
    $nupkgName = $Project + "." + $version + ".nupkg"
    $nupkgSource = Join-Path (Get-Location) "packages" $nupkgName
    $packageInstallDir = (Join-Path (Get-Location) "repository" $Project).ToLower()
    $packageInstallDir = New-Item $packageInstallDir -ItemType Directory
    $versionInstallDir = Join-Path $packageInstallDir.FullName $version
    $versionInstallDir = New-Item $versionInstallDir -ItemType Directory
    Copy-Item $nupkgSource -Destination $versionInstallDir
    $nupkgTarget = Join-Path $versionInstallDir $nupkgName
    Expand-Archive -Path $nupkgTarget -DestinationPath $versionInstallDir
}