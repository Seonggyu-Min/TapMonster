using System;
using UnityEngine;


[Flags]
public enum SafeAreaEdges
{ 
    Left = 1,
    Right = 2,
    Bottom = 4,
    Top = 8,
    All = 15
}


[ExecuteAlways]
public class SafeAreaApplier : MonoBehaviour
{
    [SerializeField] private RectTransform _safeAreaRoot;
    [SerializeField] private SafeAreaEdges _applyEdges = SafeAreaEdges.All;

    private Rect _lastSafeArea;
    private Vector2Int _lastScreenSize;
    private ScreenOrientation _lastOrientation;


    private void Reset()
    {
        _safeAreaRoot = transform as RectTransform;
    }

    private void OnEnable()
    {
        Apply();
    }

    private void Update()
    {
        if (_lastSafeArea != Screen.safeArea ||
            _lastScreenSize.x != Screen.width ||
            _lastScreenSize.y != Screen.height ||
            _lastOrientation != Screen.orientation)
        {
            Apply();
        }
    }

    private void Apply()
    {
        if (_safeAreaRoot == null) return;

        Rect safe = Screen.safeArea;

        Vector2 min = safe.position;
        Vector2 max = safe.position + safe.size;

        Vector2 anchorMin = min;
        Vector2 anchorMax = max;

        anchorMin.x /= Screen.width;
        anchorMax.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.y /= Screen.height;

        // 적용하지 않을 엣지에 대해서는 기본값으로 설정
        if ((_applyEdges & SafeAreaEdges.Left) == 0) anchorMin.x = 0f;
        if ((_applyEdges & SafeAreaEdges.Right) == 0) anchorMax.x = 1f;
        if ((_applyEdges & SafeAreaEdges.Bottom) == 0) anchorMin.y = 0f;
        if ((_applyEdges & SafeAreaEdges.Top) == 0) anchorMax.y = 1f;

        _safeAreaRoot.anchorMin = anchorMin;
        _safeAreaRoot.anchorMax = anchorMax;
        _safeAreaRoot.offsetMin = Vector2.zero;
        _safeAreaRoot.offsetMax = Vector2.zero;
    }
}
