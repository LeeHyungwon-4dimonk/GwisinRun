using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger1 : MonoBehaviour
{
    private bool m_playerDetected1 = false;
    private bool m_monsterDetected1 = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Monster")
        {
            m_monsterDetected1 = true;
        }

        if (other.gameObject.tag == "Player")
        {
            m_playerDetected1 = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Monster")
        {
            m_monsterDetected1 = false;
        }

        if (other.gameObject.tag == "Player")
        {
            m_playerDetected1 = false;            
        }        
    }

    public bool PlayerDetected()
    {
        return m_playerDetected1;
    }

    public bool MonsterDetected()
    {
        return m_monsterDetected1;
    }
}
