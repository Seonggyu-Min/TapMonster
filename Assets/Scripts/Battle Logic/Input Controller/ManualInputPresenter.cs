using System;
using UnityEngine;
using UnityEngine.UI;

public class ManualInputPresenter : IDisposable
{
    private readonly Button _manualAttackButton;
    private readonly CombatManager _combatManager;

    private bool _active;
    private const LogCategory CurrentCategory = LogCategory.UI;

    public ManualInputPresenter(Button manualAttackButton, CombatManager combatManager)
    {
        _manualAttackButton = manualAttackButton;
        _combatManager = combatManager;
    }
    public void Initialize() { /*no op*/ }
    public void Activate()
    {
        if (_active) return;
        _active = true;

        if (_manualAttackButton != null)
        {
            _manualAttackButton.onClick.AddListener(OnClick);
        }
    }

    public void Dispose()
    {
        if (!_active) return;
        _active = false;

        if (_manualAttackButton != null)
        {
            _manualAttackButton.onClick.RemoveListener(OnClick);
        }
    }

    private void OnClick()
    {
        if (_combatManager == null)
        {
            this.PrintLog("_combatManager가 null입니다.", CurrentCategory, LogType.Error);
            return;
        }

        _combatManager.TryManual();
        this.PrintLog("Manual attack", CurrentCategory);
    }
}
