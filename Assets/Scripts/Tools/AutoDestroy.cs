using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] float delay = 2;
    // Start is called before the first frame update
    void OnEnable()
    {
        Invoke(nameof(DestroyObj), delay);
    }

    void DestroyObj()
    {
        gameObject.Recycle();
    }
}
