using System;
using System.Collections.Generic;

[Serializable]
public class SaveDataDTO
{
    public int Version = 0; // 일단 안씀

    public long LastSavedAtUnixMs;

    public StageDTO StageDTO = new();
    public WalletDTO WalletDTO = new();

    public Dictionary<string, int> RelicLevels = new();
    public Dictionary<string, int> UpgradeLevels = new();
    public Dictionary<string, int> SkillLevels = new();
}


[Serializable]
public class StageDTO
{
    public int CurrentStage = 1;
}


[Serializable]
public class WalletDTO
{
    public Dictionary<string, BigNumberDTO> Currencies = new();
}


[Serializable]
public class BigNumberDTO
{
    public double Mantissa = 0.0;
    public int Exponent = 0;

    public BigNumberDTO() { }

    public BigNumberDTO(BigNumber bigNumber)
    {
        Mantissa = bigNumber.Mantissa;
        Exponent = bigNumber.Exponent;
    }

    public BigNumberDTO(double mantissa, int exponent)
    {
        Mantissa = mantissa;
        Exponent = exponent;
    }
}