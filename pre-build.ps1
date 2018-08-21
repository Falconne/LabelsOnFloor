Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$thirdpartyDir = "$PSScriptRoot\ThirdParty"
if (Test-Path $thirdpartyDir)
{
    exit 0
}

$rimworldSubdir = "Steam\SteamApps\common\RimWorld\RimWorldWin_Data\Managed"
$installDir = "$(${Env:ProgramFiles(x86)})\$($rimworldSubdir)"
if (!(Test-Path $installDir))
{
    $installDir = "$($Env:ProgramFiles)\$($rimworldSubdir)"
    if (!(Test-Path $installDir))
    {
        Write-Host -ForegroundColor Red "Rimworld installation not found; see Readme for how to set up pre-requisites manually."
        exit 1
    }
}

Copy-Item -Force "$rimworldSubdir\UnityEngine.dll"  "$thirdpartyDir"
Copy-Item -Force "$rimworldSubdir\Assembly-CSharp.dll"  "$thirdpartyDir"