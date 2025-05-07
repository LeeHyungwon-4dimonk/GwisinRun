using UnityEngine;

public class PlayerHide : MonoBehaviour
{
    public enum HideType
    {
        None,
        CoverObject,  // 책상 같은 오브젝트 뒤 숨음
        SitOnly       // 단순히 앉은 상태
    }
    
    private bool isInCoverZone = false; 
    private HideType currentHideType = HideType.None;

    [SerializeField] private Renderer[] m_renderers;
    
    private bool m_isDetected = false;

    
    public bool IsDetected => m_isDetected;

    void Start()
    {
        if (m_renderers == null || m_renderers.Length == 0)
            m_renderers = GetComponentsInChildren<Renderer>();
    } 
    
    private void Update()
    {
        if (GameManager.Instance.Input.InteractionKeyPressed)
        {
            if (m_isDetected)
                Unhide();
            else
                Hide();
        }
        
        if (CanStandUp())
        {
            Clear();
        }
        else
        {
            DetectedObjectAtHead();
        }
    }
    
    public bool IsTrulyHiding()
    {
        return currentHideType == HideType.CoverObject;
    }

    // 예시로 책상에 들어갈 때 호출
    public void EnterCover()
    {
        currentHideType = HideType.CoverObject;
    }

    // 책상에서 나왔을 때
    public void ExitCover()
    {
        currentHideType = HideType.None;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("HideObject"))
        {
            isInCoverZone = false;
            // Debug.Log("⏹ 책상 벗어남: ExitCover");
            ExitCover();
        }
    }
    
    void Hide()
    {
        m_isDetected = true;
        // SetRenderers(false);
        // Debug.Log("플레이어가 숨었습니다.");
    }

    void Unhide()
    {
        m_isDetected = false;
        // SetRenderers(true);
        // Debug.Log("플레이어가 다시 나타났습니다.");
    }

    private bool CanStandUp()
    {
        // 플레이어 머리 위 공간 체크
        float checkRadius = 0.6f;
        Vector3 checkPosition = transform.position + Vector3.up;
        return !Physics.CheckSphere(checkPosition, checkRadius);
    }

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
