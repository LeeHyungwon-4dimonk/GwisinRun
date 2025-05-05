using UnityEngine;

public class PlayerHide : MonoBehaviour
{
    private bool m_isDetected = false;
    
    public bool IsDetected => m_isDetected;

    
    //# 수정 사항(20250503) -- 시작
    private void Update()
    {
        if (CanStandUp())
        {
            Clear();
        }
        else
        {
            DetectedObjectAtHead();
        }
    }
    private bool CanStandUp()
    {
        // 플레이어 머리 위 공간 체크
        float checkRadius = 0.3f;
        Vector3 checkPosition = transform.position + Vector3.up;
        return !Physics.CheckSphere(checkPosition, checkRadius);
    }
    //# 수정 사항(20250503) -- 끝

    void OnEnable()
    {
        m_isDetected = false;
    }
    
    void DetectedObjectAtHead()
    {
        m_isDetected = true;
    }

    void Clear()
    {
        m_isDetected = false;
    }
    
}
