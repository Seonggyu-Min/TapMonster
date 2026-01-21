using System.Collections.Generic;

public static class SaveDataDtoExtensions
{
    public static SaveDataDTO Normalized(this SaveDataDTO dto)
    {
        dto ??= new SaveDataDTO();

        dto.StageDTO ??= new();
        dto.WalletDTO ??= new();
        dto.SkillSlotDTO ??= new();

        dto.WalletDTO.Currencies ??= new();

        dto.RelicLevels ??= new();
        dto.UpgradeLevels ??= new();
        dto.SkillLevels ??= new();

        return dto;
    }
}
