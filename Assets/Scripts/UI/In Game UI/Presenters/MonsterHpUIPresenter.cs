using System;
using UnityEngine;

public class MonsterHpUIPresenter : IDisposable
{
    private readonly MonsterHpView _view;
    private readonly StageManager _stageManager;

    private bool _active;

    private const LogCategory CurrentCategory = LogCategory.UI;

    public MonsterHpUIPresenter(MonsterHpView view, StageManager stageManager)
    {
        _view = view;
        _stageManager = stageManager;
    }
    public void Initialize() { /*no op*/ }
    public void Activate()
    {
        if (_active) return;
        _active = true;

        _stageManager.OnDamaged += HandleChanged;
        _stageManager.OnDied += HandleChanged;
        _stageManager.OnTargetChanged += HandleChanged;
    }
    public void Dispose()
    {
        if (!_active) return;
        _active = false;

        _stageManager.OnDamaged -= HandleChanged;
        _stageManager.OnDied -= HandleChanged;
        _stageManager.OnTargetChanged -= HandleChanged;
    }

    private void HandleChanged(BigNumber _)
    {
        Refresh();
    }

    private void HandleChanged()
    {
        Refresh();
    }

    private void Refresh()
    {
        if (_view == null || _stageManager == null) return;

        IDamageable target = _stageManager.CurrentTarget;
        if (target == null)
        {
            this.PrintLog("target이 null입니다.", CurrentCategory, LogType.Error);

            _view.SetNormalized(0f);
            _view.SetText("0 / 0");
            return;
        }

        BigNumber cur = target.CurrentHp;
        BigNumber max = target.MaxHp;

        float t01 = BigNumber.Ratio01(cur, max);

        _view.SetNormalized(t01);
        this.PrintLog($"cur: {BigNumberFormatter.ToString(cur)} / max: {BigNumberFormatter.ToString(max)}", CurrentCategory);
        _view.SetText($"{BigNumberFormatter.ToString(cur)} / {BigNumberFormatter.ToString(max)}");
    }
}
