using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BootstrapUIPresenter : MonoBehaviour
{
    [SerializeField] private Button _startDownloadBtn;

    private UniTaskCompletionSource _downloadTcs;

    public void Init()
    {
        _startDownloadBtn.gameObject.SetActive(false);
        _startDownloadBtn.onClick.RemoveAllListeners();
        _startDownloadBtn.onClick.AddListener(OnClickStartDownload);
    }

    public UniTask WaitForDownloadClickAsync()
    {
        _downloadTcs = new UniTaskCompletionSource();
        _startDownloadBtn.gameObject.SetActive(true);
        return _downloadTcs.Task;
    }

    private void OnClickStartDownload()
    {
        _startDownloadBtn.gameObject.SetActive(false);
        _downloadTcs?.TrySetResult();
    }
}
