using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    private void Start()
    {
        gameObject.tag = transform.root.tag;
        gameObject.layer = transform.root.gameObject.layer;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            gameObject.SetActive(false);
        }
    }
}
