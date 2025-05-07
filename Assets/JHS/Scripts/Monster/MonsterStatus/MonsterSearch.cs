using System.Collections;
using UnityEngine;

public class MonsterSearch : IMonsterState 
{
    private Monster monster;

    public void SetMonster(Monster monster)
    {
        this.monster = monster;
    }
    
    private int m_searchCount;

    private IEnumerator SearchRoutine()
    {
        // 초기화 시간 확보
        yield return new WaitForSeconds(0.2f); 
        
        while (true)
        {
            // Debug.Log(m_searchCount);
            Transform target = monster.FindVisibleTarget();
            if(target != null)
            {
                // Debug.Log("MonsterSearchState Chase 상태로 변경");
                monster.ChangeState(monster.GetChaseState());
                yield break;
            }
            else if(monster.IsArrived())
            {
                monster.MoveToRandomSearchPoint();
                ++m_searchCount;
                // Debug.Log($"Search Count: {m_searchCount}/{monster.searchMoveCount}");
                if(m_searchCount > monster.searchMoveCount)
                {
                    // Debug.Log("MonsterSearchState Patrol 상태로 변경");
                    monster.ChangeState(monster.GetPatrolState());
                    yield break; // 코루틴 명시적 종료
                }
            }
            yield return new WaitForSeconds(monster.stateTickDelay);

        }
    }

    public void OnEnter()
    {
        // Debug.Log(">> Search 상태 진입");
        m_searchCount = 0;
        
        monster.SetAttacking(false); 
        
        
        monster.MoveToRandomSearchPoint();
        
        // 빠른 이동 속도 적용
        monster.navMesh.speed = 7.0f;
        
        // 애니메이션 속도 빠르게
        monster.animator.speed = 2.0f;
        
        monster.StartCustomCoroutine(SearchRoutine());
        
        // Debug.Log("MonsterSearchState 실행됨 초기화됨됨" + m_searchCount);
    }

    public void OnExit()
    {
        // Debug.Log("MonsterSearchState 종료됨");
        monster.StopCustomCoroutine();
    }

    public void OnUpdate()
    {
    }


}
