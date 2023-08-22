using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboTest : MonoBehaviour
{
    //获取动画组件
    private Animator anim;
    //是否开始连击
    private bool startCombo;
    //当前连击次数
    private int combo;

    private void Start()
    {
        //获取动画组件
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        //如果按下鼠标左键
        if (Input.GetMouseButtonDown(0))
        {
            //如果Combo值为0
            if (anim.GetInteger("Combo") == 0)
            {
                //将Combo值增加1
                combo++;
                //播放Attack1
                anim.SetInteger("Combo", combo);
            }

            //如果在可以连击的时间范围中
            if (startCombo)
            {
                //将Combo值增加1
                combo++;
                //如果combo值大于3
                if (combo > 3)
                {
                    ResetCombo();
                }
                //播放后续的攻击动画
                anim.SetInteger("Combo", combo);
                startCombo = false;
            }

            Debug.Log("当前连击次数：" + combo);
        }
    }

    //动画事件中 StartComboState() 和 EndComboState() 之间的时间段里可以进行连击
    private void StartComboState()
    {
        startCombo = true;
    }

    private void EndComboState()
    {
        startCombo = false;
    }

    /// <summary>
    /// 重置连击
    /// </summary>
    private void ResetCombo()
    {
        combo = 0;
        anim.SetInteger("Combo", combo);
        EndComboState();
    }
}
