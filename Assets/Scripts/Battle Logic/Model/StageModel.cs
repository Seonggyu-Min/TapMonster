using UnityEngine;

public class StageModel
{
    private int _currentStage = -1;

    public int CurrentStage => _currentStage;

    public void SetStage(int stage)
    {
        _currentStage = Mathf.Clamp(stage, 1, int.MaxValue);
    }
}
