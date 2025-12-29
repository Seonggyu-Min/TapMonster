using UnityEditor;
using UnityEngine;

public static class CiRefresh
{
    public static void Refresh()
    {
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
        Debug.Log("[CI] Refresh 완료");
    }
}
