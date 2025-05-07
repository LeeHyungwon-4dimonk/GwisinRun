using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Speed Settings")]
    // 걷기 속도
    public static float WalkSpeed = 5f;
    // 뛰기 속도
    private float m_runSpeed = 10f;
    // 아이템 증가 속도
    public static float ItemSpeed = 0f;
    // 현재 속도를 걷기나 뛰기 속도를 저장
    public static float Speed;
    // 떨어지는 속도
    private float m_fallSpeed = -28.0f;
    // 전체적인 이동 속도 계산
    private Vector3 m_velocity;
    
    [Header("Player Sit Setting")]
    private float m_sitHeight = 1.0f;
    // 캐릭터 원래 높이
    [SerializeField] private float m_originalHeight;
    // 캐릭터 원래 중심 위치
    private Vector3 m_originalCenter;
    // 프레이어 크기 저장
    private Vector3 m_originalPlayerScale;
    // 앉은 상태의 스케일
    private Vector3 m_sitPlayerScale;
    // 앉은 상태인지 확인
    public bool IsSit;
    // 앉기 키 누른 상태인지 확인
    private bool m_releasedSitKey;

    [Header("Jump Settings")]
    // 점프 높이
    private float m_jumpHeight = 1.2f;

    private PlayerHealth m_playerHealth;
    private PlayerController m_playerController;
    private PlayerMoveState m_previousMoveState;
    
    // 넉백
    private Vector3 m_knockbackVelocity = Vector3.zero;
    private float m_knockbackTimer = 0f;

    void Start()
    {
        m_playerController = GetComponent<PlayerController>();
        
        m_playerController.PlayerTransform = transform;
        if (m_playerController.PlayerTransform == null)
        {
            // Debug.LogError("PlayerTransform이 초기화되지 않았습니다!");
            return;
        }
        
        // m_originalHeight = PlayerController.PlayerCont.height;
        // 일어났을때 위치
        m_originalHeight = m_playerController.PlayerCont.height;
        // 중심 위치
        m_originalCenter = m_playerController.PlayerCont.center;

        m_originalPlayerScale = m_playerController.PlayerTransform.localScale;
        m_sitPlayerScale = new Vector3(m_originalPlayerScale.x, m_originalPlayerScale.y * 0.5f, m_originalPlayerScale.z);

        m_playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (GameManager.Instance.IsPaused || GameManager.Instance.IsCleared || GameManager.Instance.IsGameOver)
        {
            m_playerController.PlayerCont.Move(Vector3.zero);
            return;
        }
        Move();
        Jump();
        SitPlayer();
    }
    
    public void ApplyKnockback(Vector3 direction, float power, float duration)
    {
        m_knockbackVelocity = direction.normalized * power;
        m_knockbackTimer = duration;
    }

    void Move()
    {
        if (m_knockbackTimer > 0f)
        {
            m_knockbackTimer -= Time.deltaTime;

            // 넉백 처리 (CharacterController 사용)
            m_playerController.PlayerCont.Move(m_knockbackVelocity * Time.deltaTime);

            return; // 이동 중단
        }
        
        Vector2 moveInput = GameManager.Instance.Input.MoveInput;
        float moveX = moveInput.x;
        float moveZ = moveInput.y;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        if (GameManager.Instance.Input.RunKeyBeingHeld)
        {
            Speed = m_runSpeed;
            if (moveX != 0 || moveZ != 0) m_playerHealth.StaminaMinus();
            else m_playerHealth.StaminaPlus();
        }
        else
        {
            Speed = WalkSpeed;
            m_playerHealth.StaminaPlus();
        }
        
        if (move == Vector3.zero)
        {
            if(m_previousMoveState != PlayerMoveState.Idle)
            {
                if(m_previousMoveState == PlayerMoveState.Run)
                    GameManager.Instance.Graphics.IdleAndWalkDistortion();
                
                m_previousMoveState = PlayerMoveState.Idle;
            }
        }
        else if (Speed == m_runSpeed)
        {
            if (m_previousMoveState != PlayerMoveState.Run)
            {
                m_previousMoveState = PlayerMoveState.Run;
                GameManager.Instance.Graphics.RunDistortion();
            }
            
        }
        else if (Speed == WalkSpeed)
        {
            if (m_previousMoveState != PlayerMoveState.Walk)
            {
                if(m_previousMoveState == PlayerMoveState.Run)
                    GameManager.Instance.Graphics.IdleAndWalkDistortion();
                
                m_previousMoveState = PlayerMoveState.Walk;
            }
        }
        
        GameManager.Instance.Audio.PlayMoveSound(m_previousMoveState);

        m_playerController.PlayerCont.Move( (Speed + ItemSpeed) * Time.deltaTime * move);
        
        m_velocity.y += m_fallSpeed * Time.deltaTime;
        m_playerController.PlayerCont.Move(m_velocity * Time.deltaTime);

        if (m_playerController.PlayerCont.isGrounded && m_velocity.y < 0)
            m_velocity.y = -2f;
    }

    void Jump()
    {
        if (GameManager.Instance.Input.JumpKeyPressed && m_playerController.PlayerCont.isGrounded)
        {
            m_velocity.y = Mathf.Sqrt(m_jumpHeight * -2f * m_fallSpeed);
        }
    }

    void SitPlayer()
    {
        // 앉기 키가 눌려있는지 확인
        bool sitKeyHeld = GameManager.Instance.Input.SitKeyBeingHeld;

        if (GameManager.Instance.Input.SitKeyReleased)
            m_releasedSitKey = true;

        // 키가 눌려있거나, 이미 앉은 상태인데 천장이 감지되면 계속 앉은 상태 유지
        if (sitKeyHeld || (IsSit && m_playerController.HeadTriggerObject.IsDetected))
        {
            IsSit = true;
        }
        // 키를 뗐고, 천장도 감지되지 않으면 일어남
        else if (m_releasedSitKey && !m_playerController.HeadTriggerObject.IsDetected)
        {
            IsSit = false;
            m_releasedSitKey = false;
        }

        // 목표 높이와 중심 위치, 스케일 설정
        float targetHeight = IsSit ? m_sitHeight : m_originalHeight;
        
        // 캐릭터 앉아 있는 상태 계산 아니면 일어서기
        // Vector3 targetCenter = IsSit ? new Vector3(0, m_originalCenter.y - (m_originalHeight - m_sitHeight) / 3f , 0) : m_originalCenter;
        Vector3 targetCenter = IsSit ? new Vector3(0, m_originalCenter.y - (m_originalHeight - m_sitHeight) / 6f , 0) : m_originalCenter;

        Vector3 targetScale = IsSit ? m_sitPlayerScale : m_originalPlayerScale;

        // Lerp를 통해 부드럽게 보간 처리 (Time.deltaTime * 10f → 속도 조절용)
        m_playerController.PlayerCont.height = Mathf.Lerp(m_playerController.PlayerCont.height, targetHeight, Time.deltaTime * 10f);
        m_playerController.PlayerCont.center = Vector3.Lerp(m_playerController.PlayerCont.center, targetCenter, Time.deltaTime * 10f);
        m_playerController.PlayerTransform.localScale = Vector3.Lerp(m_playerController.PlayerTransform.localScale, targetScale, Time.deltaTime * 10f);;
    }
    
}
