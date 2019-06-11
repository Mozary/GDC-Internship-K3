using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractOnButton2D : InteractOnTrigger2D
{
    public UnityEvent OnButtonPress;

    bool m_CanExecuteButtons;

    protected override void ExecuteOnEnter(Collider2D other)
    {
        m_CanExecuteButtons = true;
        OnEnter.Invoke();
    }

    protected override void ExecuteOnExit(Collider2D other)
    {
        m_CanExecuteButtons = false;
        OnExit.Invoke();
    }

    void Update()
    {
        if (m_CanExecuteButtons)
        {
            if (Input.GetKeyDown("e"))
                OnButtonPress.Invoke();
        }
    }
}
