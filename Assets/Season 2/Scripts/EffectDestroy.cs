using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDestroy : MonoBehaviour
{
    public float destoryTime;
    private ParticleSystem[] ps;

    private void Awake()
    {
        ps = GetComponentsInChildren<ParticleSystem>();
    }

    private void OnEnable()
    {
        CancelInvoke();
        Invoke(nameof(DestroyEffect), destoryTime);
        for (int i = 0; i < ps.Length; i++)
        {
            ps[i].Play();
        }
    }

    private void DestroyEffect()
    {
        gameObject.SetActive(false);
    }
}
