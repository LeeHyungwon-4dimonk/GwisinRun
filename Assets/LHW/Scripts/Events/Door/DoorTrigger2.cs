using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger2 : MonoBehaviour
{
    private bool m_playerDetected2 = false;
    private bool m_monsterDetected2 = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            m_playerDetected2 = true;
        }
        if (other.gameObject.tag == "Monster")
        {
            m_monsterDetected2 = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            m_playerDetected2 = false;
        }
        if (other.gameObject.tag == "Monster")
        {
            m_monsterDetected2 = false;
        }
    }
    public bool PlayerDetected()
    {
        return m_playerDetected2;
    }
    public bool MonsterDetected()
    {
        return m_monsterDetected2;
    }
}
