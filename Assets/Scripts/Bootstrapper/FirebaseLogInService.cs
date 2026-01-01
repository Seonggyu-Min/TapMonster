using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Google;
using System;
using System.Threading;
using UnityEngine;

public class FirebaseLogInService : MonoBehaviour
{
    [Header("에디터 로그인: 이메일 로그인")]
    [SerializeField] private string _email;
    [SerializeField] private string _password;

    private const string WebClientId = "1060578439326-0mbhma8b723jga0d8ndhsk2qv50go5tt.apps.googleusercontent.com";

    private const LogCategory CurrentCategory = LogCategory.Firebase;


    private void Awake()
    {
        MakeConfiguration();
    }


    public async UniTask<FirebaseUser> SignInAsync(CancellationToken ct)
    {
#if UNITY_EDITOR
        return await SignInOrCreateWithEmailAsync(_email, _password, ct);
#endif
        return await SignInWithGoogleAsync(ct);
    }



    private async UniTask<FirebaseUser> SignInOrCreateWithEmailAsync(
        string email, string password, CancellationToken ct)
    {

#if !UNITY_EDITOR
    throw new InvalidOperationException("이메일 로그인은 에디터에서만 사용합니다.");
#else
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("email/password가 비어있음");
        }

        try
        {
            this.PrintLog("이메일 로그인 시도", CurrentCategory);

            AuthResult signInResult = await FirebaseManager.Instance.Auth.SignInWithEmailAndPasswordAsync(email, password)
                            .AsUniTask()
                            .AttachExternalCancellation(ct);

            return signInResult.User;
        }
        catch (FirebaseException e)
        {
            this.PrintLog($"이메일 로그인 중 FirebaseException 진입, {e}", CurrentCategory, LogType.Log);
            AuthError error = (AuthError)e.ErrorCode;

            this.PrintLog("이메일 로그인 회원가입 진입", CurrentCategory);

            AuthResult created = await FirebaseManager.Instance.Auth.CreateUserWithEmailAndPasswordAsync(email, password)
                .AsUniTask()
                .AttachExternalCancellation(ct);

            return created.User;
        }
#endif
    }

    private async UniTask<FirebaseUser> SignInWithGoogleAsync(CancellationToken ct)
    {
        // 1. 구글 로그인 UI
        GoogleSignInUser gUser = await GoogleSignIn.DefaultInstance
            .SignIn()
            .AsUniTask()
            .AttachExternalCancellation(ct);

        if (string.IsNullOrEmpty(gUser.IdToken))
            throw new Exception("GoogleSignInUser.IdToken이 비어있음 (WebClientId/RequestIdToken 설정 확인)");

        // 2. Firebase에서 Credential 받기
        Credential credential = GoogleAuthProvider.GetCredential(gUser.IdToken, null);

        // 3. Firebase 로그인
        FirebaseUser user = await FirebaseManager.Instance.Auth.SignInWithCredentialAsync(credential)
            .AsUniTask()
            .AttachExternalCancellation(ct);

        return user;
    }


    private void MakeConfiguration()
    {
        GoogleSignIn.Configuration = new()
        {
            WebClientId = WebClientId,
            RequestIdToken = true,
            RequestEmail = false,
            UseGameSignIn = false
        };
    }
}