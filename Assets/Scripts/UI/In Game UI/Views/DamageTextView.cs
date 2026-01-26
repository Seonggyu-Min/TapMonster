using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamageTextView : PooledObject<DamageTextView>, IPoolable
{
    [Header("Refs")]
    [SerializeField] private TMP_Text _damageText;
    [SerializeField] private CanvasGroup _canvasGroup;

    [Header("Critical Config")]
    [SerializeField] private Color _nonCriticalColor;
    [SerializeField] private Color _criticalColor;
    [SerializeField] private float _criticalFontSizeIncreaseAmount = 10;

    [Header("Tween Config")]
    [SerializeField] private float _riseY = 80f;
    [SerializeField] private float _duration = 0.6f;
    [SerializeField] private bool _unscaledTime = true;

    private RectTransform _rect;
    private Tween _tween;

    private const LogCategory CurrenCategory = LogCategory.UI;

    private void Awake()
    {
        this.TryBindComponent(ref _canvasGroup, CurrenCategory);
        this.TryBindComponentInChildren(ref _damageText, CurrenCategory);
        _rect = (RectTransform)transform;
    }

    public void Show(string text, Vector2 anchoredPos, bool isCritical)
    {
        if (_damageText != null)
        {
            if (isCritical)
            {
                _damageText.color = _criticalColor;
                _damageText.text = $"{text}!";
                _damageText.fontSize
                    = _damageText.fontSize + _criticalFontSizeIncreaseAmount;
            }
            else
            {
                _damageText.color = _nonCriticalColor;
                _damageText.text = text;
            }
        }

        _rect.anchoredPosition = anchoredPos;
        if (_canvasGroup != null) _canvasGroup.alpha = 1f;

        PlayTween();
    }

    private void PlayTween()
    {
        _tween?.Kill();

        Vector2 start = _rect.anchoredPosition;
        Vector2 end = start + Vector2.up * _riseY;

        _tween = DOTween.Sequence()
            .SetUpdate(_unscaledTime)
            .Append(_rect.DOAnchorPos(end, _duration))
            .Join(_canvasGroup != null ? _canvasGroup.DOFade(0f, _duration) : null)
            .OnComplete(ReturnPool);
    }

    public void OnPop()
    {
        _tween?.Kill();
        _tween = null;
        if (_canvasGroup != null) _canvasGroup.alpha = 1f;
    }

    public void OnPush()
    {
        _tween?.Kill();
        _tween = null;

        if (_canvasGroup != null)
        {
            _canvasGroup.DOKill();
            _canvasGroup.alpha = 1f;
        }
    }
}
