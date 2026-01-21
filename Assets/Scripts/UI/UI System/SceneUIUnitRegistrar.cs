using UnityEngine;

public class SceneUIUnitRegistrar : MonoBehaviour
{
    private void Awake()
    {
        foreach (var unit in GetComponentsInChildren<UIUnit>(true))
        {
            string key = unit.name;
            UIUnit go = unit;

            UIManager.Instance.RegisterUnit(key, go);
        }
    }

    private void OnDestroy()
    {
        // 만약 씬이 바뀌어도 유지되어야 하는 유닛이 있다면 이거 수정하고 직접 key 넣어서 지우도록 바꿔야 됨
        if (UIManager.TryGetInstance(out var instance))
        {
            instance.ClearUnit();
        }
    }
}
