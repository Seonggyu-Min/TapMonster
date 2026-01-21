using UnityEngine;

public struct SkillItemVM : IBottomItemData
{
    public int Id { get; }
    public string Name { get; }
    public Sprite Icon { get; }

    public int Level { get; }
    public int MaxLevel { get; }

    public Cost NextCost { get; }
    public bool CanBuy { get; }

    public bool IsMaxed => Level >= MaxLevel;

    public SkillItemVM(
        int id, string name, Sprite icon,
        int level, int maxLevel,
        Cost nextCost, bool canBuy)
    {
        Id = id;
        Name = name;
        Icon = icon;
        Level = level;
        MaxLevel = maxLevel;
        NextCost = nextCost;
        CanBuy = canBuy;
    }
}
