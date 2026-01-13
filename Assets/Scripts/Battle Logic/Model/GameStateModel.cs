using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStateModel
{
    #region Fields and Properties

    private RelicModel _relicModel;
    private SkillModel _skillModel;
    private StageModel _stageModel;
    private UpgradeModel _upgradeModel;
    private WalletModel _walletModel;

    private const LogCategory CurrentCategory = LogCategory.GameLogic;


    public GameStateModel(
        RelicModel relicModel,
        SkillModel skillModel,
        StageModel stageModel,
        UpgradeModel upgradeModel,
        WalletModel walletModel
        )
    {
        _relicModel = relicModel;
        _skillModel = skillModel;
        _stageModel = stageModel;
        _upgradeModel = upgradeModel;
        _walletModel = walletModel;
    }


    public RelicModel RelicModel => _relicModel;
    public SkillModel SkillModel => _skillModel;
    public StageModel StageModel => _stageModel;
    public UpgradeModel UpgradeModel => _upgradeModel;
    public WalletModel WalletModel => _walletModel;

    #endregion


    #region Public Methods

    // Model To DTO
    public SaveDataDTO ToDTO(long nowUnixMs)
    {
        var dto = new SaveDataDTO
        {
            LastSavedAtUnixMs = nowUnixMs,
            StageDTO = new StageDTO(),
            WalletDTO = new WalletDTO()
        }.Normalized();

        // Stage
        FillStages(dto.StageDTO, _stageModel.CurrentStage);
        

        // Wallet
        FillCurrencies(dto.WalletDTO.Currencies, _walletModel.Values);

        // Levels
        FillLevels(dto.RelicLevels, _relicModel.RelicLevels);
        FillLevels(dto.UpgradeLevels, _upgradeModel.UpgradeLevels);
        FillLevels(dto.SkillLevels, _skillModel.SkillLevels);

        return dto;
    }

    // DTO to Model
    public void ApplyToClient(SaveDataDTO dto)
    {
        if (dto == null)
        {
            this.PrintLog("dto가 null입니다.", CurrentCategory, LogType.Error);
            return;
        }

        dto = dto.Normalized();

        // Stage
        ApplyStage(_stageModel, dto.StageDTO);

        // Wallet
        ApplyCurrencies(_walletModel, dto.WalletDTO.Currencies);

        // Levels
        ApplyLevels(_relicModel, dto.RelicLevels);
        ApplyLevels(_upgradeModel, dto.UpgradeLevels);
        ApplyLevels(_skillModel, dto.SkillLevels);
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

    private static void FillLevels(Dictionary<string, int> dst, IReadOnlyDictionary<int, int> src)
    {
        dst.Clear();
        foreach (var kv in src)
        {
            dst[kv.Key.ToString()] = kv.Value;
        }
    }



    private static void ApplyStage(StageModel stageModel, StageDTO stageDto)
    {
        int stage = stageDto?.CurrentStage ?? 1;
        stageModel.SetStage(Mathf.Max(1, stage));
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

    #endregion
}
