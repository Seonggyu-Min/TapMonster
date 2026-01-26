using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossTimerView : MonoBehaviour
{
    [SerializeField] private GameObject _bossTimerView;
    [SerializeField] private Image _timeFillImage;
    [SerializeField] private TMP_Text _timerText;

    private const LogCategory CurrentCategory = LogCategory.UI;

    private void Awake()
    {
        if (_bossTimerView == null)
        {
            this.PrintLog("_bossTimerView가 null입니다.", CurrentCategory, LogType.Error);
        }
        // Children에서 찾으면 Image가 해당 이미지로 찾는다는 보장이 없음
        if (_timeFillImage == null)
        {
            this.PrintLog("_timeFillImage가 null입니다.", CurrentCategory, LogType.Error);
        }
        this.TryBindComponentInChildren(ref _timerText, CurrentCategory);
    }

    public void SetVisible(bool visible)
    {
        if (_bossTimerView != null)
        {
            _bossTimerView.SetActive(visible);
        }
    }

    public void SetTimer(float remaining, float duration)
    {
        int sec = Mathf.CeilToInt(remaining);
        if (_timerText != null) _timerText.text = sec.ToString();

        float normalized = (duration <= 0f) ? 0f : remaining / duration;
        normalized = Mathf.Clamp01(normalized);

        if (_timeFillImage != null)
        {
            _timeFillImage.fillAmount = normalized;
        }
    }
}
