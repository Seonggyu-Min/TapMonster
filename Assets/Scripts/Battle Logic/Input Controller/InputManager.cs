using System;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Button _manualAttackButton;
    // TODO: 스킬 버튼 List

    public event Action OnManualAttack;


    private void OnEnable()
    {
        HandleSubscribe(true);
    }

    private void OnDisable()
    {
        HandleSubscribe(false);
    }


    private void HandleSubscribe(bool willSubscribe)
    {
        if (willSubscribe)
        {
            _manualAttackButton.onClick.AddListener(InvokeManualAttack);
        }
        else
        {
            _manualAttackButton.onClick.RemoveListener(InvokeManualAttack);
        }
    }


    private void InvokeManualAttack()
    {
        this.PrintLog("이벤트 발생");
        OnManualAttack?.Invoke();
    }
}
