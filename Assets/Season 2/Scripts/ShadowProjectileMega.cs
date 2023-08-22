using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//°µÓ°Ä§·¨Çò
public class ShadowProjectileMega : MonoBehaviour
{
    public float destoryTime;
    public float moveSpeed;
    private Collider col;
    private ParticleSystem[] ps;

    private void Awake()
    {
        col = GetComponent<Collider>();
        ps = GetComponentsInChildren<ParticleSystem>();
    }

    private void OnEnable()
    {
        CancelInvoke();
        col.enabled = true;
        for (int i = 0; i < ps.Length; i++)
        {
            ps[i].Play();
        }
        Invoke(nameof(DestroyEffect), destoryTime);
    }

    private void Update()
    {
        transform.Translate(transform.forward * moveSpeed * Time.deltaTime, Space.World);
    }

    private void DestroyEffect()
    {
        for (int i = 0; i < ps.Length; i++)
        {
            ps[i].Stop();
        }
        gameObject.SetActive(false);
    }
}
