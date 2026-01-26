using UnityEngine;

public class BossTimerModel
{
    public bool IsRunning { get; private set; }
    public int BossStage { get; private set; }
    public float DurationSeconds { get; private set; }
    public float RemainingSeconds { get; private set; }
    public float NormalizedRemaining
        => DurationSeconds <= 0f ? 0f : RemainingSeconds / DurationSeconds;

    public void Start(int bossStage, float durationSeconds)
    {
        IsRunning = true;
        BossStage = bossStage;

        DurationSeconds = durationSeconds;
        RemainingSeconds = durationSeconds;
    }

    public void Stop()
    {
        IsRunning = false;
        BossStage = 0;

        DurationSeconds = 0f;
        RemainingSeconds = 0f;
    }

    public void SetRemaining(float remaining)
    {
        RemainingSeconds = Mathf.Clamp(remaining, 0f, DurationSeconds);
    }

    public void SetDuration(float duration)
    {
        if (duration < 1f)
        {
            duration = 1f;
        }
        DurationSeconds = duration;

        if (RemainingSeconds > DurationSeconds)
        {
            RemainingSeconds = DurationSeconds;
        }
    }

    public void AddRemaining(float delta)
    {
        SetRemaining(RemainingSeconds + delta);
    }

    public void AddDuration(float delta)
    {
        // 남은 시간도 같이 증감
        SetDuration(DurationSeconds + delta);
        SetRemaining(RemainingSeconds + delta);
    }
}
