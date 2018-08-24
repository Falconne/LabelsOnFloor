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

function removePath($path)
{
    while ($true)
    {
        if (!(Test-Path $path))
        {
            return
        }

        Write-Host "Deleting $path"
        try
        {
            Remove-Item -Recurse $path
            break
        }
        catch
        {
            Write-Host "Could not remove $path, will retry"
            Start-Sleep 3
        }
    }
}

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
    $distDir = "$PSScriptRoot\dist"
    $distTargetDir = "$distDir\$TargetName"
    removePath $distDir

    $targetDir = "$PSScriptRoot\src\$TargetName\bin\Release"
    $targetPath = "$targetDir\$TargetName.dll"
    $distAssemblyDir = "$distTargetDir\Assemblies"
    mkdir $distAssemblyDir | Out-Null

    Copy-Item -Recurse -Force "$PSScriptRoot\mod-structure\*" $distTargetDir
    Copy-Item -Force $targetPath $distAssemblyDir
    Copy-Item -Force "$targetDir\*HugsLibChecker.dll" $distAssemblyDir


    $installDir = getInstallDir
    if (!$installDir)
    {
        Write-Host -ForegroundColor Yellow `
            "No Steam installation found, build will not be published"

        return
    }

    $modsDir = "$installDir\Mods"
    $modDir = "$modsDir\$TargetName"
    removePath $modDir

    Write-Host "Copying mod to $modDir"
    Copy-Item -Recurse -Force "$distDir\*" $modsDir
}

& $Command