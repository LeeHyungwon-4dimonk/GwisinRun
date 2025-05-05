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
    [Header("Movement")]
    public float rotationSpeed = 5f; // 회전 속도
    //===========================================================
    [Header("Combat Settings")]
    public float attackRange = 2.0f;
    public float attackCooldown = 2.0f;
    private bool isAttacking = false; // 공격 중 상태
    private float lastAttackTime = 0f;
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
        LookAtTarget(fieldOfViewSystem.currentTarget);
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


    public void SetAttacking(bool attacking)
    {
        isAttacking = attacking;
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }

    public bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }

    // 공격 시작 시 호출
    public void StartAttack()
    {
        lastAttackTime = Time.time;
        Debug.Log($"Monster attack started, next attack available at: {lastAttackTime + attackCooldown}");
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
        // 명확한 로그 추가
        Debug.Log($"Distance changed: {distance}, Attack range: {attackRange}, Can attack: {CanAttack()}, Current state: {FSM.CurrentState}");
        
        // 공격 가능 상태이고, 추적 중이며, 거리가 가까우면 공격
        if (!isAttacking && CanAttack() && distance <= attackRange && FSM.CurrentState == MonsterState.Chase)
        {
            Debug.Log("Transitioning to Attack state");
            FSM.ChangeState(MonsterState.Attack);
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

    public void LookAtTarget(Transform target)
    {
        if (target == null) return;
        
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0; // y축 회전만 적용
        
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                lookRotation, 
                Time.deltaTime * rotationSpeed); // 회전 속도 조절
        }
    }
}