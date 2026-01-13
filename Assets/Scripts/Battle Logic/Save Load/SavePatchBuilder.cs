using System;
using System.Collections.Generic;
using UnityEngine;


[Flags]
public enum SaveDirtyFlags
{
    None = 0,
    Stage = 1 << 0,
    Relic = 1 << 1,
    Upgrade = 1 << 2,
    Skill = 1 << 3,
    Wallet = 1 << 4,
    All = Stage | Relic | Upgrade | Skill | Wallet
}


public class SavePatchBuilder
{
    public Dictionary<string, object> BuildPatch(string uid, SaveDirtyFlags dirty, GameStateModel gameState)
    {
        var updates = new Dictionary<string, object>(128);

        if ((dirty & SaveDirtyFlags.Stage) != 0)
            WriteStage(updates, uid, gameState);

        if ((dirty & SaveDirtyFlags.Wallet) != 0)
            WriteWalletCurrentGold(updates, uid, gameState);

        if ((dirty & SaveDirtyFlags.Relic) != 0)
            WriteLevels(updates, DBRoutes.RelicLevels(uid), gameState.RelicModel.RelicLevels);

        if ((dirty & SaveDirtyFlags.Upgrade) != 0)
            WriteLevels(updates, DBRoutes.UpgradeLevels(uid), gameState.UpgradeModel.UpgradeLevels);

        if ((dirty & SaveDirtyFlags.Skill) != 0)
            WriteLevels(updates, DBRoutes.SkillLevels(uid), gameState.SkillModel.SkillLevels);

        return updates;
    }

    private static void WriteStage(Dictionary<string, object> updates, string uid, GameStateModel gs)
    {
        updates[DBRoutes.CurrentStage(uid)] = gs.StageModel.CurrentStage;
    }

    private static void WriteWalletCurrentGold(Dictionary<string, object> updates, string uid, GameStateModel gs)
    {
        BigNumber gold = gs.WalletModel.Get(CurrencyId.Gold);

        updates[DBRoutes.GoldMantissa(uid)] = gold.Mantissa;
        updates[DBRoutes.GoldExponent(uid)] = gold.Exponent;
    }

    private static void WriteLevels(Dictionary<string, object> updates, string basePath, IReadOnlyDictionary<int, int> levels)
    {
        foreach (var kv in levels)
        {
            updates[DBPathMaker.Join(basePath, kv.Key.ToString())] = Mathf.Max(0, kv.Value);
        }
    }
}
