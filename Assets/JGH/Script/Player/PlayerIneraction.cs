using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements.Experimental;

public class PlayerIneraction : MonoBehaviour
{
    [Header("RayCast Distance")]
    private float m_raycastDistance = 2.3f;

    [FormerlySerializedAs("m_dropPosition")]
    [Header("Drop Position")]
    [SerializeField] private Transform m_dropTransform;
    [SerializeField] private float m_dropForce = 5f;
    
    private Coroutine m_interactionCoroutine;
    private GameObject m_detectedObject;
    private GameObject m_previousDetectedObject;
    
    [Header("Interaction UI")]
    [SerializeField] GameObject m_ineractionPanel;
    [SerializeField] private TMP_Text m_interactionPopupText;

    
    void Update()
    {
        if (GameManager.Instance.IsPaused || GameManager.Instance.IsCleared || GameManager.Instance.IsGameOver)
            return;
        
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
    }
    
    private void DisplayInteractableObjectUI()
    {
        if (m_detectedObject == null )
        {
            if (m_previousDetectedObject != null && m_previousDetectedObject.GetComponent<OutlineController>() != null)
            {
                m_previousDetectedObject.GetComponent<OutlineController>().OutlineOff();
            }
            m_ineractionPanel.SetActive(false);
            return;
        }
        
        var interactionKey = PlayerPrefs.GetString("Interaction");
        if (m_detectedObject.CompareTag("Item"))
        {
            if (m_detectedObject.GetComponent<OutlineController>() != null)
            {
                m_detectedObject.GetComponent<OutlineController>().OutlineOn();
            }
            m_interactionPopupText.text = $"아이템을 얻으려면 [{interactionKey}]를 누르세요.";
            m_ineractionPanel.SetActive(true);
            m_previousDetectedObject = m_detectedObject;            
        }

        else if (m_detectedObject.CompareTag("InteractableObject"))
        {
            if (m_detectedObject.GetComponent<IsLockedDoor>() != null && m_detectedObject.GetComponent<IsLockedDoor>().IsLocked())
            {
                if(GameManager.Instance.Inventory.FindKey())
                    m_interactionPopupText.text = $"열쇠를 보유하고 있습니다.\n문을 열려면 [{interactionKey}]를 누르세요.";
                else
                    m_interactionPopupText.text = "잠긴 문입니다. 열쇠를 찾아주세요.";
                m_ineractionPanel.SetActive(true);
            }
            else
            {
                if(m_detectedObject.GetComponent<OutlineController>() != null)
                {
                    m_detectedObject.GetComponent<OutlineController>().OutlineOn();
                }
                m_interactionPopupText.text = $"상호작용을 하려면 [{interactionKey}]를 누르세요.";
                m_ineractionPanel.SetActive(true);
            }
            m_previousDetectedObject = m_detectedObject;
        }
        else
        {
            if (m_previousDetectedObject != null && m_previousDetectedObject.GetComponent<OutlineController>() != null)
            {
                m_previousDetectedObject.GetComponent<OutlineController>().OutlineOff();
            }
            m_ineractionPanel.SetActive(false);
            m_previousDetectedObject = m_detectedObject;
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
        // Debug.Log($"Forward : {transform.forward}");
        GameManager.Instance.Inventory.DropItem(m_dropTransform.position, transform.forward, m_dropForce);
        
    }
    
    IEnumerator InteractionDelay()
    {
        yield return new WaitForSeconds(0.1f);
        m_interactionCoroutine = null;
    }

}
