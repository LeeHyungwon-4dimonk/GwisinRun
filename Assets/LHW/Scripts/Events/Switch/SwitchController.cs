using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class SwitchController : MonoBehaviour, IInteractable
{
    [SerializeField] List<Animator> m_objectAnimator;
    [SerializeField] GameObject m_button;
    private bool m_isPressed = false;    

    public void Interact()
    {
        for (int i = 0; i < m_objectAnimator.Count; i++)
        {
            boolAnimator(m_objectAnimator[i]);
        }

        m_isPressed = !m_isPressed;
        if (m_isPressed)
        {
            m_button.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else
        {
            m_button.GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }

    private void boolAnimator(Animator animator)
    {
        bool isOpen = animator.GetBool("IsOpen");
        isOpen = !isOpen;
        animator.SetBool("IsOpen", isOpen);
    }
}
