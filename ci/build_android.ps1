$ErrorActionPreference = "Stop"

$unityExe = "C:\Program Files\Unity\Hub\Editor\2022.3.62f2\Editor\Unity.exe"
$logPath  = Join-Path $env:WORKSPACE "Build\unity_build.log"
$buildDir = Join-Path $env:WORKSPACE "Build\Android"

$buildRoot = Join-Path $env:WORKSPACE "Build"
New-Item -ItemType Directory -Force -Path $buildRoot | Out-Null
New-Item -ItemType Directory -Force -Path $buildDir | Out-Null

if (Test-Path $logPath) { Remove-Item $logPath -Force }

$args = @(
  "-batchmode", "-nographics", "-quit",
  "-projectPath", $env:WORKSPACE,
  "-executeMethod", "BuildScript.BuildAndroidApk",
  "-workspace", $env:WORKSPACE,
  "-logFile", "-"
)

Write-Host "[CI] WORKSPACE = $env:WORKSPACE"
Write-Host "[CI] logPath   = $logPath"
Write-Host "[CI] buildDir  = $buildDir"
Write-Host "[CI] buildRoot exists? " (Test-Path (Join-Path $env:WORKSPACE "Build"))

$unityOutput = & $unityExe @args 2>&1 | Tee-Object -Variable unityLines
$exitCode = $LASTEXITCODE
Write-Host "[CI] Unity process exit code: $exitCode"

$logText = ""
if (Test-Path $logPath) { $logText = Get-Content $logPath -Raw }

$hasSuccessMarker  = $logText -match '\[CI\] Build succeeded'
$hasBuildException = $logText -match '\[CI\] Build exception'

$apk = Get-ChildItem -Path $buildDir -Filter *.apk -ErrorAction SilentlyContinue | Select-Object -First 1
$hasApk = $null -ne $apk

if ($hasBuildException) { throw "BuildScript reported exception" }
if (($exitCode -ne 0) -and -not ($hasSuccessMarker -and $hasApk)) {
  throw "Unity failed (exit=$exitCode). No success marker or apk missing."
}

Write-Host "[CI] Unity build considered SUCCESS."
if ($hasApk) { Write-Host "[CI] APK: $($apk.FullName)" }
