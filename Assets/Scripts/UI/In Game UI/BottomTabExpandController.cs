using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class BottomTabExpandController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Button _toggleButton;
    [SerializeField] private TMP_Text _buttonText;
    [SerializeField] private RectTransform _heightTarget;

    [Header("Height Ratio")]
    [SerializeField, Range(0f, 1f)] private float _expandedHeightRatio = 1f;
    [SerializeField, Range(0f, 1f)] private float _collapsedHeightRatio = 0.2f;

    [Header("Tween")]
    [SerializeField] private Ease _ease;
    [SerializeField] private float _duration = 0.25f;

    [SerializeField] private bool _isExpanded = true;

    private Tween _sizeTween;

    private void OnEnable()
    {
        _toggleButton.onClick.AddListener(OnClick);
        StartCoroutine(InitAndWaitForSnap());
    }

    private void OnDisable()
    {
        _toggleButton.onClick.RemoveListener(OnClick);
        _sizeTween?.Kill();
        _sizeTween = null;
        SetInputLocked(false);
    }

    private void OnRectTransformDimensionsChange()
    {
        if (!isActiveAndEnabled) return;

        _sizeTween?.Kill();
        _sizeTween = null;
        SnapToState();
    }

    private void OnClick()
    {
        if (_sizeTween != null && _sizeTween.IsActive() && _sizeTween.IsPlaying())
            return;

        _isExpanded = !_isExpanded;
        SetButtonText();
        AnimateToState();
    }

    private void AnimateToState()
    {
        _sizeTween?.Kill();

        float targetH = GetTargetHeight(_isExpanded ? _expandedHeightRatio : _collapsedHeightRatio);

        SetInputLocked(true);

        Vector2 to = _heightTarget.sizeDelta;
        to.y = targetH;

        _sizeTween = _heightTarget
            .DOSizeDelta(to, _duration)
            .SetEase(_ease)
            .SetUpdate(true)
            .OnComplete(EndTween)
            .OnKill(EndTween);
    }

    private void EndTween()
    {
        SetInputLocked(false);
        _sizeTween = null;
    }

    private IEnumerator InitAndWaitForSnap()
    {
        SetButtonText();
        yield return null;
        SnapToState();
    }

    private void SnapToState()
    {
        float h = GetTargetHeight(_isExpanded ? _expandedHeightRatio : _collapsedHeightRatio);
        Vector2 size = _heightTarget.sizeDelta;
        size.y = h;
        _heightTarget.sizeDelta = size;
    }

    private float GetTargetHeight(float ratio)
    {
        RectTransform parent = _heightTarget.parent as RectTransform;
        float parentH = parent != null ? parent.rect.height : 0f;
        return parentH * Mathf.Clamp01(ratio);
    }

    private void SetInputLocked(bool locked)
    {
        _toggleButton.interactable = !locked;
    }

    private void SetButtonText()
    {
        _buttonText.text = _isExpanded ? "▼" : "▲";
    }
}