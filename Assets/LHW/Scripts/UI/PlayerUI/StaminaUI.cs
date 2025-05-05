using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    Image m_stImage;

    private int m_maxStamina;
    private int m_currentStamina;

    Color HPColor;

    void Awake()
    {
        m_stImage = GetComponent<Image>();
        m_maxStamina = PlayerHealth.MaxStamina;
        m_currentStamina = PlayerHealth.CurrentStamina;
    }

    void Update()
    {
        float toFill;
        toFill = (float)m_currentStamina / m_maxStamina;

        m_stImage.fillAmount = toFill;
    }
}
