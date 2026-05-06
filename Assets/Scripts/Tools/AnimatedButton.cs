using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AnimatedButton : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [Serializable]
    public class ButtonClickedEvent : UnityEvent { }

    public bool interactable = true;

    [SerializeField]
    private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();
    [SerializeField]
    private Transform tf;

    private bool isCLicked;

    public ButtonClickedEvent onClick
    {
        get { return m_OnClick; }
        set { m_OnClick = value; }
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        if (tf == null)
        {
            tf = transform;
        }
    }
#endif 

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || !interactable)
            return;

        if (tf != null && !isCLicked)
        {
            tf.localScale *= 0.9f;
        }

        isCLicked = true;
    }

    private void Press()
    {
        if (!IsActive())
            return;
        OnClickAction();
    }

    private void OnClickAction()
    {
        m_OnClick.Invoke();
        SoundManager.instance.PlaySound("Button");
        HapticFeedbackController.TriggerHaptics(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!interactable)
        {
            return;
        }

        if (tf != null && isCLicked)
        {
            tf.localScale /= 0.9f;
        }
        isCLicked = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!interactable)
        {
            return;
        }
        Press();
    }
}