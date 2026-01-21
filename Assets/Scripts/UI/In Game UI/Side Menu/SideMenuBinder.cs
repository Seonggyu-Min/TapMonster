using UnityEngine;
using UnityEngine.UI;

public class SideMenuBinder : MonoBehaviour
{
    [SerializeField] private Button _skillInventoryButton;


    private void OnEnable()
    {
        _skillInventoryButton.onClick.AddListener(OnClickSkillInventoryButton);
    }

    private void OnDisable()
    {
        _skillInventoryButton.onClick.RemoveListener(OnClickSkillInventoryButton);
    }

    private void OnClickSkillInventoryButton()
    {
        UIManager.Instance.Show(UIKey.SkillInventoryPanel);
    }
}
