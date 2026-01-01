using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    private const LogCategory CurrentCategory = LogCategory.Bootstrap;

    [Header("모듈")]
    [SerializeField] private FirebaseLogInService _firebaseLogInService;

    [Header("재시도 정책")]
    [SerializeField] private int _maxRetry = 3;
    [SerializeField] private int _firebaseInitStepTimeOutMs = 5000;
    [SerializeField] private int _firebaseLogInStepTimeOutMs = 10000;
    [SerializeField] private int _retryDelayMs = 1000;


    private void Start()
    {
        BootstrapFlowAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }



    private async UniTask BootstrapFlowAsync(CancellationToken ct)
    {
        await RunStepWithRetry("Firebase 초기화", () => FirebaseManager.Instance.Initialize(ct), _firebaseInitStepTimeOutMs, ct);
        this.PrintLog("Firebase 초기화 완료", CurrentCategory, LogType.Log);

        var user = await RunStepWithRetry("Firebase 로그인", () => _firebaseLogInService.SignInAsync(ct), _firebaseLogInStepTimeOutMs, ct);
        this.PrintLog($"Firebase 로그인 완료: {user.UserId}", CurrentCategory, LogType.Log);
        // await 프로필 로드
        // await Pun2
    }


    private async UniTask RunStepWithRetry(string stepName, Func<UniTask> step, int stepTimeoutMs, CancellationToken ct)
    {
        for (int attempt = 1; attempt <= _maxRetry; attempt++)
        {
            try
            {
                this.PrintLog($"{stepName} 시도, {attempt} / {_maxRetry}", CurrentCategory, LogType.Log);

                await step().Timeout(TimeSpan.FromMilliseconds(stepTimeoutMs));
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

                return await step().Timeout(TimeSpan.FromMilliseconds(stepTimeoutMs));
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
}
