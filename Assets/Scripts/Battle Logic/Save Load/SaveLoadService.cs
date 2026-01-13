using Cysharp.Threading.Tasks;
using Firebase.Database;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SaveLoadService
{
    #region Field

    private const LogCategory CurrentCategory = LogCategory.Firebase;

    #endregion


    #region Public Methods

    public async UniTask<SaveDataDTO> LoadAsync(string uid, CancellationToken ct)
    {
        string path = DBRoutes.SaveData(uid);

        DataSnapshot snapshot = await RTDBService.Instance.GetUniTaskAsync(path, ct);

        if (snapshot == null || !snapshot.Exists)
            return new SaveDataDTO().Normalized();

        SaveDataDTO dto = SnapshotParser.ParseSaveDataDTO(snapshot);
        return dto.Normalized();
    }

    /// <summary>
    /// 전체 저장
    /// </summary>
    public async UniTask SaveAsync(string uid, SaveDataDTO dto, CancellationToken ct)
    {
        dto = dto.Normalized();

        Dictionary<string, object> updates = BuildFullSaveUpdates(uid, dto);

        await RTDBService.Instance.UpdateUniTaskAsync(updates, ct);
    }

    /// <summary>
    /// 부분 저장: 외부에서 updates를 만들어 넘겨 UpdateChildren
    /// </summary>
    public async UniTask PatchAsync(string uid, Dictionary<string, object> updates, CancellationToken ct)
    {
        if (updates == null || updates.Count == 0) return;

        // 저장 시각은 서버 기준으로 항상 갱신
        updates[DBRoutes.LastSavedAtUnixMs(uid)] = ServerValue.Timestamp;

        await RTDBService.Instance.UpdateUniTaskAsync(updates, ct);
    }

    #endregion


    #region Private Methods

    private static Dictionary<string, object> BuildFullSaveUpdates(string uid, SaveDataDTO dto)
    {
        var updates = new Dictionary<string, object>(128);

        // Server time
        updates[DBRoutes.LastSavedAtUnixMs(uid)] = ServerValue.Timestamp;

        // Stage
        updates[DBRoutes.CurrentStage(uid)] = dto.StageDTO.CurrentStage;

        // Wallet
        if (dto.WalletDTO?.Currencies != null &&
            dto.WalletDTO.Currencies.TryGetValue(DatabaseKeys.Gold, out BigNumberDTO goldDto) &&
            goldDto != null)
        {
            updates[DBRoutes.GoldMantissa(uid)] = goldDto.Mantissa;
            updates[DBRoutes.GoldExponent(uid)] = goldDto.Exponent;
        }
        else
        {
            // 없으면 0
            updates[DBRoutes.GoldMantissa(uid)] = 0d;
            updates[DBRoutes.GoldExponent(uid)] = 0;
        }

        // Levels
        WriteLevels(updates, DBRoutes.RelicLevels(uid), dto.RelicLevels);
        WriteLevels(updates, DBRoutes.UpgradeLevels(uid), dto.UpgradeLevels);
        WriteLevels(updates, DBRoutes.SkillLevels(uid), dto.SkillLevels);

        return updates;
    }

    private static void WriteLevels(Dictionary<string, object> updates, string basePath, Dictionary<string, int> levels)
    {
        if (levels == null) return;

        foreach (var kv in levels)
        {
            updates[DBPathMaker.Join(basePath, kv.Key)] = Mathf.Max(0, kv.Value);
        }
    }

    #endregion
}
