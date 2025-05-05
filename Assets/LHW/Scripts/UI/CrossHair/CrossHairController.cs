using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHairController : MonoBehaviour
{
    [SerializeField] GameObject m_crosshair;
    [SerializeField] GameObject m_itemPanel;
    private void Update()
    {
        if (GameManager.Instance.IsPaused == true)
        {
            m_crosshair.SetActive(false);
        }
        else
        {
            if (m_itemPanel.activeSelf == false)
            {
                m_crosshair.SetActive(true);
            }
            else
            {
                m_crosshair.SetActive(false);
            }
        }
    }
}
