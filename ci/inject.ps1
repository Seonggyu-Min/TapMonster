$ErrorActionPreference = "Stop"

Copy-Item "$env:GS_JSON" "Assets\google-services.json" -Force
Write-Host "[CI] google-services.json injected"

Write-Host "[CI] Injecting DOTween Pro..."
$zipPath = "$env:DOTWEEN_ZIP"
$tempRoot = Join-Path $env:WORKSPACE "_temp_dotween"

if (Test-Path $tempRoot) { Remove-Item $tempRoot -Recurse -Force }
New-Item -ItemType Directory -Force -Path $tempRoot | Out-Null

Expand-Archive -Path $zipPath -DestinationPath $tempRoot -Force

$demigiantDir = Get-ChildItem -Path $tempRoot -Directory -Recurse -Force |
                Where-Object { $_.Name -eq "Demigiant" } |
                Select-Object -First 1
if ($null -eq $demigiantDir) { throw "Demigiant folder not found" }

$demigiantMetaPath = Join-Path $demigiantDir.Parent.FullName "Demigiant.meta"
if (!(Test-Path $demigiantMetaPath)) {
  $meta = Get-ChildItem -Path $tempRoot -File -Recurse -Force |
          Where-Object { $_.Name -eq "Demigiant.meta" } |
          Select-Object -First 1
  if ($null -eq $meta) { throw "Demigiant.meta not found" }
  $demigiantMetaPath = $meta.FullName
}

$destPluginsDir = "Assets\Plugins"
$destDemigiantDir = "Assets\Plugins\Demigiant"
$destDemigiantMeta = "Assets\Plugins\Demigiant.meta"

if (!(Test-Path $destPluginsDir)) { New-Item -ItemType Directory -Force -Path $destPluginsDir | Out-Null }
if (Test-Path $destDemigiantDir) { Remove-Item $destDemigiantDir -Recurse -Force }

Copy-Item -Recurse -Force $demigiantDir.FullName $destDemigiantDir
Copy-Item -Force $demigiantMetaPath $destDemigiantMeta

Write-Host "[CI] DOTween Pro injected successfully"
