using TMPro;
using UnityEngine;

public class StageIndexView : MonoBehaviour
{
    [SerializeField] private TMP_Text _stageIndexText;


    public void SetStage(int i)
    {
        _stageIndexText.text = $"stage: {i}";
    }
}
