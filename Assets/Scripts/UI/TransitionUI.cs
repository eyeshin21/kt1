using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class TransitionUI : MonoBehaviour
{
    [SerializeField] Animator anim;
    UnityEvent evt;

    public void FadeIn(UnityEvent e)
    {
        anim.Play("FadeIn", 0, 0);
        this.evt = e;
    }

    public void FadeOut()
    {
        anim.Play("FadeOut", 0, 0);
        this.evt = null;
    }

    public void EndFadeIn()
    {
        if(evt != null)
        {
            evt.Invoke();
        }
    }
}
