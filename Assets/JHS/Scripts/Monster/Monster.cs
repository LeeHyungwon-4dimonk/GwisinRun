using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
public class Monster : MonoBehaviour
{
    
    public Animator animator;
    [Header ("MonsterPatrolState")]
    public GameObject[] walkPoints;             // 순찰할 위치들을 담은 배열

    [Header ("MonsterSearchState")]
    public float    searchRadius        = 5f;       //탐색 반경
    public int      searchMoveCount     = 3;        //탐색 횟수

    [Header("Behavior Timing")]
    public float    stateTickDelay = 0.2f;

    [Header("References")]
    public FieldOfViewSystem    fieldOfViewSystem;
    public NavMeshAgent         navMesh;

    //===========================================================
    public  StateMachine<MonsterState> FSM { get; private set; }
    private Coroutine           customCoroutine;
    private void Awake()
    {
        animator = GetComponent<Animator>();

        FSM = new StateMachine<MonsterState>();
        FSM.AddState(new MonsterPatrolState(this));
        FSM.AddState(new MonsterChaseState(this));
        FSM.AddState(new MonsterSearchState(this));
        FSM.AddState(new MonsterAttackState(this));
        FSM.ChangeState(MonsterState.Patrol);
        // 이벤트 구독
        RegisterEvents();
    }

    private void Update()
    {
        if (GameManager.Instance.IsPaused || GameManager.Instance.IsCleared || GameManager.Instance.IsGameOver)
        {
            navMesh.isStopped = true;
        }
        else
        {
            navMesh.isStopped = false;
        }
    }

    private void RegisterEvents()
    {
        // 기존 이벤트 구독
        fieldOfViewSystem.OnTargetDetected += HandleTargetDetected;
        fieldOfViewSystem.OnTargetLost += HandleTargetLost;
        
        // 새 이벤트 구독
        fieldOfViewSystem.OnTargetDistanceChanged += HandleTargetDistanceChanged;
        fieldOfViewSystem.OnLastKnownPositionUpdated += HandleLastKnownPositionUpdated;
    }
    private bool isAttacking = false; // 공격 중 상태

    public void SetAttacking(bool attacking)
    {
        isAttacking = attacking;
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }
    private void HandleTargetDetected(Transform target)
    {

        // 공격 중에는 다른 상태로 전환되지 않도록 막음
        if (isAttacking) return;

        switch(FSM.CurrentState)
        {
            case MonsterState.Patrol:
                FSM.ChangeState(MonsterState.Chase);
                break;
            case MonsterState.Search:
                FSM.ChangeState(MonsterState.Chase);
                break;
        // 이미 추적 중이거나 공격 중이면 상태 변경 필요 없음
        }

    }

    private void HandleTargetLost()
    {
        // 공격 중에는 다른 상태로 전환되지 않도록 막음
        if (isAttacking) return;

        // 추적 중일 때만 탐색 상태로 전환
        if (FSM.CurrentState == MonsterState.Chase)
        {
            FSM.ChangeState(MonsterState.Search);
        }
    }
    
    private void HandleTargetDistanceChanged(float distance)
    {
        // 거리에 따른 공격 판단
        const float attackRange = 2.0f; // 적절한 공격 범위 설정
        
        if (!isAttacking && distance < attackRange && FSM.CurrentState == MonsterState.Chase)
        {
            FSM.ChangeState(MonsterState.Attack);
        }
        else if (!isAttacking && distance > attackRange && FSM.CurrentState == MonsterState.Attack)
        {
            FSM.ChangeState(MonsterState.Chase);
        }
    }
    
    private void HandleLastKnownPositionUpdated(Vector3 position)
    {
        // 탐색 시 마지막 위치 정보 활용
        if (FSM.CurrentState == MonsterState.Search)
        {
            navMesh.SetDestination(position);
        }
    }

    #region Coroutine Control
    public void StartCustomCoroutine(IEnumerator routine)
    {
        StopCustomCoroutine();
        customCoroutine = StartCoroutine(routine);
    }

    public void StopCustomCoroutine()
    {
        if (customCoroutine != null)
        {
            StopCoroutine(customCoroutine);
            customCoroutine = null;
        }
    }
    #endregion
    // public void Update()
    // {
    //     FSM.Update(Time.deltaTime);
    // }
    // 상태에서 코루틴 정지 요청
    public bool IsArrived()
    {
        return !navMesh.pathPending &&
            navMesh.remainingDistance <= navMesh.stoppingDistance &&
            (!navMesh.hasPath || navMesh.velocity.sqrMagnitude == 0f);
    }


    public void MoveToRandomSearchPoint()
    {
        Vector3 forwardDir = transform.forward;
        Vector3 randomOffset = Random.insideUnitSphere * searchRadius;
        randomOffset.y = 0;

        Vector3 biasedDirection = forwardDir * 2f + randomOffset;
        Vector3 randomPoint = transform.position + biasedDirection; //lastKnownPosition

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, searchRadius, NavMesh.AllAreas))
        {
            navMesh.SetDestination(hit.position);
        }
    }
}