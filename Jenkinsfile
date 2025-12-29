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
        Copy-Item "$env:GS_JSON" "Assets\\google-services.json" -Force

        Write-Host "Injecting DOTween Pro..."

        $zipPath = "$env:DOTWEEN_ZIP"
        $tempRoot = Join-Path $env:WORKSPACE "_temp_dotween"
        $destPluginsDir = "Assets\\Plugins"
        $destDemigiantDir = "Assets\\Plugins\\Demigiant"
        $destDemigiantMeta = "Assets\\Plugins\\Demigiant.meta"

        if (Test-Path $tempRoot) { Remove-Item $tempRoot -Recurse -Force }
        New-Item -ItemType Directory -Force -Path $tempRoot | Out-Null

        Expand-Archive -Path $zipPath -DestinationPath $tempRoot -Force

        $pluginsDir = Get-ChildItem -Path $tempRoot -Directory -Recurse -Force |
                      Where-Object { $_.Name -eq "Plugins" } |
                      Select-Object -First 1

        if ($null -eq $pluginsDir) {
          Write-Error "[CI] Plugins folder not found"
          exit 1
        }

        $sourceDemigiantDir = Join-Path $pluginsDir.FullName "Demigiant"
        $sourceDemigiantMeta = Join-Path $pluginsDir.FullName "Demigiant.meta"

        if (!(Test-Path $sourceDemigiantDir)) {
          Write-Error "[CI] Demigiant folder not found"
          exit 1
        }
        if (!(Test-Path $sourceDemigiantMeta)) {
          Write-Error "[CI] Demigiant.meta not found"
          exit 1
        }

        if (!(Test-Path $destPluginsDir)) {
          New-Item -ItemType Directory -Force -Path $destPluginsDir | Out-Null
        }

        if (Test-Path $destDemigiantDir) { Remove-Item $destDemigiantDir -Recurse -Force }

        Copy-Item -Recurse -Force $sourceDemigiantDir $destDemigiantDir
        Copy-Item -Force $sourceDemigiantMeta $destDemigiantMeta

        Write-Host "DOTween Pro injected successfully"
      '''
    }
  }
}
