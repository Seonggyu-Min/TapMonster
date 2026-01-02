using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/AddressablesLables")]
public class AddressablesLableSO : ScriptableObject
{
    [SerializeField] private List<string> _labels = new();
    public List<string> Labels => _labels;
}
