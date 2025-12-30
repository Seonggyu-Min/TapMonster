$ErrorActionPreference = "Stop"

Write-Host "[CI] ===== Inject Firebase + DOTween (NO UNITY BUILD) ====="
Write-Host "[CI] WORKSPACE   = $env:WORKSPACE"
Write-Host "[CI] GS_JSON     = $env:GS_JSON"
Write-Host "[CI] DOTWEEN_ZIP = $env:DOTWEEN_ZIP"

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

  $pluginsInZip = Join-Path $tmpDir "Plugins"

  $assetsInZip  = Join-Path $tmpDir "Assets"

  if (Test-Path $pluginsInZip) {
    $dstPlugins = Join-Path $env:WORKSPACE "Plugins"
    New-Item -ItemType Directory -Force -Path $dstPlugins | Out-Null

    Write-Host "[CI] Detected Plugins/ in zip. Merging -> $dstPlugins"
    robocopy $pluginsInZip $dstPlugins /E /NFL /NDL /NJH /NJS | Out-Null

    $pluginsMetaInZip = Join-Path $tmpDir "Plugins.meta"
    if (Test-Path $pluginsMetaInZip) {
      Copy-Item -Force $pluginsMetaInZip (Join-Path $env:WORKSPACE "Plugins.meta")
      Write-Host "[CI] Copied Plugins.meta -> WORKSPACE"
    }
  }
  elseif (Test-Path $assetsInZip) {
    $dstAssets = Join-Path $env:WORKSPACE "Assets"
    New-Item -ItemType Directory -Force -Path $dstAssets | Out-Null

    Write-Host "[CI] Detected Assets/ in zip. Merging -> $dstAssets"
    robocopy $assetsInZip $dstAssets /E /NFL /NDL /NJH /NJS | Out-Null
  }
  else {
    Write-Host "[CI] No Plugins/ or Assets/ found. Merging zip root -> WORKSPACE"
    robocopy $tmpDir $env:WORKSPACE /E /NFL /NDL /NJH /NJS | Out-Null
  }

  Write-Host "[CI] DOTween Pro injected successfully"
}
finally {
  if (Test-Path $tmpDir) { Remove-Item $tmpDir -Recurse -Force }
}

Write-Host "[CI] Inject stage finished successfully (no Unity build here)."
