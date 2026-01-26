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
    SkillSlots = 1 << 5,
    MonsterHp = 1 << 6,
    BossTimer = 1 << 7,
    All = Stage | Relic | Upgrade | Skill | Wallet | SkillSlots | MonsterHp | BossTimer,
}


public class SavePatchBuilder
{
    public Dictionary<string, object> BuildPatchFromGameState(string uid, SaveDirtyFlags dirty, GameStateModel gs)
    {
        var updates = new Dictionary<string, object>(128);

        if ((dirty & SaveDirtyFlags.Stage) != 0)
            WriteStage(updates, uid, gs);

        if ((dirty & SaveDirtyFlags.Wallet) != 0)
            WriteWallet(updates, uid, gs);

        if ((dirty & SaveDirtyFlags.Relic) != 0)
            WriteLevelsIntKey(updates, DBRoutes.RelicLevels(uid), gs.RelicModel.RelicLevels);

        if ((dirty & SaveDirtyFlags.Upgrade) != 0)
            WriteLevelsIntKey(updates, DBRoutes.UpgradeLevels(uid), gs.UpgradeModel.UpgradeLevels);

        if ((dirty & SaveDirtyFlags.Skill) != 0)
            WriteLevelsIntKey(updates, DBRoutes.SkillLevels(uid), gs.SkillModel.SkillLevels);

        if ((dirty & SaveDirtyFlags.SkillSlots) != 0)
            WriteSkillSlotsFromModel(updates, uid, gs.SkillSlotModel);

        if ((dirty & SaveDirtyFlags.MonsterHp) != 0)
            WriteMonsterHpFromModel(updates, uid, gs.MonsterHpModel);

        if ((dirty & SaveDirtyFlags.BossTimer) != 0)
            WriteBossTimer(updates, uid, gs.BossTimerModel);

        return updates;
    }

    public Dictionary<string, object> BuildFullFromDTO(string uid, SaveDataDTO dto)
    {
        dto = dto?.Normalized() ?? new SaveDataDTO().Normalized();

        var updates = new Dictionary<string, object>(128);

        // Stage
        updates[DBRoutes.CurrentStage(uid)] = dto.StageDTO?.CurrentStage ?? 1;

        // Monster Hp
        WriteMonsterHpFromDTO(updates, uid, dto.MonsterHpDTO);

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
            updates[DBRoutes.GoldMantissa(uid)] = 0d;
            updates[DBRoutes.GoldExponent(uid)] = 0;
        }

        // SkillSlots
        WriteSkillSlotsFromDTO(updates, uid, dto.SkillSlotDTO);

        // Levels
        WriteLevelsStringKey(updates, DBRoutes.RelicLevels(uid), dto.RelicLevels);
        WriteLevelsStringKey(updates, DBRoutes.UpgradeLevels(uid), dto.UpgradeLevels);
        WriteLevelsStringKey(updates, DBRoutes.SkillLevels(uid), dto.SkillLevels);

