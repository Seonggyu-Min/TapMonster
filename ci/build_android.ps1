$ErrorActionPreference = "Stop"

$unityExe = "C:\Program Files\Unity\Hub\Editor\2022.3.62f2\Editor\Unity.exe"

$buildRoot = Join-Path $env:WORKSPACE "Build"
$buildDir  = Join-Path $buildRoot "Android"
$logPath   = Join-Path $buildRoot "unity_build.log"

New-Item -ItemType Directory -Force -Path $buildRoot | Out-Null
New-Item -ItemType Directory -Force -Path $buildDir  | Out-Null

Write-Host "[CI] WORKSPACE = $env:WORKSPACE"
Write-Host "[CI] buildDir  = $buildDir"

$env:CI_KEYSTORE_PATH = $env:ANDROID_KEYSTORE
$env:CI_KEYSTORE_PASS = $env:ANDROID_KEYSTORE_PASS
$env:CI_KEYALIAS_NAME = $env:ANDROID_KEYALIAS_NAME
$env:CI_KEYALIAS_PASS = $env:ANDROID_KEYALIAS_PASS

Write-Host "[CI] CI_KEYSTORE_PATH exists? " (Test-Path $env:CI_KEYSTORE_PATH)
Write-Host "[CI] CI_KEYSTORE_PASS length = " ($env:CI_KEYSTORE_PASS.Length)
Write-Host "[CI] CI_KEYALIAS_NAME = " $env:CI_KEYALIAS_NAME
Write-Host "[CI] CI_KEYALIAS_PASS length = " ($env:CI_KEYALIAS_PASS.Length)

if (Test-Path $logPath) { Remove-Item $logPath -Force }

$args = @(
  "-batchmode",
  "-nographics",
  "-quit",
  "-projectPath", "`"$env:WORKSPACE`"",
  "-executeMethod", "BuildScript.BuildAndroidApk",
  "-workspace", "`"$env:WORKSPACE`"",
  "-logFile", "`"$logPath`"",
  "-stackTraceLogType", "Full"
) -join " "

Write-Host "[CI] Running Unity:"
Write-Host "$unityExe $args"

cmd /c "`"$unityExe`" $args"
$exitCode = $LASTEXITCODE
Write-Host "[CI] Unity process exit code: $exitCode"

$logText = ""
if (Test-Path $logPath) {
    $logText = Get-Content $logPath -Raw
}

$hasSuccessMarker  = $logText -match '\[CI\] Build succeeded'
$hasBuildException = $logText -match '\[CI\] Build exception'

$apk = Get-ChildItem -Path $buildDir -Filter *.apk -ErrorAction SilentlyContinue | Select-Object -First 1
$hasApk = $null -ne $apk

if ($hasBuildException) {
    throw "BuildScript reported exception. Check $logPath"
}

if (($exitCode -ne 0) -and -not ($hasSuccessMarker -and $hasApk)) {
    throw "Unity failed (exit=$exitCode). Check $logPath"
}

Write-Host "[CI] Unity build considered SUCCESS."
if ($hasApk) {
    Write-Host "[CI] APK: $($apk.FullName)"
}
