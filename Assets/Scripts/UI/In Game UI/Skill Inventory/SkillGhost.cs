using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillGhost : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Color ghostColor = new(1f, 1f, 1f, 0.75f);
    private RectTransform _rect;

    private void Awake()
    {
        _rect = transform as RectTransform;
        if (iconImage != null)
        {
            iconImage.raycastTarget = false;
        }
    }

    public void SetIcon(Sprite icon)
    {
        if (iconImage == null) return;

        if (icon == null)
        {
            iconImage.enabled = false;
            return;
        }

        iconImage.enabled = true;
        iconImage.sprite = icon;
        iconImage.color = ghostColor;
    }

    public void SetScreenPosition(Vector2 screenPos, Camera eventCamera, Canvas rootCanvas)
    {
        if (_rect == null || rootCanvas == null) return;

        RectTransform canvasRect = rootCanvas.transform as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            eventCamera,
            out Vector2 localPoint
        );

        _rect.anchoredPosition = localPoint;
    }
}
