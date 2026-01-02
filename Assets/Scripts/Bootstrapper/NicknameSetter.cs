using Cysharp.Threading.Tasks;
using Firebase.Database;
using System.Threading;
using UnityEngine;

public enum NicknameSetErrorCode
{
    None,
    Success,
    AlreadyExist,
    NotAllowed,     // TODO: 비속어 등
    Empty,
    ElseError
}


public class NicknameSetter : MonoBehaviour
{
    private LogCategory _currentCategory = LogCategory.Firebase;

    public async UniTask<bool> IsSetNickname(string uid, CancellationToken ct)
    {
        var nicknameSnap = await RTDBService.Instance.GetUniTaskAsync(DBRoutes.Nickname(uid), ct);

        // 닉네임 없으면 최초 회원가입 혹은 uid는 있는데 닉네임 설정 안한 것
        if (!nicknameSnap.Exists) return false;
        else return true;
    }


    public async UniTask<NicknameSetErrorCode> TrySetNickname(string nickname, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(nickname))
        {
            return NicknameSetErrorCode.Empty;
        }

        var result = await RTDBService.Instance.RunTransactionUniTaskAsync(
            DBRoutes.NicknameStorage(nickname),
            mutable =>
            {
                if (mutable.Value != null)
                {
                    return TransactionResult.Abort();
                }

                mutable.Value = nickname;
                return TransactionResult.Success(mutable);
            },
            ct: ct
        );

        if (!result.Exists)
        {
            this.PrintLog("중복된 닉네임이 존재합니다.", _currentCategory, LogType.Log);
            return NicknameSetErrorCode.AlreadyExist;
        }

        return NicknameSetErrorCode.Success;
    }
}
