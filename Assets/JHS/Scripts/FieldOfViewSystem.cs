using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewSystem : MonoBehaviour
{
    public event Action<Transform>  OnTargetDetected;
    public event Action             OnTargetLost;
    public event Action<float>      OnTargetDistanceChanged; //타겟과의 거리 변화
    public event Action<Vector3>    OnLastKnownPositionUpdated; //마지막 발견 위치 업데이트



    public Vector3 LastKnownPosition {get; private set;}
    public float DistanceToTarget {get; private set;}

    public     Transform           currentTarget;

    // // 탐지 주기
    [SerializeField] private float m_DetectionDelay = 0.5f;
    [SerializeField] private float m_ViewRadius = 10f;
    [SerializeField] private float m_ViewAngle = 90f;
    [SerializeField] private LayerMask m_TargetMask;
    [SerializeField] private LayerMask m_ObstacleMask;
    
    // (250502) 데미지 변수 추가 :: S
    [Header("MonsterAbility")]
    public int MonsterPower = 20;
    // (250502) 데미지 변수 추가 :: E


    private void Start()
    {
        StartCoroutine(DetectionLoop());
    }
    //여기에서 모든 이벤트 처리를 담당한다.
    private IEnumerator DetectionLoop()
    {
        while(true)
        {
            var newTarget = FindVisibleTarget();
            
            // 타겟 발견/손실 이벤트
            if(newTarget != null && currentTarget == null)
            {
                currentTarget = newTarget;
                LastKnownPosition = newTarget.position;
                OnTargetDetected?.Invoke(newTarget);
            }
            else if(newTarget == null && currentTarget != null)
            {
                OnTargetLost?.Invoke();
                currentTarget = null;
            }
            
            // 타겟이 있을 때 추가 정보 업데이트
            if(currentTarget != null)
            {
                float newDistance = Vector3.Distance(transform.position, currentTarget.position);
                
                // 거리 변화 감지
                if(Mathf.Abs(newDistance - DistanceToTarget) > 0.5f)
                {
                    DistanceToTarget = newDistance;
                    OnTargetDistanceChanged?.Invoke(DistanceToTarget);
                }
                
                // 마지막 위치 업데이트
                if(Vector3.Distance(LastKnownPosition, currentTarget.position) > 1f)
                {
                    LastKnownPosition = currentTarget.position;
                    OnLastKnownPositionUpdated?.Invoke(LastKnownPosition);
                }
            }
            
            yield return new WaitForSeconds(m_DetectionDelay);
        }
    }
    

    private bool IsTargetVisible(Transform target)
    {
        Vector3 origin = transform.position;
        Vector3 dirToTarget = (target.position - origin);
        float sqrDistanceToTarget = dirToTarget.sqrMagnitude;

        // 시야각 체크
        float angleToTarget = Vector3.Angle(transform.forward, dirToTarget.normalized);
        if (angleToTarget > m_ViewAngle * 0.5f)
            return false;

        // 시야 거리 체크
        if (sqrDistanceToTarget > m_ViewRadius * m_ViewRadius)
            return false;

        // 장애물 체크 (Raycast)
        if (Physics.Raycast(origin, dirToTarget.normalized, out RaycastHit hit, Mathf.Sqrt(sqrDistanceToTarget), m_ObstacleMask))
        {
            // 장애물이 타겟보다 앞에 있는 경우
            if (hit.transform != target)
                return false;
        }

        return true;
    }
    private void FindVisibleTargets(ref List<Transform> visibleTargets)
    {
        visibleTargets.Clear();
        
        //반경 내의 타겟들들
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, m_ViewRadius, m_TargetMask);

        foreach (var collider in targetsInViewRadius)
        {
            Transform target = collider.transform;
            // 시야각 내에 있는지 확인
            if (IsTargetVisible(target))
            {
                visibleTargets.Add(target);
                // 플레이어를 발견했을 때 정보 저장
                // lastSeenDirection = dirToTarget;
                // lastSeenPosition = target.position;
            }
            else
            {
                // 장애물에 가려진 경우
                //lastObstaclePosition = hit.point;
            }
        }
    }
    public Transform FindVisibleTarget()
    {
        // 시야 반경 내의 타겟 검색
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, m_ViewRadius, m_TargetMask);

        foreach (var collider in targetsInViewRadius)
        {
            Transform target = collider.transform;

            if (IsTargetVisible(target))
            {
                return target;
            }
        }
        return null;
    }

    /// <summary>
    /// 각도를 방향 벡터로 변환합니다.
    /// </summary>
    /// <param name="angleInDegrees">각도(도)</param>
    /// <param name="angleIsGlobal">전역 각도 여부</param>
    /// <returns>방향 벡터</returns>
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if(!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    // 속성 접근자 추가
    public float ViewRadius => m_ViewRadius;
    public float ViewAngle => m_ViewAngle;
}
