param
(
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]
    $Command
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$targetName = (Get-ChildItem -Path "$PSScriptRoot\src" -Recurse -Filter *.csproj | select -First 1).Basename
$distDir = "$PSScriptRoot\dist"

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

function getProjectDir
{
    return "$PSScriptRoot\src\$targetName"
}

$assemblyInfoFile = "$(getProjectDir)\properties\AssemblyInfo.cs"

function getFileVersionInAssemblyInfo
{
    $content = Get-Content -Raw $assemblyInfoFile
    if (!($content -match '"(\d+\.\d+\.\d+\.\d+)"'))
    {
        throw "Version info not found in $assemblyInfoFile"
    }

    return $matches[1]
}

function updateToGameVersion
{
    $installDir = getInstallDir
    if (!$installDir)
    {
        Write-Host -ForegroundColor Red `
            "Rimworld installation not found; not setting game version."

        return
    }

    $currentVersionInFile = [version] (getFileVersionInAssemblyInfo)

    $gameVersionFile = "$installDir\Version.txt"
    $gameVersionWithRev = Get-Content $gameVersionFile
    $gameVersion = [version] ($gameVersionWithRev.Split(" "))[0]

    $content = Get-Content -Raw $assemblyInfoFile
    $newContent = $content -replace '"\d+\.\d+(\.\d+\.\d+")', "`"$($gameVersion.Major).$($gameVersion.Minor)`$1"

    if ($newContent -eq $content)
    {
        return
    }

    Set-Content -Encoding UTF8 -Path $assemblyInfoFile $newContent

    # Check game version in mod's About.xml
    $aboutFile = Resolve-Path "$PSScriptRoot\mod-structure\About\About.xml"
    $aboutFileContent = [xml] (Get-Content -Raw $aboutFile)
    if ($aboutFileContent.ModMetaData.targetVersion -ne $gameVersion.ToString())
    {
        Write-Host "Updating to mod to game version $gameVersion"
        $aboutFileContent.ModMetaData.targetVersion = $gameVersion.ToString()
        $aboutFileContent.Save($aboutFile)
    }
}

function copyDependencies
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

function doPreBuild
{
    removePath $distDir
    copyDependencies
    updateToGameVersion
}

function doPostBuild
{
    $distTargetDir = "$distDir\$targetName"
    removePath $distDir

    $targetDir = "$(getProjectDir)\bin\Release"
    $targetPath = "$targetDir\$targetName.dll"
    $distAssemblyDir = "$distTargetDir\Assemblies"
    mkdir $distAssemblyDir | Out-Null

    Copy-Item -Recurse -Force "$PSScriptRoot\mod-structure\*" $distTargetDir
    Copy-Item -Force $targetPath $distAssemblyDir
    Copy-Item -Force "$targetDir\*HugsLibChecker.dll" $distAssemblyDir

    Write-Host "Creating distro package"
    $version = getFileVersionInAssemblyInfo
    $distZip = "$distDir\$targetName.$version.zip"
    Compress-Archive -Path $distTargetDir -DestinationPath $distZip -CompressionLevel Optimal
    Write-Host "Created $distZip"


    $installDir = getInstallDir
    if (!$installDir)
    {
        Write-Host -ForegroundColor Yellow `
            "No Steam installation found, build will not be published"

        return
    }

    $modsDir = "$installDir\Mods"
    $modDir = "$modsDir\$targetName"
    removePath $modDir

    Write-Host "Copying mod to $modDir"
    Copy-Item -Recurse -Force -Exclude *.zip "$distDir\*" $modsDir
}

& $Command