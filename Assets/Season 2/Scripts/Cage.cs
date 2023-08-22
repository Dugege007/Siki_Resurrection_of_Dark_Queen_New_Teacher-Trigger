using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 创建人：Dugege
 * 功能说明：困瓦格里的笼子
 * 创建时间：2023年1月27日23:05:12
 */

public class Cage : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = transform.Find("ValkyrieStatic").GetComponent<Animator>();
    }

    private void Update()
    {

    }

    public void UnLockValkyrie()
    {
        animator.CrossFade("IdleA", 0.1f);
        animator.transform.SetParent(null);
        Destroy(gameObject);
    }
}
