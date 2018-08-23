param
(
    [Parameter(Mandatory = $false)]
    [ValidateNotNullOrEmpty()]
    [string]
    $Command = "doBuild",

    [Parameter(Mandatory = $false)]
    [string]
    $TargetName
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function getInstallDir
{
    $installSubDir = "Steam\SteamApps\common\RimWorld"
    $installDir = "$(${Env:ProgramFiles(x86)})\$($installSubDir)"
    if (Test-Path $installDir)
    {
        return $installDir
    }

    $installDir = "$($Env:ProgramFiles)\$($installSubDir)"
    if (Test-Path $installDir)
    {
        return $installDir
    }

    return $null
}

function doPreBuild
{
    $thirdpartyDir = "$PSScriptRoot\ThirdParty"
    if (Test-Path "$thirdpartyDir\*.dll")
    {
        return
    }

    $installDir = getInstallDir
    if (!$installDir)
    {
        Write-Host -ForegroundColor Red `
            "Rimworld installation not found; see Readme for how to set up pre-requisites manually."

        exit 1
    }

    $depsDir = "$installDir\RimWorldWin_Data\Managed"
    Write-Host "Copying dependencies from installation directory"
    if (!(Test-Path $thirdpartyDir)) { mkdir $thirdpartyDir | Out-Null }
    Copy-Item -Force "$depsDir\UnityEngine.dll" "$thirdpartyDir\"
    Copy-Item -Force "$depsDir\Assembly-CSharp.dll" "$thirdpartyDir\"
}

function doPostBuild
{
    $installDir = getInstallDir
    if (!$installDir)
    {
        Write-Host -ForegroundColor Yellow `
            "No Steam installation found, build will not be published"

        return
    }

    $modDir = "$installDir\Mods\$($TargetName)\Assemblies"
    if (!(Test-Path $modDir)) { mkdir $modDir | Out-Null }

    # xcopy /y/s  "$(ProjectDir)dist\*" "%ProgramFiles(x86)%\Steam\SteamApps\common\RimWorld\Mods"
    # copy /y "$(TargetPath)" "%ProgramFiles(x86)%\Steam\SteamApps\common\RimWorld\Mods\$(TargetName)\Assemblies\"
    # copy /y "$(TargetDir)\*HugsLibChecker.dll" "%ProgramFiles(x86)%\Steam\SteamApps\common\RimWorld\Mods\$(TargetName)\Assemblies\"
}

& $Command