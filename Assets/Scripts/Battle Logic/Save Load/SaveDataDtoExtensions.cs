using System.Collections.Generic;

public static class SaveDataDtoExtensions
{
    public static SaveDataDTO Normalized(this SaveDataDTO dto)
    {
        dto ??= new SaveDataDTO();

        dto.StageDTO ??= new StageDTO();
        dto.WalletDTO ??= new WalletDTO();

        dto.WalletDTO.Currencies ??= new Dictionary<string, BigNumberDTO>();

        dto.RelicLevels ??= new Dictionary<string, int>();
        dto.UpgradeLevels ??= new Dictionary<string, int>();
        dto.SkillLevels ??= new Dictionary<string, int>();

        return dto;
    }
}
