using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [Header("Flickering Settings")]
    [SerializeField] private float m_flickeringDuration;
    private float m_lightIntensityMin;
    private float m_lightIntensityMax;
    private float m_flickeringDurationMin;
    private float m_flickeringDurationMax;
    private float m_currentFlickeringDuration;
    private float m_timer;
    private float m_flickerLightIntensity;
    
    [Header("Component Settings")]
    [SerializeField] private Light m_flickeringLight;
    
    void Start()
    {
        m_lightIntensityMin = m_flickeringLight.intensity * 0.95f;
        m_lightIntensityMax = m_flickeringLight.intensity * 1.05f;

        m_flickeringDurationMin = m_flickeringDuration * 0.9f;
        m_flickeringDurationMax = m_flickeringDuration * 1.5f;
        
        ChangeFlickeringDuration();
    }


    void Update()
    {
        m_timer += Time.deltaTime;

        if (m_timer >= m_currentFlickeringDuration)
        {
            ChangeLightIntensity();
        }
        m_flickeringLight.intensity = m_flickerLightIntensity;
    }

    private void ChangeLightIntensity()
    {
        m_flickerLightIntensity = Random.Range(m_lightIntensityMin, m_lightIntensityMax);
        ChangeFlickeringDuration();
        m_timer = 0f;
    }

    private void ChangeFlickeringDuration()
    {
        m_currentFlickeringDuration = Random.Range(m_flickeringDurationMin, m_flickeringDurationMax);
    }
    
}
