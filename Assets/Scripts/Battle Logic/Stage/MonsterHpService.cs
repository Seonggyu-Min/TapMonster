using System;

public class MonsterHpService
{
    public readonly MonsterHpModel _hPModel;

    public event Action<BigNumber> OnDamaged
    {
        add => _hPModel.OnDamaged += value;
        remove => _hPModel.OnDamaged -= value;
    }

    public event Action OnDied
    {
        add => _hPModel.OnDied += value;
        remove => _hPModel.OnDied -= value;
    }


    public MonsterHpService(MonsterHpModel model)
    {
        _hPModel = model;
    }

    public BigNumber CurrentHp => _hPModel.CurrentHp;
    public BigNumber MaxHp => _hPModel.MaxHp;
    public bool HasLoadedValue => _hPModel.HasLoadedValue;
    public bool IsDead => _hPModel.IsDead;

    public void SetHpSilently(BigNumber maxHp, BigNumber currentHp)
        => _hPModel.SetSilently(maxHp, currentHp);
    public void InitializeHp(BigNumber maxHp)
        => _hPModel.Initialize(maxHp);
    public BigNumber ApplyDamage(BigNumber finalDamage)
        => _hPModel.ApplyDamage(finalDamage);
    public void SetLoadedFlag(bool loaded)
        => _hPModel.SetLoadedFlag(loaded);
}
