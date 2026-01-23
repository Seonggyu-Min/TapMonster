using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHpView : MonoBehaviour
{
    [SerializeField] private Image _fill;
    [SerializeField] private TMP_Text _hpText;

    public void SetNormalized(float t01)
    {
        if (_fill != null)
        {
            float amount = Mathf.Clamp01(t01);
            _fill.fillAmount = amount;
        }
    }

    public void SetText(string s)
    {
        if (_hpText != null)
        {
            _hpText.text = s;
        }
    }
}
