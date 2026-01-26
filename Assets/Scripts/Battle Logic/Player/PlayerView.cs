using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    

    private const LogCategory CurrentCategory = LogCategory.Animation;


    //// 수동 공격 연출
    //public void PlayManualAttackAnimation(AnimationKey key)
    //{
    //    Play(key);
    //}

    //// 스킬 연출
    //public void PlaySkillAnimation(AnimationKey key)
    //{
    //    Play(key);
    //}

    public void PlayAnimation(AnimationKey key)
    {
        int hash = AnimationHashes.Get(key);
        if (hash == 0)
        {
            this.PrintLog($"애니메이션을 찾을 수 없습니다 key: {key}", CurrentCategory, LogType.Warning);
            return;
        }

        _animator.Play(hash, 0, 0f);
    }
}
