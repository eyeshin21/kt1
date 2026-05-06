using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCheck : MonoBehaviour
{
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    collision.rigidbody.gameObject.SetActive(false);
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.attachedRigidbody.gameObject.SetActive(false);
    }
}
