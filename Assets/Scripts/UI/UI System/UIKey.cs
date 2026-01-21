public static class UIKey
{
    public const string SkillInventoryPanel = "Skill Inventory Panel";


    public static string EnumToString(UIKeyEnum key)
    {
        return key switch
        {
            UIKeyEnum.SkillInventoryPanel => SkillInventoryPanel,



            _ => string.Empty
        };
    }
}

public enum UIKeyEnum
{
    SkillInventoryPanel,
}