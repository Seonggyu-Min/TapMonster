using DG.Tweening;
using UnityEngine;

public class AutoAttackerView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Ease _flashEase = Ease.OutCubic;

    private MaterialPropertyBlock _mpb;
    private Tween _flashTween;

    private static readonly int FlashId = Shader.PropertyToID("_Flash");

    private const LogCategory CurrentCategory = LogCategory.GameLogic;


    private void Awake()
    {
        this.TryBindComponent(ref _renderer, CurrentCategory);
        _mpb = new();
    }

    public void Flash(float duration)
    {
        _flashTween?.Kill();

        _flashTween = DOTween.To(
            () => 1f,
            SetFlash,
            0f,
            duration
        ).SetEase(_flashEase);
    }

    private void SetFlash(float value)
    {
        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat(FlashId, value);
        _renderer.SetPropertyBlock(_mpb);
    }
}
