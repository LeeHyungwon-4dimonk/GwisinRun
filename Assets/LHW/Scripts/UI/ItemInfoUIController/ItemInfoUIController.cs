using System.Collections;
using TMPro;
using UnityEngine;

public class ItemInfoUIController : MonoBehaviour
{
    [SerializeField] Canvas m_canvasRectTransfom;
    [SerializeField] GameObject m_panel;
    [SerializeField] private TMP_Text m_popupText;
    private RectTransform m_panelpos;
    [SerializeField] private float m_detectRadius;
    [SerializeField] private LayerMask m_playerLayer;

    private Coroutine m_refreshCoroutine;
    private bool m_canOn;

    public void Awake()
    {
        m_panelpos = m_panel.GetComponent<RectTransform>();
    }

    private void UpdateTextMessage()
    {
        var interactionKey = PlayerPrefs.GetString("Interaction");
        if (string.IsNullOrEmpty(interactionKey))
            return;

        if (gameObject.CompareTag("Item"))
        {
            m_popupText.text = $"아이템을 얻으려면 [{interactionKey}]를 누르세요.";
        }

        else if (gameObject.CompareTag("InteractableObject"))
        {
            m_popupText.text = $"상호작용을 하려면 [{interactionKey}]를 누르세요.";
        }
    }


    private void OnMouseOver()
    {
        if (GameManager.Instance.IsPaused)
        {
            m_panel.SetActive(false);
        }
        else
        {
            m_canOn = true;
            if (m_refreshCoroutine != null)
                return;

            if (Physics.OverlapSphere(transform.position, m_detectRadius, m_playerLayer).Length > 0)
            {
                m_panel.SetActive(true);
                UpdateTextMessage();

                Vector3 localPos = Vector3.zero;

                RectTransform rectTransform = m_canvasRectTransfom.transform as RectTransform;
                Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);


                RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, mousePos, m_canvasRectTransfom.worldCamera, out localPos);
                m_panelpos.anchoredPosition = localPos;

                m_refreshCoroutine = StartCoroutine(RefreshTime());
            }
            else
            {
                m_panel.SetActive(false);
            }
        }
    }

    IEnumerator RefreshTime()
    {
        yield return new WaitForSeconds(0.5f);
        m_refreshCoroutine = null;
        m_panel.SetActive(m_canOn);
    }

    private void OnMouseExit()
    {
        // if (m_refreshCoroutine != null)
        //     return;
        // m_panel.SetActive(false);
        m_canOn = false;
    }

    private void OnDisable()
    {
        if (m_panel == null)
            return;
        m_panel.SetActive(false);
    }
}
