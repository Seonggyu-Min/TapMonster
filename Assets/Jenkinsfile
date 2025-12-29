stage('Inject Firebase + DOTween') {
  steps {
    withCredentials([
      file(credentialsId: 'firebase-google-services-json', variable: 'GS_JSON'),
      file(credentialsId: 'dotween-pro-zip', variable: 'DOTWEEN_ZIP')
    ]) {
      powershell '''
        Copy-Item "$env:GS_JSON" "Assets\\google-services.json" -Force

        $zipPath = "$env:DOTWEEN_ZIP"
        $tempRoot = Join-Path $env:WORKSPACE "_temp_dotween"
        $sourceDir = Join-Path $tempRoot "Plugins\\Demigiant"
        $destDir = "Assets\\Plugins\\Demigiant"

        if (Test-Path $tempRoot) { Remove-Item $tempRoot -Recurse -Force }
        New-Item -ItemType Directory -Force -Path $tempRoot | Out-Null

        Expand-Archive -Path $zipPath -DestinationPath $tempRoot -Force

        if (!(Test-Path $sourceDir)) {
          Write-Error "[CI] DOTween source not found: $sourceDir"
          exit 1
        }

        if (Test-Path $destDir) { Remove-Item $destDir -Recurse -Force }
        New-Item -ItemType Directory -Force -Path (Split-Path $destDir) | Out-Null

        Copy-Item -Recurse -Force $sourceDir $destDir
      '''
    }
  }
}
