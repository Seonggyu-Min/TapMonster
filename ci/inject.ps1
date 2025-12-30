$ErrorActionPreference = "Stop"

function Invoke-RobocopySafe {
  param(
    [Parameter(Mandatory=$true)][string]$Source,
    [Parameter(Mandatory=$true)][string]$Dest
  )

  if (-not (Test-Path $Source)) {
    throw "[CI] robocopy source not found: $Source"
  }

  New-Item -ItemType Directory -Force -Path $Dest | Out-Null

  robocopy $Source $Dest /E /NFL /NDL /NJH /NJS | Out-Null

  $code = $LASTEXITCODE
  Write-Host "[CI] robocopy exit code = $code (0~7 = success, 8+ = failure)"

  if ($code -ge 8) {
    throw "[CI] robocopy failed with exit code $code"
  }
}

Write-Host "[CI] ===== Inject Firebase + DOTween (NO UNITY BUILD) ====="
Write-Host "[CI] WORKSPACE   = $env:WORKSPACE"
Write-Host "[CI] GS_JSON     = ****"
Write-Host "[CI] DOTWEEN_ZIP = ****"

if ([string]::IsNullOrWhiteSpace($env:WORKSPACE)) {
  throw "[CI] WORKSPACE env is empty. Jenkins workspace is not set."
}

if ([string]::IsNullOrWhiteSpace($env:GS_JSON) -or -not (Test-Path $env:GS_JSON)) {
  throw "[CI] GS_JSON missing or file not found. Check Jenkins credentials: firebase-google-services-json"
}

$firebaseDst = Join-Path $env:WORKSPACE "Assets\google-services.json"
New-Item -ItemType Directory -Force -Path (Split-Path $firebaseDst) | Out-Null
Copy-Item -Force $env:GS_JSON $firebaseDst
Write-Host "[CI] google-services.json injected -> $firebaseDst"

if ([string]::IsNullOrWhiteSpace($env:DOTWEEN_ZIP) -or -not (Test-Path $env:DOTWEEN_ZIP)) {
  throw "[CI] DOTWEEN_ZIP missing or file not found. Check Jenkins credentials: dotween-pro-zip"
}

$tmpDir = Join-Path $env:TEMP ("dotween_" + [Guid]::NewGuid().ToString("N"))
New-Item -ItemType Directory -Force -Path $tmpDir | Out-Null

try {
  Write-Host "[CI] Expanding DOTween zip -> $tmpDir"
  Expand-Archive -Path $env:DOTWEEN_ZIP -DestinationPath $tmpDir -Force

  $pluginsDir = Get-ChildItem -Path $tmpDir -Directory -Recurse -Filter "Plugins" -ErrorAction SilentlyContinue |
                Select-Object -First 1
  $assetsDir  = Get-ChildItem -Path $tmpDir -Directory -Recurse -Filter "Assets" -ErrorAction SilentlyContinue |
                Select-Object -First 1

  if ($pluginsDir) {
    Write-Host "[CI] Found Plugins folder at: $($pluginsDir.FullName)"

    $dstPlugins = Join-Path $env:WORKSPACE "Plugins"
    Invoke-RobocopySafe -Source $pluginsDir.FullName -Dest $dstPlugins

    $pluginsMetaCandidate = Join-Path $pluginsDir.Parent.FullName "Plugins.meta"
    if (Test-Path $pluginsMetaCandidate) {
      Copy-Item -Force $pluginsMetaCandidate (Join-Path $env:WORKSPACE "Plugins.meta")
      Write-Host "[CI] Copied Plugins.meta -> WORKSPACE"
    }
    else {
      $pluginsMeta = Get-ChildItem -Path $tmpDir -File -Recurse -Filter "Plugins.meta" -ErrorAction SilentlyContinue |
                     Select-Object -First 1
      if ($pluginsMeta) {
        Copy-Item -Force $pluginsMeta.FullName (Join-Path $env:WORKSPACE "Plugins.meta")
        Write-Host "[CI] Copied Plugins.meta (found via recurse) -> WORKSPACE"
      }
    }
  }
  elseif ($assetsDir) {
    Write-Host "[CI] Found Assets folder at: $($assetsDir.FullName)"

    $dstAssets = Join-Path $env:WORKSPACE "Assets"
    Invoke-RobocopySafe -Source $assetsDir.FullName -Dest $dstAssets
  }
  else {
    Write-Host "[CI] No Plugins/Assets folder found. Merging zip root -> WORKSPACE"
    Invoke-RobocopySafe -Source $tmpDir -Dest $env:WORKSPACE
  }

  $demigiant1 = Join-Path $env:WORKSPACE "Plugins\Demigiant"
  $demigiant2 = Join-Path $env:WORKSPACE "Assets\Demigiant"
  if (-not (Test-Path $demigiant1) -and -not (Test-Path $demigiant2)) {
    Write-Host "[CI] WARNING: Demigiant folder not detected at expected paths."
    Write-Host "[CI]          - $demigiant1"
    Write-Host "[CI]          - $demigiant2"
    Write-Host "[CI]          If build fails due to DOTween, check zip structure."
  }

  Write-Host "[CI] DOTween Pro injected successfully"
}
finally {
  if (Test-Path $tmpDir) { Remove-Item $tmpDir -Recurse -Force }
}

Write-Host "[CI] Inject stage finished successfully (no Unity build here)."
exit 0
