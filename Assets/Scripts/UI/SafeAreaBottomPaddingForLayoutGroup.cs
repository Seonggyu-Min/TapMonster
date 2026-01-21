using System.Collections;
using UnityEngine;
using UnityEngine.UI;


[ExecuteAlways]
public class SafeAreaBottomPaddingForLayoutGroup : MonoBehaviour
{
    [SerializeField] private VerticalLayoutGroup _layout;
    [SerializeField] private int _extraPadding = 0; // 추가 여유

    private int _lastBottom;

    //private void OnEnable()
    //{
    //    StartCoroutine(Init());
    //}

    //private IEnumerator Init()
    //{
    //    yield return null;
    //    Apply();
    //}


    private void OnRectTransformDimensionsChange()
    {
        Apply();
    }

    private void Apply()
    {
        Rect safe = Screen.safeArea;
        int bottomInset = Mathf.RoundToInt(safe.y);

        if (bottomInset == _lastBottom) return;
        _lastBottom = bottomInset;

        if (_layout == null) return;

        RectOffset p = _layout.padding;
        p.bottom = bottomInset + _extraPadding;
        _layout.padding = p;

        LayoutRebuilder.MarkLayoutForRebuild(_layout.transform as RectTransform);
    }
}
