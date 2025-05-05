using UnityEngine;

public class DeskDrawerController : MonoBehaviour, IInteractable
{
    private Animator m_animator;
    // private Material[] mat = new Material[2];
    // private bool isDetected = false;
    // [SerializeField] private Material m_material1;
    // [SerializeField] private Material m_material2;

    void Awake()
    {
        m_animator = GetComponent<Animator>();
        // mat[0] = m_material1;
        // mat[1] = m_material2;
    }

    private void Update()
    {
        //ChangeMat(isDetected);
    }

    public void Interact()
    {
        DrawerAnimator(m_animator);
    }

    private void DrawerAnimator(Animator animator)
    {
        bool isOpen = animator.GetBool("IsOpen");
        isOpen = !isOpen;
        animator.SetBool("IsOpen", isOpen);
    }

    // 아웃라인 셰이더 관련 오류로 우선 주석 처리 (에셋상의 오류로 추정)
    /*
    private void ChangeMat(bool isDetected)
    {
        if(isDetected)
        {
            gameObject.GetComponent<MeshRenderer>().material = mat[1];
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().material = mat[0];
        }
    }

    public bool OutlineOn()
    {
        isDetected = true;
        return isDetected;
    }

    public bool OutlineOff()
    {
        isDetected = false;
        return false;
    }
    */
}
