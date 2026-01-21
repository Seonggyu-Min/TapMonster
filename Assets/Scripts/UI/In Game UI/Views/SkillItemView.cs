using TMPro;
using UnityEngine;

public class SkillItemView : BottomItemView<SkillItemVM>
{
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private TMP_Text _costText;

    protected override void ApplySpecific(SkillItemVM data)
    {
        _levelText.text = $"{data.Level}/{data.MaxLevel}";
        _costText.text = data.IsMaxed ? "MAX" : data.NextCost.ToString();
        _button.interactable = !data.IsMaxed && data.CanBuy;
    }
}
