$ErrorActionPreference = "Stop"

$unityExe = "C:\Program Files\Unity\Hub\Editor\2022.3.62f2\Editor\Unity.exe"

$buildDir = Join-Path $env:WORKSPACE "Build\Android"
$buildRoot = Join-Path $env:WORKSPACE "Build"
New-Item -ItemType Directory -Force -Path $buildRoot | Out-Null
New-Item -ItemType Directory -Force -Path $buildDir  | Out-Null

Write-Host "[CI] WORKSPACE = $env:WORKSPACE"
Write-Host "[CI] buildDir  = $buildDir"

$cmdArgs = @(
  "`"$unityExe`"",
  "-batchmode", "-nographics", "-quit",
  "-projectPath", "`"$env:WORKSPACE`"",
  "-executeMethod", "BuildScript.BuildAndroidApk",
  "-logFile", "-",
  "-stackTraceLogType", "Full"
) -join " "

Write-Host "[CI] Running: $cmdArgs"

cmd /c $cmdArgs
$exitCode = $LASTEXITCODE
Write-Host "[CI] Unity process exit code: $exitCode"

if ($exitCode -ne 0) {
  throw "Unity failed (exit=$exitCode). Check console log above for details."
}

$apk = Get-ChildItem -Path $buildDir -Filter *.apk -ErrorAction SilentlyContinue | Select-Object -First 1
if ($null -eq $apk) { throw "Unity returned 0 but apk missing: $buildDir" }
Write-Host "[CI] APK: $($apk.FullName)"
