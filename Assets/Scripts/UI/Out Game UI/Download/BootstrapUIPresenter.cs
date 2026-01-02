using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class BootstrapUIPresenter : MonoBehaviour
{
    [SerializeField] private Button _startDownloadBtn;
    [SerializeField] private NicknameSetUpView _nicknameSetUpView;
    [SerializeField] private Button _nicknameSubmitBtn;

    [SerializeField] private NicknameSetter _nicknameSetter;


    private UniTaskCompletionSource _downloadTcs;
    private UniTaskCompletionSource<string> _nicknameSubmitTcs;

    public void Init()
    {
        _startDownloadBtn.gameObject.SetActive(false);
        _startDownloadBtn.onClick.RemoveAllListeners();
        _startDownloadBtn.onClick.AddListener(OnClickStartDownload);

        _nicknameSetUpView.gameObject.SetActive(false);
        _nicknameSubmitBtn.onClick.RemoveAllListeners();
    }

    public UniTask WaitForDownloadClickAsync(CancellationToken ct)
    {
        if (_downloadTcs != null)
        {
            return _downloadTcs.Task.AttachExternalCancellation(ct);
        }

        _downloadTcs = new UniTaskCompletionSource();

        _startDownloadBtn.gameObject.SetActive(true);

        ct.Register(() =>
        {
            _downloadTcs?.TrySetCanceled();
            _downloadTcs = null;
        });

        return _downloadTcs.Task;
    }

    private void OnClickStartDownload()
    {
        _startDownloadBtn.gameObject.SetActive(false);
        _downloadTcs?.TrySetResult();
        _downloadTcs = null;
    }


    public void ShowNicknameUI()
    {
        _nicknameSetUpView.gameObject.SetActive(true);
    }

    public void HideNicknameUI()
    {
        _nicknameSetUpView.gameObject.SetActive(false);
    }

    public UniTask<string> WaitNicknameSubmitAsync(CancellationToken ct)
    {
        if (_nicknameSubmitTcs != null)
        {
            return _nicknameSubmitTcs.Task.AttachExternalCancellation(ct);
        }
        _nicknameSubmitTcs = new();

        _nicknameSubmitBtn.onClick.RemoveAllListeners();
        _nicknameSubmitBtn.onClick.AddListener(() =>
        {
            _nicknameSubmitTcs.TrySetResult(_nicknameSetUpView.NicknameText);
            _nicknameSubmitTcs = null;
        });
        ct.Register(() =>
        {
            _nicknameSubmitTcs?.TrySetCanceled();
            _nicknameSubmitTcs = null;
        });

        return _nicknameSubmitTcs.Task;
    }

    public void ShowNicknameError(NicknameSetErrorCode code)
    {
        // TODO: 에러코드 매핑
        _nicknameSetUpView.ShowError(code.ToString());
    }

    public void HideNicknameError()
    {
        _nicknameSetUpView.HideError();
    }
}
