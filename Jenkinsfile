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

        if (Test-Path $tempRoot) { Remove-Item $tempRoot -Recurse -Force }
        New-Item -ItemType Directory -Force -Path $tempRoot | Out-Null

        Expand-Archive -Path $zipPath -DestinationPath $tempRoot -Force

        $rootItems = Get-ChildItem -Path $tempRoot -Force
        Write-Host "[CI] Extracted root items:"
        $rootItems | ForEach-Object { Write-Host (" - " + $_.FullName) }

        $demigiantDir = Get-ChildItem -Path $tempRoot -Directory -Recurse -Force |
                        Where-Object { $_.Name -eq "Demigiant" } |
                        Select-Object -First 1

        if ($null -eq $demigiantDir) {
          Write-Error "[CI] Demigiant folder not found"
          exit 1
        }

        $demigiantMetaPath = Join-Path $demigiantDir.Parent.FullName "Demigiant.meta"
        if (!(Test-Path $demigiantMetaPath)) {
          $meta = Get-ChildItem -Path $tempRoot -File -Recurse -Force |
                  Where-Object { $_.Name -eq "Demigiant.meta" } |
                  Select-Object -First 1
          if ($null -eq $meta) { Write-Error "[CI] Demigiant.meta not found"; exit 1 }
          $demigiantMetaPath = $meta.FullName
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

  stage('Build Android (APK)') {
    withCredentials([
      file(credentialsId: 'android-keystore-file', variable: 'ANDROID_KEYSTORE'),
      string(credentialsId: 'android-keystore-pass', variable: 'ANDROID_KEYSTORE_PASS'),
      string(credentialsId: 'android-keyalias-pass', variable: 'ANDROID_KEYALIAS_PASS')
    ]) {
      powershell '''
        $ErrorActionPreference = "Stop"

        $unityExe = "C:\\Program Files\\Unity\\Hub\\Editor\\2022.3.62f2\\Editor\\Unity.exe"
        $logPath = Join-Path $env:WORKSPACE "Build\\unity_build.log"
        $buildDir = Join-Path $env:WORKSPACE "Build\\Android"

        if (Test-Path $logPath) { Remove-Item $logPath -Force }

        $args = @(
          "-batchmode", "-nographics", "-quit",
          "-projectPath", $env:WORKSPACE,
          "-executeMethod", "BuildScript.BuildAndroidApk",
          "-workspace", $env:WORKSPACE,
          "-logFile", $logPath
        )

        $proc = Start-Process -FilePath $unityExe -ArgumentList $args -NoNewWindow -Wait -PassThru
        $exitCode = $proc.ExitCode
        Write-Host "[CI] Unity process exit code: $exitCode"

        $logText = ""
        if (Test-Path $logPath) { $logText = Get-Content $logPath -Raw }

        $hasSuccessMarker = $logText -match "\[CI\] Build succeeded"
        $hasBuildException = $logText -match "\[CI\] Build exception"

        $apk = Get-ChildItem -Path $buildDir -Filter *.apk -ErrorAction SilentlyContinue | Select-Object -First 1
        $hasApk = $null -ne $apk

        if ($hasBuildException) {
          Write-Error "[CI] BuildScript reported exception. Failing."
          exit 1
        }

        if (($exitCode -ne 0) -and -not ($hasSuccessMarker -and $hasApk)) {
          Write-Error "[CI] Unity failed (exit=$exitCode). No success marker or apk missing."
          exit $exitCode
        }

        Write-Host "[CI] Unity build considered SUCCESS."
        if ($hasApk) { Write-Host "[CI] APK: $($apk.FullName)" }

      '''
    }
  }
}
