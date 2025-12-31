using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    private const LogCategory CurrentCategory = LogCategory.Bootstrap;

    [SerializeField] private int _maxRetry = 3;
    [SerializeField] private int _retryDelayMs = 1000;


    private void Start()
    {
        BootstrapFlowAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }



    private async UniTask BootstrapFlowAsync(CancellationToken ct)
    {
        await RunStepWithRetry("Firebase 초기화", () => FirebaseManager.Instance.Initialize(ct), ct);
        // await 로그인 기능
        // await 프로필 로드
        // await Pun2
    }


    private async UniTask RunStepWithRetry(string stepName, Func<UniTask> step, CancellationToken ct)
    {
        for (int attempt = 1; attempt <= _maxRetry; attempt++)
        {
            try
            {
                this.PrintLog($"{stepName} 시도, {attempt} / {_maxRetry}", CurrentCategory, LogType.Log);

                await step().Timeout(TimeSpan.FromMilliseconds(_retryDelayMs));
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
}
