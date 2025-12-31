using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/DebugConfig")]
public class DebugConfigSO : ScriptableObject
{
    [field: SerializeField] public bool EnableLog { get; private set; } = true;

}
