using System.Collections;
using UnityEngine;

public class MonsterChase : IMonsterState 
{
    private Monster monster;

    public void SetMonster(Monster monster)
    {
        this.monster = monster;
    }

    private IEnumerator ChaseRoutine()
    {
        while (true)
        {
            Transform target = monster.FindVisibleTarget();
            if (target != null)
            {
                monster.navMesh.SetDestination(target.position);
                
                // OnTargetDistanceChanged 이벤트가 제대로 동작하지 않을 경우를 대비한 보조 로직
                float distanceToTarget = Vector3.Distance(monster.transform.position, target.position);
                if (distanceToTarget <= monster.attackRange && monster.CanAttack() && !monster.IsAttacking)
                {
                    // Debug.Log("ChaseRoutine detected attack range - changing to Attack state");
                    monster.ChangeState(monster.GetAttackState()); 
                    // monster.GetCurrentState = "Attack";
                    yield break;
                }
            }
            else
            {
                monster.ChangeState(monster.GetSearchState());
                // monster.GetCurrentState = "Search";
            }
            
            yield return new WaitForSeconds(monster.stateTickDelay);
        }
    }

    public void OnEnter()
    {
        // Debug.Log(">> Chase 상태 진입");
        
        // 빠른 이동 속도 적용
        monster.navMesh.speed = 7.0f;
        
        // 애니메이션 속도 빠르게
        monster.animator.speed = 2.0f;
        
        monster.StartCustomCoroutine(ChaseRoutine());
    }

    public void OnExit()
    {
        monster.StopCustomCoroutine();
    }

    public void OnUpdate()
    {
    }

}
