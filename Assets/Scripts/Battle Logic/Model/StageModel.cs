using System;
using UnityEngine;

public class StageModel
{
    private int _currentStage = -1;

    public event Action<int> OnStageChanged;

    public int CurrentStage => _currentStage;

    public void SetStage(int stage)
    {
        _currentStage = Mathf.Clamp(stage, 1, int.MaxValue);
        OnStageChanged?.Invoke(_currentStage);
    }
}
