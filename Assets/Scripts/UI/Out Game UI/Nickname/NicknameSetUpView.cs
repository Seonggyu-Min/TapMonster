using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NicknameSetUpView : MonoBehaviour
{
    [Header("닉네임 입력 UI")]
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Button _submitBtn;

    [Header("에러 UI")]
    [SerializeField] private GameObject _errorObj;
    [SerializeField] private TMP_Text _errorText;

    public event Action OnSubmitClicked;

    private LogCategory _currentCategory = LogCategory.UI;

    public string NicknameText => _inputField.text;


    private void Awake()
    {
        this.TryBindComponent(ref _inputField, _currentCategory);
    }

    private void OnEnable()
    {
        _errorObj.SetActive(false);
        _inputField.text = string.Empty;
    }


    public void ShowError(string error)
    {
        _errorObj.SetActive(true);
        _errorText.text = error;
    }

    public void HideError()
    {
        _errorObj.SetActive(false);
        _errorText.text = string.Empty;
    }
}
