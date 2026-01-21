using Cysharp.Threading.Tasks;
using Firebase.Database;
using System.Collections.Generic;
using System.Threading;


public class SaveLoadService
{
    public async UniTask<SaveDataDTO> LoadAsync(string uid, CancellationToken ct)
    {
        string path = DBRoutes.SaveData(uid);
        DataSnapshot snapshot = await RTDBService.Instance.GetUniTaskAsync(path, ct);

        if (snapshot == null || !snapshot.Exists)
            return new SaveDataDTO().Normalized();

        SaveDataDTO dto = SnapshotParser.ParseSaveDataDTO(snapshot);
        return dto.Normalized();
    }

    // 전체 저장
    public async UniTask SaveFullAsync(string uid, Dictionary<string, object> updates, CancellationToken ct)
    {
        if (updates == null || updates.Count == 0) return;

        updates[DBRoutes.LastSavedAtUnixMs(uid)] = ServerValue.Timestamp;
        await RTDBService.Instance.UpdateUniTaskAsync(updates, ct);
    }

    // 부분 저장
    public async UniTask PatchAsync(string uid, Dictionary<string, object> updates, CancellationToken ct)
    {
        if (updates == null || updates.Count == 0) return;

        updates[DBRoutes.LastSavedAtUnixMs(uid)] = ServerValue.Timestamp;
        await RTDBService.Instance.UpdateUniTaskAsync(updates, ct);
    }
}
