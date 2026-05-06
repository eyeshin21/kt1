using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnim : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] string anim;

    // Start is called before the first frame update
    void Start()
    {
        animator.gameObject.SetActive(false);
        Invoke(nameof(PlayTutorial), 0.5f);
    }

    void PlayTutorial()
    {
        animator.gameObject.SetActive(true);
        animator.Play(anim, 0, 0);
    }
}
