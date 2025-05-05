using System;
using System.Collections;
using System.Dynamic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public enum MonsterState
{
    Patrol,
    Chase,
    Search,
    Attack
}


public class MonsterPatrolState : FsmState<MonsterState>
{
    private Monster monster;

    private int currentEnemyPosition;
    float walkingPointRadius = 3; // 순찰 지점 도달 판정에 사용되는 반경

    
    public MonsterPatrolState(Monster monster) : base(MonsterState.Patrol)
    {
        this.monster = monster;
    }

    public override void OnEnter(MonsterState from, FsmMessage msg)
    {
        Debug.Log("MonsterPatrolState 시작.");
        if(monster.walkPoints.Length <= 1)
        {
            //포인트가 없으므로 추가해야합니다.
            Debug.Log("포인트가 없으므로 추가해야합니다.");
        }
        monster.StartCustomCoroutine(PatrolRoutine());
    }

    public override void OnExit(MonsterState to)
    {
        // 상태 종료 시 코루틴 정지
        monster.StopCustomCoroutine();
    }

    private void repeatPatrol()
    {
        if (Vector3.Distance(monster.walkPoints[currentEnemyPosition].transform.position, 
            monster.transform.position) < walkingPointRadius)
        {
            // 다음 순찰 지점을 무작위로 선택
            currentEnemyPosition = UnityEngine.Random.Range(0, monster.walkPoints.Length);
            // 배열의 범위를 넘어선 경우 인덱스를 초기화
            if (currentEnemyPosition >= monster.walkPoints.Length)
            {
                currentEnemyPosition = 0;
            }
        }
        // 선택된 지점으로 이동 명령을 내림
        monster.navMesh.SetDestination(monster.walkPoints[currentEnemyPosition].transform.position);
    }

    // 상태별 코루틴 로직
    private IEnumerator PatrolRoutine()
    {
        while (true)
        {
            // 타겟 감지는 FieldOfViewSystem에서 이벤트로 처리하므로 제거
            // 단순히 패트롤 로직만 유지
            if (monster.IsArrived())
            {
                repeatPatrol();
            }
            yield return new WaitForSeconds(monster.stateTickDelay);
        }
    }
}

public class MonsterSearchState : FsmState<MonsterState>
{
    private Monster monster;
    private int m_searchCount;
    
    public MonsterSearchState(Monster monster) : base(MonsterState.Search)
    {
        this.monster = monster;
    }
    public override void OnEnter(MonsterState fromState, FsmMessage msg)
    {
        m_searchCount = 0;
        monster.StartCustomCoroutine(SearchRoutine());
        Debug.Log("MonsterSearchState 실행됨 초기화됨됨" + m_searchCount);
    }

    

    public override void OnExit(MonsterState toState)
    {
        Debug.Log("MonsterSearchState 종료됨");
        monster.StopCustomCoroutine();
    }
    private IEnumerator SearchRoutine()
    {
        while (true)
        {
            Debug.Log(m_searchCount);
            Transform target = monster.fieldOfViewSystem.FindVisibleTarget();
            if(target != null)
            {
                Debug.Log("MonsterSearchState Chase 상태로 변경");
                monster.FSM.ChangeState(MonsterState.Chase);
                yield break; // 코루틴 명시적 종료
            }
            else if(monster.IsArrived())
            {
                monster.MoveToRandomSearchPoint();
                ++m_searchCount;
                Debug.Log($"Search Count: {m_searchCount}/{monster.searchMoveCount}");
                if(m_searchCount > monster.searchMoveCount)
                {
                    Debug.Log("MonsterSearchState Patrol 상태로 변경");
                    monster.FSM.ChangeState(MonsterState.Patrol);
                    yield break; // 코루틴 명시적 종료
                }
            }
            yield return new WaitForSeconds(monster.stateTickDelay);

        }
    }

}


public class MonsterChaseState : FsmState<MonsterState>
{
    private Monster monster;
    public MonsterChaseState(Monster monster) : base(MonsterState.Chase)
    {
        this.monster = monster;
    }
    public override void OnEnter(MonsterState fromState, FsmMessage msg)
    {
        monster.StartCustomCoroutine(ChaseRoutine());
    }


    public override void OnExit(MonsterState toState)
    {
        monster.StopCustomCoroutine();
    }
    private IEnumerator ChaseRoutine()
    {
        while (true)
        {
            // 직접 타겟을 찾는 대신 currentTarget 사용
            if (monster.fieldOfViewSystem.currentTarget != null)
            {
                monster.navMesh.SetDestination(monster.fieldOfViewSystem.currentTarget.position);
            }
            // 타겟 손실은 이벤트로 처리하므로 여기서 검사할 필요 없음
            yield return new WaitForSeconds(monster.stateTickDelay);
        }
    }
}


public class MonsterAttackState : FsmState<MonsterState>
{
    
    private Monster monster;
    
    public MonsterAttackState(Monster monster) : base(MonsterState.Attack)
    {
        this.monster = monster;
    }
    
    public override void OnEnter(MonsterState fromState, FsmMessage msg)
    {
        // 공격 시작 시 몬스터의 공격 중 상태 설정
        monster.SetAttacking(true);
        
        // NavMesh 이동 중지
        monster.navMesh.isStopped = true;
        
        monster.StartCustomCoroutine(Attack());
    }
    

    public override void OnExit(MonsterState toState)
    {
        monster.SetAttacking(false);

        // NavMesh 이동 재개
        monster.navMesh.isStopped = false;

        monster.StopCustomCoroutine();
    }

    private IEnumerator Attack()
    {
        Debug.Log("공격 시작");
        
        // 공격 애니메이션이나 효과를 여기에 추가
        monster.animator.SetTrigger("IsAttacking");
        // (250502) 데미지 처리 추가 :: S
        if (monster.fieldOfViewSystem.currentTarget != null)
        {
            IDamageable damageable = monster.fieldOfViewSystem.currentTarget.root.GetComponent<IDamageable>();
            if (damageable != null)
            {
                // 250503 EDIT :: S
                damageable.TakeDamage(monster.fieldOfViewSystem.MonsterPower); // 원하는 데미지 값
                // 250503 EDIT :: E
            }
        } 
        // (250502) 데미지 처리 추가 :: E
        
        yield return new WaitForSeconds(3f);
        
        Debug.Log("공격 종료");
        
        // 공격 후 상태 결정 (타겟이 있으면 추적, 없으면 탐색)
        if (monster.fieldOfViewSystem.currentTarget != null)
        {
            monster.FSM.ChangeState(MonsterState.Chase);
        }
        else
        {
            monster.FSM.ChangeState(MonsterState.Search);
        }
    }
}