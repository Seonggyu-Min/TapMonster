using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStateModel
{
    #region Fields and Properties

    private RelicModel _relicModel;
    private SkillModel _skillModel;
    private SkillCooldownModel _skillCooldownModel;
    private SkillSlotModel _skillSlotModel;
    private StageModel _stageModel;
    private MonsterHpModel _monsterHpModel;
    private UpgradeModel _upgradeModel;
    private WalletModel _walletModel;
    private BossTimerModel _bossTimerModel;

    private const LogCategory CurrentCategory = LogCategory.GameLogic;


    public GameStateModel(
        RelicModel relicModel,
        SkillModel skillModel,
        SkillCooldownModel skillCooldownModel,
        SkillSlotModel skillSlotModel,
        StageModel stageModel,
        MonsterHpModel monsterHpModel,
        UpgradeModel upgradeModel,
        WalletModel walletModel,
        BossTimerModel bossTimerModel
        )
    {
        _relicModel = relicModel;
        _skillModel = skillModel;
        _skillCooldownModel = skillCooldownModel;
        _skillSlotModel = skillSlotModel;
        _stageModel = stageModel;
        _monsterHpModel = monsterHpModel;
        _upgradeModel = upgradeModel;
        _walletModel = walletModel;
        _bossTimerModel = bossTimerModel;
    }

    private BossTimerDTO _loadedBossTimerDTO;
    public BossTimerDTO LoadedBossTimerDTO => _loadedBossTimerDTO;




    public RelicModel RelicModel => _relicModel;
    public SkillModel SkillModel => _skillModel;
    public SkillCooldownModel SkillCooldownModel => _skillCooldownModel;    // 현재는 DTO 변환 안함
    public SkillSlotModel SkillSlotModel => _skillSlotModel;
    public StageModel StageModel => _stageModel;
    public MonsterHpModel MonsterHpModel => _monsterHpModel;
    public UpgradeModel UpgradeModel => _upgradeModel;
    public WalletModel WalletModel => _walletModel;
    public BossTimerModel BossTimerModel => _bossTimerModel;

    #endregion


    #region Public Methods

    // Model To DTO
    public SaveDataDTO ToDTO(long nowUnixMs)
    {
        var dto = new SaveDataDTO
        {
            LastSavedAtUnixMs = nowUnixMs,
            StageDTO = new(),
            WalletDTO = new(),
            SkillSlotDTO = new(),
        }.Normalized();

        // Stage
        FillStages(dto.StageDTO, _stageModel.CurrentStage);

        // Wallet
        FillCurrencies(dto.WalletDTO.Currencies, _walletModel.Values);

        // Skill Slots
        FillSkillSlots(_skillSlotModel, dto.SkillSlotDTO);

        // Levels
        FillLevels(dto.RelicLevels, _relicModel.RelicLevels);
        FillLevels(dto.UpgradeLevels, _upgradeModel.UpgradeLevels);
        FillLevels(dto.SkillLevels, _skillModel.SkillLevels);

        // Boss Timer
        FillBossTimer(dto.BossTimerDTO, _bossTimerModel);

        return dto;
    }

    // DTO to Model
    public void ApplyToClient(SaveDataDTO dto, out bool generated)
    {
        generated = false;

        if (dto == null)
        {
            this.PrintLog("dto가 null입니다.", CurrentCategory, LogType.Error);
            return;
        }

        dto = dto.Normalized();

        // Stage
        ApplyStage(_stageModel, dto.StageDTO);

        // Monster Hp
        ApplyMonsterHp(_monsterHpModel, dto.MonsterHpDTO);

        // Wallet
        ApplyCurrencies(_walletModel, dto.WalletDTO.Currencies);

        // Levels
        ApplyLevels(_relicModel, dto.RelicLevels);
        ApplyLevels(_upgradeModel, dto.UpgradeLevels);
        ApplyLevels(_skillModel, dto.SkillLevels);

        // Skill slots
        EnsureSkillSlots(_skillSlotModel, dto.SkillSlotDTO, out generated);

        // Boss Timer
        _loadedBossTimerDTO = dto.BossTimerDTO;
    }

    #endregion


    #region Private Methods

    private static void FillStages(StageDTO stageDTO, int stage)
    {
        stageDTO.CurrentStage = stage;
    }

    private static void FillCurrencies(Dictionary<string, BigNumberDTO> dst, IReadOnlyDictionary<CurrencyId, BigNumber> src)
    {
        dst.Clear();
        foreach (var kv in src)
        {
            dst[kv.Key.ToString()] = new BigNumberDTO(kv.Value);
        }
    }

    private static void FillSkillSlots(SkillSlotModel model, SkillSlotDTO dst)
    {
        // Equipped
        if (dst.Equipped == null || dst.Equipped.Length != SkillSlotModel.EquippedSlotCount)
        {
            dst.Equipped = new int[SkillSlotModel.EquippedSlotCount];
        }

        for (int i = 0; i < SkillSlotModel.EquippedSlotCount; i++)
        {
            dst.Equipped[i] = model.GetEquipped(i);
        }

        // Inventory
        dst.Inventory ??= new();
        dst.Inventory.Clear();
        for (int i = 0; i < model.Inventory.Count; i++)
        {
            dst.Inventory.Add(model.Inventory[i]);
        }
    }


    private static void FillLevels(Dictionary<string, int> dst, IReadOnlyDictionary<int, int> src)
    {
        dst.Clear();
        foreach (var kv in src)
        {
            dst[kv.Key.ToString()] = kv.Value;
        }
    }

    private static void FillBossTimer(BossTimerDTO dst, BossTimerModel model)
    {
        if (dst == null) return;

        if (model == null || !model.IsRunning)
        {
            dst.IsRunning = false;
            dst.BossStage = 0;
            dst.RemainingSeconds = 0f;
            return;
        }

        dst.IsRunning = true;
        dst.BossStage = model.BossStage;
        dst.RemainingSeconds = model.RemainingSeconds;
    }

    private static void ApplyStage(StageModel stageModel, StageDTO stageDto)
    {
        int stage = stageDto?.CurrentStage ?? 1;
        stageModel.SetStage(Mathf.Max(1, stage));
    }

    private static void ApplyMonsterHp(MonsterHpModel model, MonsterHpDTO src)
    {
        if (model == null) return;
        if (src == null || !src.HasValue || src.MaxHp == null || src.CurrentHp == null)
        {
            model.SetLoadedFlag(false);
            return;
        }

        BigNumber max = new(src.MaxHp.Mantissa, src.MaxHp.Exponent);
        BigNumber cur = new(src.CurrentHp.Mantissa, src.CurrentHp.Exponent);

        model.SetSilently(max, cur);
    }

    private static void ApplyLevels(ILevelModel model, Dictionary<string, int> src)
    {
        model.Clear();
        if (src == null) return;

        foreach (var kv in src)
        {
            if (!int.TryParse(kv.Key, out int id)) continue;
            model.SetLevel(id, Mathf.Max(0, kv.Value));
        }
    }

    private static void ApplyCurrencies(WalletModel walletModel, Dictionary<string, BigNumberDTO> src)
    {
        if (src == null) return;

        foreach (var kv in src)
        {
            if (!Enum.TryParse<CurrencyId>(kv.Key, ignoreCase: true, out var currency)) continue;

            var bnDto = kv.Value;
            if (bnDto == null) continue;

            walletModel.Set(currency, new BigNumber(bnDto.Mantissa, bnDto.Exponent));
        }
    }

    private void EnsureSkillSlots(SkillSlotModel skillSlotModel, SkillSlotDTO slotDto, out bool generated)
    {
        // 1. 슬롯이 있으면 그대로 적용
        if (slotDto != null && slotDto.Inventory != null && slotDto.Inventory.Count > 0)
        {
            _skillSlotModel.ApplySnapshot(slotDto.Equipped, slotDto.Inventory);
            generated = false;
            return;
        }

        // 2. 슬롯이 없으면 SkillModel 기반으로 생성
        List<int> owned = BuildOwnedSkillsFromSkillModel(_skillModel);

        int[] equipped = new int[SkillSlotModel.EquippedSlotCount];
        for (int i = 0; i < equipped.Length; i++)
        {
            equipped[i] = SkillId.None;
        }

        generated = true;
        _skillSlotModel.SetInitial(owned, equipped);
    }

    private static List<int> BuildOwnedSkillsFromSkillModel(SkillModel skillModel)
    {
        var list = new List<int>();
        foreach (var kv in skillModel.SkillLevels)
        {
            int id = kv.Key;
            int level = kv.Value;

            if (level <= 0) continue;
            if (id == SkillId.None) continue;

            list.Add(id);
        }
        list.Sort();
        return list;
    }

    #endregion
}
