using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 创建人：Dugege
 * 功能说明：瓦格里AI
 * 创建时间：2023年2月4日21:42:31
 */

public class ValkyrieAI : MonoBehaviour
{
    private CharacterBaseController playerCBC;
    private CharacterBaseController cbc;
    //移动前去救援
    private bool reviveMoving;
    private Vector3 initPos;
    private float reviveTimer;

    private void Start()
    {
        playerCBC = GameObject.Find("Player").GetComponent<CharacterBaseController>();
        cbc = transform.GetComponentInParent<CharacterBaseController>();
    }

    private void Update()
    {
        cbc.ic.SetDefaultValue();
        if (reviveMoving)
        {
            ValkyrieRevive();
        }
        else
        {
            ValkyrieMove();
        }
    }

    private void ValkyrieMove()
    {
        ValkyrieLook();
        if (Vector3.Distance(cbc.transform.position, playerCBC.transform.position) > 10)
        {
            MoveBehaviour();
        }
        if (playerCBC.isDead)
        {
            reviveMoving = true;
            initPos = transform.position;
        }
    }

    private void MoveBehaviour()
    {
        ValkyrieLook();
        cbc.ic.SetInputValue(InputCode.VerticalMoveValue, 1);
        cbc.transform.position = Vector3.MoveTowards(cbc.transform.position, GetTargetPos(), Time.deltaTime * cbc.moveSpeed);
    }

    private void ValkyrieLook()
    {
        cbc.transform.LookAt(GetTargetPos());
    }

    private Vector3 GetTargetPos()
    {
        return new Vector3(playerCBC.transform.position.x, transform.position.y, playerCBC.transform.position.z);
    }

    private void ValkyrieRevive()
    {
        //玩家死亡，前去救援
        if (playerCBC.isDead)
        {
            if (Vector3.Distance(cbc.transform.position, playerCBC.transform.position) > 2)
            {
                cbc.ic.SetInputValue(InputCode.VerticalMoveValue, 1);
                cbc.transform.position = Vector3.MoveTowards(cbc.transform.position, playerCBC.transform.position, Time.deltaTime * cbc.moveSpeed);
            }
            else
            {
                if (Time.time - reviveTimer > 10)
                {
                    cbc.targetTransCBC = playerCBC;
                    cbc.targetTrans = playerCBC.transform;
                    cbc.ic.SetInputValue(InputCode.SkillsState[3], true);

                    reviveTimer = Time.time;
                }
            }
        }
        //玩家复活，移动到初始位置
        else
        {
            if (Vector3.Distance(cbc.transform.position, initPos) > 0.5f)
            {
                cbc.transform.LookAt(initPos);
                cbc.transform.position = Vector3.MoveTowards(cbc.transform.position, initPos, Time.deltaTime * cbc.moveSpeed);
                cbc.ic.SetInputValue(InputCode.VerticalMoveValue, 1);
            }
            else
            {
                reviveMoving = false;
            }
        }
    }
}
