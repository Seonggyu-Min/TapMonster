using UnityEngine;
using UnityEngine.UI;

public class PanelCloser : MonoBehaviour
{
    [SerializeField] private UIKeyEnum _uIKeyEnumToClose;
    [SerializeField] private Button _closeButton;


    private void OnEnable()
    {
        if (_closeButton != null)
        {
            _closeButton.onClick.AddListener(ClosePanel);
        }
    }

    private void OnDisable()
    {
        if (_closeButton != null)
        {
            _closeButton.onClick.RemoveListener(ClosePanel);
        }
    }

    private void ClosePanel()
    {
        UIManager.Instance.Hide(UIKey.EnumToString(_uIKeyEnumToClose));
    }
}
