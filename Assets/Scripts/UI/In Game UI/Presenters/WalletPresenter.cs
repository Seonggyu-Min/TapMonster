using System;
using UnityEngine;

public class WalletPresenter : IDisposable
{
    private WalletManager _walletManager;
    private WalletView _walletView;

    private bool _activated;

    private LogCategory CurrentCategory = LogCategory.UI;


    public WalletPresenter(WalletView walletView, WalletManager walletManager)
    {
        _walletView = walletView;
        _walletManager = walletManager;
    }
    public void Initialize() { /*no op*/ }
    public void Activate()
    {
        if (_activated) return;
        _activated = true;

        _walletManager.OnCurrencyChanged += OnCurrencyChanged;
    }
    public void Dispose()
    {
        if (!_activated) return;
        _activated = false;

        _walletManager.OnCurrencyChanged -= OnCurrencyChanged;
    }

    private void OnCurrencyChanged(CurrencyId currency, BigNumber amount)
    {
        if (currency == CurrencyId.Gold)
        {
            _walletView.SetGold(amount);
        }
        else
        {
            // 추가 재화 생기면 처리
            this.PrintLog($"처리되지 않은 재화 타입입니다: {currency}", CurrentCategory, LogType.Warning);
        }
    }
}
