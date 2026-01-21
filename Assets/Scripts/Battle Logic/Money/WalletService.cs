using System;
using UnityEngine;

public enum SpendResult
{
    Success,
    NotEnough,
    InvalidAmount
}

public class WalletService
{
    private WalletModel _walletModel;

    private const LogCategory CurrentCategory = LogCategory.GameLogic;


    public event Action<CurrencyId, BigNumber> OnCurrencyChanged
    {
        add => _walletModel.OnCurrencyChanged += value;
        remove => _walletModel.OnCurrencyChanged -= value;
    }

    public WalletService(WalletModel walletModel)
    {
        _walletModel = walletModel;
    }

    public BigNumber Get(CurrencyId id) => _walletModel.Get(id);

    public void Earn(CurrencyId id, BigNumber amount)
    {
        if (amount.Mantissa <= 0) return; // 0 이하의 수익은 무시
        _walletModel.Set(id, _walletModel.Get(id) + amount);
    }

    public SpendResult TrySpend(CurrencyId id, BigNumber cost)
    {
        if (cost.Mantissa <= 0) return SpendResult.InvalidAmount;

        BigNumber cur = _walletModel.Get(id);
        if (cur < cost) return SpendResult.NotEnough;

        _walletModel.Set(id, cur - cost);
        return SpendResult.Success;
    }

    public bool CanPay(CurrencyId id, BigNumber cost)
    {
        // 디버그용
        this.PrintLog($"cost.Mantissa >= 0 ? {cost.Mantissa >= 0} / " +
            $"_walletModel.Get(id) >= cost ? {_walletModel.Get(id) >= cost} / " +
            $"Current Balance: {BigNumberFormatter.ToString(_walletModel.Get(id))} / " +
            $"CurrencyId: {id} / " +
            $"Cost: {BigNumberFormatter.ToString(cost)}", CurrentCategory, LogType.Log);

        return cost.Mantissa >= 0 && _walletModel.Get(id) >= cost;
    }
}
