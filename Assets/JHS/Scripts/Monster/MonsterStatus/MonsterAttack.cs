using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterAttack : IMonsterState 
{
    private Monster monster;

    public void SetMonster(Monster monster)
    {
        this.monster = monster;
    }
    
    public IEnumerator Attack()
    {
        while (monster.GetCurrentStateInstance() == monster.GetAttackState())
        {
            if (GameManager.Instance.IsPaused || GameManager.Instance.IsCleared || GameManager.Instance.IsGameOver)
            {
                yield return null;
                continue;
            }
            
            // 타겟이 null이면 Search 상태로 전환
            if (monster.CurrentTarget == null)
            {
                monster.ChangeState(monster.GetSearchState());
                yield break;
            }
            
            float distance = Vector3.Distance(monster.transform.position, monster.CurrentTarget.position);
            if (distance > monster.attackRange * 1.1f)
            {
                monster.ChangeState(monster.GetSearchState());
                yield break;
            }

            // 애니메이션 실행
            monster.animator.SetTrigger("IsAttacking");

            // 애니메이션 클립 길이 자동 계산
            float clipLength = GetAnimationClipLength(monster.animator, monster.AttackAnimationName);

            // 타격 타이밍 계산 (예: 40% 시점에서 데미지 적용)
            float hitTime = clipLength * monster.AttackHitTimingRatio;

            GameManager.Instance.Audio.PlaySound(SoundType.GhostAttack);
            
            // 기다렸다가 데미지 적용
            yield return new WaitForSeconds(hitTime);

            // 타격 판정
            if (monster.CurrentTarget != null)
            {
                if (distance <= monster.attackRange * 1.2f && monster.CanAttack())
                {
                    IDamageable damageable = monster.CurrentTarget.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(monster.attackPower, monster.transform);
                    }

                    monster.StartAttack(); // 쿨다운 갱신
                }
            }


            // 애니메이션 전체 재생 길이만큼 대기
            float remainingTime = clipLength - hitTime;
            if (remainingTime > 0)
                yield return new WaitForSeconds(remainingTime);

            // 추가 쿨다운 대기 (원한다면 여기에 attackCooldown - clipLength 넣기)
            float wait = Mathf.Max(0, monster.attackCooldown - clipLength);
            if (wait > 0f)
                yield return new WaitForSeconds(wait);
        }
    }

    
    private float GetAnimationClipLength(Animator animator, string clipName)
    {
        if (animator.runtimeAnimatorController == null)
            return 1f; // 기본값

        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
                return clip.length;
        }

        // Debug.LogWarning($"Attack 애니메이션 '{clipName}' 찾을 수 없음. 기본값 사용.");
        return 1f; // 기본 fallback
    }


    public void OnEnter()
    {
        // Debug.Log(">> Attact 상태 진입");
        monster.SetAttacking(true);
        monster.StartAttack(); // 공격 시작 시간 기록
        
        // 애니메이션 속도 빠르게
        monster.animator.speed = 2.0f;
        
        monster.StartCustomCoroutine(Attack());
    }

    public void OnExit()
    {
        // Debug.Log("MonsterAttackState 종료");
        monster.SetAttacking(false); // 중요: 공격 상태 해제
        
        monster.StopCustomCoroutine();
    }

    public void OnUpdate()
    {
    }

}
