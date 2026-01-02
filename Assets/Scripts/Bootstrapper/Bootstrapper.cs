using Cysharp.Threading.Tasks;
using Firebase.Auth;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    private const LogCategory CurrentCategory = LogCategory.Bootstrap;

    [Header("모듈")]
    [SerializeField] private FirebaseLogInService _firebaseLogInService;
    [SerializeField] private AddressablesRemoteService _addressablesRemoteService;
    [SerializeField] private BootstrapUIPresenter _bootstrapUIPresenter;
    [SerializeField] private NicknameSetter _nicknameSetter;

    [Header("재시도 정책")]
    [SerializeField] private int _maxRetry = 3;
    [SerializeField] private int _firebaseInitStepTimeOutMs = 5000;
    [SerializeField] private int _firebaseLogInStepTimeOutMs = 10000;
    [SerializeField] private int _retryDelayMs = 1000;

    [Header("테스트용")]
    [SerializeField] private GameObject _testImage;

    private void Start()
    {
        BootstrapFlowAsync(this.GetCancellationTokenOnDestroy())
            .Forget(e => this.PrintLog($"부트스트랩 예외 발생: {e}", CurrentCategory, LogType.Error)); ;
    }


    private async UniTask BootstrapFlowAsync(CancellationToken ct)
    {
        _bootstrapUIPresenter.Init();

        await InitFirebaseStepAsync(ct); this.PrintLog("Firebase 초기화 완료", CurrentCategory, LogType.Log);
        var user = await FirebaseLogInStepAsync(ct); this.PrintLog($"Firebase 로그인 완료: {user.UserId}", CurrentCategory, LogType.Log);
        // await 프로필 로드
        await DownloadAssetStepAsync(ct); this.PrintLog("다운로드 완료", CurrentCategory, LogType.Log);

        _testImage.SetActive(true); // 테스트용

        await EnsureNicknameStepAsync(FirebaseManager.Instance.Auth.CurrentUser.UserId, ct); this.PrintLog("닉네임 설정 완료", CurrentCategory, LogType.Log);

        // await Pun2

        SceneManager.LoadScene(1);
    }

    #region Private Step Methods


    private async UniTask InitFirebaseStepAsync(CancellationToken ct)
    {
        await RunStepWithRetry("Firebase 초기화", () => FirebaseManager.Instance.Initialize(ct), _firebaseInitStepTimeOutMs, ct);
    }

    private async UniTask<FirebaseUser> FirebaseLogInStepAsync(CancellationToken ct)
    {
        var user = await RunStepWithRetry("Firebase 로그인", () => _firebaseLogInService.SignInAsync(ct), _firebaseLogInStepTimeOutMs, ct);
        return user;
    }

    private async UniTask DownloadAssetStepAsync(CancellationToken ct)
    {
        string catalogUrl = await _addressablesRemoteService.GetCatalogUrlAsync(ct);
        await _addressablesRemoteService.InitializeAsync(ct);
        await _addressablesRemoteService.LoadRemoteCatalogAsync(catalogUrl, ct);

        bool isDownloadNeeded = await _addressablesRemoteService.GetNeedDownloadAsync(ct);
        if (!isDownloadNeeded) return;

        await _bootstrapUIPresenter.WaitForDownloadClickAsync(ct);
        await _addressablesRemoteService.DownloadAllAsync(ct);
    }

    private async UniTask EnsureNicknameStepAsync(string uid, CancellationToken ct)
    {
        bool isSet = await _nicknameSetter.IsSetNickname(uid, ct);
        if (isSet) return;

        _bootstrapUIPresenter.ShowNicknameUI();

        while (true)
        {
            _bootstrapUIPresenter.HideNicknameError();
            string nickname = await _bootstrapUIPresenter.WaitNicknameSubmitAsync(ct);

            // TODO: 길이, 금칙어
            if (string.IsNullOrWhiteSpace(nickname))
            {
                _bootstrapUIPresenter.ShowNicknameError(NicknameSetErrorCode.Empty);
                continue;
            }

            NicknameSetErrorCode result = await _nicknameSetter.TrySetNickname(nickname, ct);

            // 설정 성공할 때만 return
            if (result == NicknameSetErrorCode.Success)
            {
                await _nicknameSetter.SetUsersTreeAlso(nickname, ct);
                _bootstrapUIPresenter.HideNicknameUI();
                return;
            }

            _bootstrapUIPresenter.ShowNicknameError(result);
        }
    }

    #endregion


    #region Private Helper Methods

    private async UniTask RunStepWithRetry(string stepName, Func<UniTask> step, int stepTimeoutMs, CancellationToken ct)
    {
        for (int attempt = 1; attempt <= _maxRetry; attempt++)
        {
            try
            {
                this.PrintLog($"{stepName} 시도, {attempt} / {_maxRetry}", CurrentCategory, LogType.Log);

                await step().AttachExternalCancellation(ct).Timeout(TimeSpan.FromMilliseconds(stepTimeoutMs));
                return;
            }
            catch (TimeoutException) when (attempt < _maxRetry)
            {
                this.PrintLog($"{stepName} 타임아웃, {attempt}/{_maxRetry}", CurrentCategory, LogType.Warning);
            }
            catch (Exception) when (attempt < _maxRetry)
            {
                this.PrintLog($"{stepName} 실패. {attempt}/{_maxRetry}", CurrentCategory, LogType.Warning);
            }

            await UniTask.Delay(_retryDelayMs, cancellationToken: ct);
        }

        // 최종 실패
        throw new Exception($"{stepName} 실패");
    }

    private async UniTask<T> RunStepWithRetry<T>(string stepName, Func<UniTask<T>> step, int stepTimeoutMs, CancellationToken ct)
    {
        for (int attempt = 1; attempt <= _maxRetry; attempt++)
        {
            try
            {
                this.PrintLog($"{stepName} 시도, {attempt} / {_maxRetry}", CurrentCategory, LogType.Log);

                return await step().AttachExternalCancellation(ct).Timeout(TimeSpan.FromMilliseconds(stepTimeoutMs));
            }
            catch (TimeoutException) when (attempt < _maxRetry)
            {
                this.PrintLog($"{stepName} 타임아웃, {attempt}/{_maxRetry}", CurrentCategory, LogType.Warning);
            }
            catch (Exception) when (attempt < _maxRetry)
            {
                this.PrintLog($"{stepName} 실패. {attempt}/{_maxRetry}", CurrentCategory, LogType.Warning);
            }

            await UniTask.Delay(_retryDelayMs, cancellationToken: ct);
        }

        // 최종 실패
        throw new Exception($"{stepName} 실패");
    }

    #endregion
}
