node {
  stage('Checkout') { checkout scm }

  stage('Inject Firebase + DOTween') {
    withCredentials([
      file(credentialsId: 'firebase-google-services-json', variable: 'GS_JSON'),
      file(credentialsId: 'dotween-pro-zip', variable: 'DOTWEEN_ZIP')
    ]) {
      powershell 'ci\\inject.ps1'
    }
  }

  stage('Build Android (APK)') {
    withCredentials([
      file(credentialsId: 'android-keystore-file', variable: 'ANDROID_KEYSTORE'),
      string(credentialsId: 'android-keystore-pass', variable: 'ANDROID_KEYSTORE_PASS'),
      string(credentialsId: 'android-keyalias-name', variable: 'ANDROID_KEYALIAS_NAME'),
      string(credentialsId: 'android-keyalias-pass', variable: 'ANDROID_KEYALIAS_PASS')
    ]) {
      powershell 'ci\\build_android.ps1'
    }
  }
}
