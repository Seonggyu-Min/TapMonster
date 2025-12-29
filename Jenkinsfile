node {
  stage('Checkout') {
    checkout scm
  }

  stage('Inject Firebase + DOTween') {
    withCredentials([
      file(credentialsId: 'firebase-google-services-json', variable: 'GS_JSON'),
      file(credentialsId: 'dotween-pro-zip', variable: 'DOTWEEN_ZIP')
    ]) {
      powershell '''
        $ErrorActionPreference = "Stop"

        Copy-Item "$env:GS_JSON" "Assets\\google-services.json" -Force
        Write-Host "[CI] google-services.json injected"

        Write-Host "[CI] Injecting DOTween Pro..."

        $zipPath = "$env:DOTWEEN_ZIP"
        $tempRoot = Join-Path $env:WORKSPACE "_temp_dotween"

        if (!(Test-Path $zipPath)) {
          Write-Error "[CI] DOTWEEN_ZIP not found at: $zipPath"
          exit 1
        }
        $zipInfo = Get-Item $zipPath
        Write-Host ("[CI] DOTWEEN_ZIP path: " + $zipInfo.FullName)
        Write-Host ("[CI] DOTWEEN_ZIP size: " + $zipInfo.Length + " bytes")

        if (Test-Path $tempRoot) { Remove-Item $tempRoot -Recurse -Force }
        New-Item -ItemType Directory -Force -Path $tempRoot | Out-Null

        try {
          Expand-Archive -Path $zipPath -DestinationPath $tempRoot -Force
          Write-Host "[CI] Expand-Archive success"
        }
        catch {
          Write-Warning "[CI] Expand-Archive failed, fallback to ZipFile::ExtractToDirectory"
          Add-Type -AssemblyName System.IO.Compression.FileSystem
          [System.IO.Compression.ZipFile]::ExtractToDirectory($zipPath, $tempRoot, $true)
          Write-Host "[CI] ZipFile Extract success"
        }

        $rootItems = Get-ChildItem -Path $tempRoot -Force
        Write-Host "[CI] Extracted root items:"
        $rootItems | ForEach-Object { Write-Host (" - " + $_.FullName) }

        if ($rootItems.Count -eq 0) {
          Write-Error "[CI] Extraction produced no files. The zip might be wrong/corrupted."
          exit 1
        }

        $demigiantDir = Get-ChildItem -Path $tempRoot -Directory -Recurse -Force |
                        Where-Object { $_.Name -eq "Demigiant" } |
                        Select-Object -First 1

        if ($null -eq $demigiantDir) {
          Write-Host "[CI] Could not find Demigiant. Showing directory tree (depth ~3):"
          Get-ChildItem -Path $tempRoot -Recurse -Force | Select-Object -First 200 FullName | ForEach-Object { Write-Host $_.FullName }
          Write-Error "[CI] Demigiant folder not found anywhere under extracted zip."
          exit 1
        }

        Write-Host ("[CI] Found Demigiant at: " + $demigiantDir.FullName)

        $demigiantMetaPath = Join-Path $demigiantDir.Parent.FullName "Demigiant.meta"
        if (!(Test-Path $demigiantMetaPath)) {
          Write-Warning ("[CI] Demigiant.meta not found next to folder. Searched: " + $demigiantMetaPath)
          $meta = Get-ChildItem -Path $tempRoot -File -Recurse -Force |
                  Where-Object { $_.Name -eq "Demigiant.meta" } |
                  Select-Object -First 1
          if ($null -eq $meta) {
            Write-Error "[CI] Demigiant.meta not found anywhere. Please include meta in zip."
            exit 1
          }
          $demigiantMetaPath = $meta.FullName
          Write-Host ("[CI] Found Demigiant.meta at: " + $demigiantMetaPath)
        }

        $destPluginsDir = "Assets\\Plugins"
        $destDemigiantDir = "Assets\\Plugins\\Demigiant"
        $destDemigiantMeta = "Assets\\Plugins\\Demigiant.meta"

        if (!(Test-Path $destPluginsDir)) { New-Item -ItemType Directory -Force -Path $destPluginsDir | Out-Null }

        if (Test-Path $destDemigiantDir) { Remove-Item $destDemigiantDir -Recurse -Force }

        Copy-Item -Recurse -Force $demigiantDir.FullName $destDemigiantDir
        Copy-Item -Force $demigiantMetaPath $destDemigiantMeta

        Write-Host "[CI] DOTween Pro injected successfully"
      '''
    }
  }

  stage('Done') {
    echo 'Inject stage finished'
  }
}
