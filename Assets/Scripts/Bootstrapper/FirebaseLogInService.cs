using UnityEngine;

public class FirebaseLogInService : MonoBehaviour
{
    private const string WebClientId = "1060578439326-0mbhma8b723jga0d8ndhsk2qv50go5tt.apps.googleusercontent.com";

    [Header("에디터 로그인 (이메일 로그인)")]
    [SerializeField] private string _email;
    [SerializeField] private string _password;

    private const LogCategory CurrentCategory = LogCategory.Firebase;

  
}
