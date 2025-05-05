using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerIneration : MonoBehaviour
{
    [Header("RayCast Distance")]
    private float m_raycastDistance = 1.8f;

    [FormerlySerializedAs("m_dropPosition")]
    [Header("Drop Position")]
    [SerializeField] private Transform m_dropTransform;
    [SerializeField] private float m_dropForce = 5f;
    
    private Coroutine m_interactionCoroutine;
    private GameObject m_detectedObject;
    
    //# 수정 사항(20250503) -- 시작
    [SerializeField] GameObject m_panel;
    [SerializeField] private TMP_Text m_popupText;
    //# 수정 사항(20250503) -- 끝

    
    void Update()
    {
        //# 수정 사항(20250502) -- 시작
        if (GameManager.Instance.IsPaused || GameManager.Instance.IsCleared || GameManager.Instance.IsGameOver)
            return;
        //# 수정 사항(20250502) -- 끝
        
        //# 수정 사항(20250503) -- 시작
        DetectInteractableObjectByRay();
        
        DisplayInteractableObjectUI();
        
        if (m_detectedObject != null && GameManager.Instance.Input.InteractionKeyPressed)
        {
            InteractWithObject();
        }

        if (GameManager.Instance.Input.DropKeyPressed)
        {
            DropItem();
        }
        //# 수정 사항(20250503) -- 끝
    }
    
    //# 수정 사항(20250503) -- 시작
    private void DisplayInteractableObjectUI()
    {
        if (m_detectedObject == null)
        {
            m_panel.SetActive(false);
            return;
        }
        
        var interactionKey = PlayerPrefs.GetString("Interaction");
        if (m_detectedObject.CompareTag("Item"))
        {
            m_popupText.text = $"아이템을 얻으려면 [{interactionKey}]를 누르세요.";
            m_panel.SetActive(true);
        }

        else if (m_detectedObject.CompareTag("InteractableObject"))
        {
            m_popupText.text = $"상호작용을 하려면 [{interactionKey}]를 누르세요.";
            m_panel.SetActive(true);
        }
        
    }

    void InteractWithObject()
    {
        IInteractable interactableObject = m_detectedObject.gameObject.GetComponent<IInteractable>();

        if (interactableObject == null)
            return;
        
        if (m_interactionCoroutine == null)
        {
            interactableObject.Interact();
            m_interactionCoroutine = StartCoroutine(InteractionDelay());
        }
    }
    //# 수정 사항(20250503) -- 끝

    void DetectInteractableObjectByRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Debug.DrawRay(ray.origin, ray.direction * m_raycastDistance, Color.red, 0.1f);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            if (hitInfo.distance > m_raycastDistance)
            {
                m_detectedObject = null;
                return;
            }
            
            m_detectedObject = hitInfo.collider.gameObject;
        }
    }

    private void DropItem()
    {
        Debug.Log($"Forward : {transform.forward}");
        GameManager.Instance.Inventory.DropItem(m_dropTransform.position, transform.forward, m_dropForce);
        
    }
    
    IEnumerator InteractionDelay()
    {
        yield return new WaitForSeconds(0.1f);
        m_interactionCoroutine = null;
    }

}
