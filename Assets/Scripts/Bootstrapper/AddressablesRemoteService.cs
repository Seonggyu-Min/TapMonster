using Cysharp.Threading.Tasks;
using Firebase.Database;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;


public class AddressablesRemoteService : MonoBehaviour
{
    [SerializeField] private AddressablesLableSO _labelSO;

    private bool _isInitialized = false;
    private string _loadedCatalogUrl;

    private LogCategory _currentCategory = LogCategory.Addressables;


    public async UniTask<string> GetCatalogUrlAsync(CancellationToken ct)
    {
        DataSnapshot urlSnapshot = await RTDBService.Instance.GetUniTaskAsync(DBRoutes.AddressablesCatalogJsonUrl, ct);

        if (!urlSnapshot.Exists)
        {
            throw new InvalidOperationException($"카탈로그 URL 스냅샷이 비어있습니다: {DBRoutes.AddressablesCatalogJsonUrl}");
        }

        if (urlSnapshot.Value is not string url || string.IsNullOrEmpty(url))
        {
            throw new InvalidOperationException($"카탈로그 URL이 정상적이지 않습니다: {DBRoutes.AddressablesCatalogJsonUrl}");
        }

        return url;
    }

    public async UniTask InitializeAsync(CancellationToken ct)
    {
        if (_isInitialized)
        {
            this.PrintLog("이미 초기화되어 return", _currentCategory, LogType.Warning);
            return;
        }

        AsyncOperationHandle init = Addressables.InitializeAsync();
        await init.ToUniTask().AttachExternalCancellation(ct);

        if (init.Status == AsyncOperationStatus.Succeeded)
        {
            _isInitialized = true;
        }
        else
        {
            throw new Exception($"어드레서블 초기화 실패, {init.Status}");
        }
    }

    public async UniTask LoadRemoteCatalogAsync(string catalogJsonUrl, CancellationToken ct)
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException("어드레서블을 먼저 초기화해야 합니다. InitializeAsync 메서드를 먼저 호출하세요.");
        }
        if (string.IsNullOrEmpty(catalogJsonUrl))
        {
            throw new ArgumentNullException(nameof(catalogJsonUrl));
        }

        // 같은 카탈로그면 스킵
        if (_loadedCatalogUrl == catalogJsonUrl)
        {
            return;
        }

        AsyncOperationHandle<IResourceLocator> handle = Addressables.LoadContentCatalogAsync(catalogJsonUrl, true);
        IResourceLocator locator = await handle.ToUniTask().AttachExternalCancellation(ct);

        if (handle.Status != AsyncOperationStatus.Succeeded || locator == null)
        {
            throw new Exception($"LoadContentCatalogAsync 실패. url={catalogJsonUrl}");
        }

        _loadedCatalogUrl = catalogJsonUrl;
        this.PrintLog($"Remote catalog 로드 완료: {catalogJsonUrl}", _currentCategory, LogType.Log);
    }

    public async UniTask<bool> GetNeedDownloadAsync(CancellationToken ct)
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException("어드레서블을 먼저 초기화해야 합니다. InitializeAsync 메서드를 먼저 호출하세요.");
        }

        if (_labelSO == null || _labelSO.Labels == null || _labelSO.Labels.Count == 0)
        {
            this.PrintLog("라벨이 null이거나 비어있습니다.", _currentCategory, LogType.Warning);
            return false;
        }

        // 카탈로그 업데이트 확인
        var checkHandle = Addressables.CheckForCatalogUpdates(false);
        var catalogs = await checkHandle.ToUniTask(cancellationToken: ct);
        Addressables.Release(checkHandle);

        if (catalogs != null && catalogs.Count > 0)
        {
            // 카탈로그 업데이트 적용
            var updateHandle = Addressables.UpdateCatalogs(catalogs, false);
            await updateHandle.ToUniTask(cancellationToken: ct);
            Addressables.Release(updateHandle);
        }

        // 다운로드 크기 계산
        long totalSize = 0;

        foreach (string label in _labelSO.Labels)
        {
            var sizeHandle = Addressables.GetDownloadSizeAsync(label);
            long size = await sizeHandle.ToUniTask(cancellationToken: ct);
            Addressables.Release(sizeHandle);

            totalSize += size;
        }

        return totalSize > 0;
    }

    public async UniTask DownloadAllAsync(CancellationToken ct)
    {
        if (_labelSO == null || _labelSO.Labels == null || _labelSO.Labels.Count == 0)
        {
            throw new InvalidOperationException("_labelSO가 비어있거나 SO에 리스트가 비어있습니다.");
        }

        foreach (var label in _labelSO.Labels)
        {
            await DownloadLabelAsync(label, ct);
        }
    }

    public async UniTask DownloadLabelAsync(string label, CancellationToken ct)
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException("어드레서블을 먼저 초기화해야 합니다. InitializeAsync 메서드를 먼저 호출하세요.");
        }

        AsyncOperationHandle download = Addressables.DownloadDependenciesAsync(label, true);
        await download.ToUniTask().AttachExternalCancellation(ct);

        if (download.Status != AsyncOperationStatus.Succeeded)
        {
            throw new Exception($"DownloadDependenciesAsync 실패. label={label}");
        }

        this.PrintLog($"label[{label}] 다운로드 완료", _currentCategory, LogType.Log);
    }

    public async UniTask<T> LoadAssetAsync<T>(string address, CancellationToken ct) where T : class
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException("어드레서블을 먼저 초기화해야 합니다. InitializeAsync 메서드를 먼저 호출하세요.");
        }

        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(address);
        T asset = await handle.ToUniTask().AttachExternalCancellation(ct);

        if (handle.Status != AsyncOperationStatus.Succeeded || asset == null)
        {
            throw new Exception($"LoadAssetAsync 실패. address={address}");
        }

        this.PrintLog($"address[{address}] 다운로드 완료", _currentCategory, LogType.Log);
        return asset;
    }
}
