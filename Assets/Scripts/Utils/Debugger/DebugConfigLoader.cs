using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugConfigLoader : MonoBehaviour
{
    [SerializeField] private DebugConfigSO _config;

    private void Awake() => DebugConfig.Current = _config;
}
