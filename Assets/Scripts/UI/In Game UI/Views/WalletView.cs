using TMPro;
using UnityEngine;

public class WalletView : MonoBehaviour
{
    [SerializeField] private TMP_Text _walletText;

    public void SetGold(BigNumber amount)
    {
        _walletText.text = $"Gold: {BigNumberFormatter.ToString(amount)}";
    }
}