        return updates;
    }

    private static void WriteStage(Dictionary<string, object> updates, string uid, GameStateModel gs)
        => updates[DBRoutes.CurrentStage(uid)] = gs.StageModel.CurrentStage;

    private static void WriteWallet(Dictionary<string, object> updates, string uid, GameStateModel gs)
    {
        BigNumber gold = gs.WalletModel.Get(CurrencyId.Gold);
        updates[DBRoutes.GoldMantissa(uid)] = gold.Mantissa;
        updates[DBRoutes.GoldExponent(uid)] = gold.Exponent;
    }

    private static void WriteLevelsIntKey(Dictionary<string, object> updates, string basePath, IReadOnlyDictionary<int, int> levels)
    {
        if (levels == null) return;
        foreach (var kv in levels)
            updates[DBPathMaker.Join(basePath, kv.Key.ToString())] = Mathf.Max(0, kv.Value);
    }

    private static void WriteLevelsStringKey(Dictionary<string, object> updates, string basePath, Dictionary<string, int> levels)
    {
        if (levels == null) return;
        foreach (var kv in levels)
            updates[DBPathMaker.Join(basePath, kv.Key)] = Mathf.Max(0, kv.Value);
    }

    private static void WriteSkillSlotsFromModel(Dictionary<string, object> updates, string uid, SkillSlotModel model)
    {
        // Equipped
        for (int i = 0; i < SkillSlotModel.EquippedSlotCount; i++)
            updates[DBRoutes.SkillEquippedAt(uid, i)] = model != null ? model.GetEquipped(i) : SkillId.None;

        // Inventory
        var invDict = new Dictionary<string, object>();
        if (model?.Inventory != null)
        {
            for (int i = 0; i < model.Inventory.Count; i++)
                invDict[i.ToString()] = model.Inventory[i];
        }
        updates[DBRoutes.SkillSlotsInventory(uid)] = invDict;
    }

    private static void WriteSkillSlotsFromDTO(Dictionary<string, object> updates, string uid, SkillSlotDTO slotDto)
    {
        // Equipped
        int[] eq = slotDto?.Equipped;
        for (int i = 0; i < SkillSlotModel.EquippedSlotCount; i++)
        {
            int id = (eq != null && i < eq.Length) ? eq[i] : SkillId.None;
            updates[DBRoutes.SkillEquippedAt(uid, i)] = id;
        }

        // Inventory
        var invDict = new Dictionary<string, object>();
        if (slotDto?.Inventory != null)
        {
            for (int i = 0; i < slotDto.Inventory.Count; i++)
                invDict[i.ToString()] = slotDto.Inventory[i];
        }
        updates[DBRoutes.SkillSlotsInventory(uid)] = invDict;
    }

    private static void WriteMonsterHpFromModel(Dictionary<string, object> updates, string uid, MonsterHpModel model)
    {
        if (model == null) return;

        updates[DBRoutes.MonsterHpHasValue(uid)] = true;

        updates[DBRoutes.MonsterMaxHpMantissa(uid)] = model.MaxHp.Mantissa;
        updates[DBRoutes.MonsterMaxHpExponent(uid)] = model.MaxHp.Exponent;

        updates[DBRoutes.MonsterCurHpMantissa(uid)] = model.CurrentHp.Mantissa;
        updates[DBRoutes.MonsterCurHpExponent(uid)] = model.CurrentHp.Exponent;
    }

    private static void WriteMonsterHpFromDTO(Dictionary<string, object> updates, string uid, MonsterHpDTO mhp)
    {
        bool has = mhp != null && mhp.HasValue && mhp.MaxHp != null && mhp.CurrentHp != null;
        updates[DBRoutes.MonsterHpHasValue(uid)] = has;

        BigNumberDTO max = mhp?.MaxHp;
        BigNumberDTO cur = mhp?.CurrentHp;

        updates[DBRoutes.MonsterMaxHpMantissa(uid)] = has ? max.Mantissa : 0d;
        updates[DBRoutes.MonsterMaxHpExponent(uid)] = has ? max.Exponent : 0;

        updates[DBRoutes.MonsterCurHpMantissa(uid)] = has ? cur.Mantissa : 0d;
        updates[DBRoutes.MonsterCurHpExponent(uid)] = has ? cur.Exponent : 0;
    }

    private static void WriteBossTimer(Dictionary<string, object> updates, string uid, BossTimerModel model)
    {
        bool isRunning = model != null && model.IsRunning;

        updates[DBRoutes.BossTimerIsRunning(uid)] = isRunning;
        updates[DBRoutes.BossTimerBossStage(uid)] = isRunning ? model.BossStage : 0;
        updates[DBRoutes.BossTimerRemainingSeconds(uid)] = isRunning ? model.RemainingSeconds : 0f;
    }
}
