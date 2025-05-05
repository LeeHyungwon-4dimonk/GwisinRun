using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    Image m_hpImage;

    private int m_maxHealth;
    private int m_currentHealth;

    Color HPColor;

    void Start()
    {
        m_hpImage = GetComponent<Image>();
        m_maxHealth = PlayerHealth.MaxHealth;
        m_currentHealth = PlayerHealth.CurrentHealth;
    }

    void Update()
    {
        float toFill;
        toFill = (float)m_currentHealth / m_maxHealth;

        m_hpImage.fillAmount = toFill;
    }
}
