using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputStateController : StateMachineBehaviour
{
    public bool lockMove;
    public bool lockJump;
    public bool lockEquip;
    public bool lockAttack;
    public bool lockUseSkill;
    public bool lockAll = true;

    public bool onlyLock;
    public bool keepLocking;

    private CharacterBaseController cbc;
    public bool keepLockingInput;

    public bool lockGetHit;
    public bool lockDecreaseHP;

    public bool lockRotate;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!cbc)
            cbc = animator.GetComponentInParent<CharacterBaseController>();
        if (lockDecreaseHP)
            cbc.canDecreaseHP = false;
        if (lockGetHit)
            cbc.canGetHit = false;
        if (lockRotate)
            cbc.canRotarte = false;

        LockInput();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("����������");
        if (keepLocking)
        {
            cbc.canGetPlayerInputValue = false;
        }
        if (keepLockingInput)
        {
            LockInput();
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (lockAll)
        {
            //ͨ�� animator �����õ���ǰ��Ϸ�������ϵ��κ�����Ͳ���
            cbc.UnLockAll();
        }
        if (lockMove)
            cbc.canMove = true;
        if (lockJump)
            cbc.canJump = true;
        if (lockEquip)
            cbc.canEquip = true;
        if (lockAttack)
            cbc.canAttack = true;
        if (lockUseSkill)
            cbc.canUseSkill = true;
        if (lockDecreaseHP)
            cbc.canDecreaseHP = true;
        if (lockGetHit)
            cbc.canGetHit = true;
        if (lockRotate)
            cbc.canRotarte = true;

        cbc.canGetPlayerInputValue = true;

        //Debug.Log("��ǰ�˳��Ķ���״̬����״̬�����ص���Ϸ�����ǣ�" + animator.gameObject);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}

    public void LockInput()
    {
        if (onlyLock)
            return;

        if (lockAll)
        {
            //ͨ�� animator �����õ���ǰ��Ϸ�������ϵ��κ�����Ͳ���
            cbc.canGetPlayerInputValue = false;
        }
        if (lockMove)
            cbc.canMove = false;
        if (lockJump)
            cbc.canJump = false;
        if (lockEquip)
            cbc.canEquip = false;
        if (lockAttack)
            cbc.canAttack = false;
        if (lockUseSkill)
            cbc.canUseSkill = false;

        //�����ǰ�����Ĺ�ϣֵ
        //Debug.Log("��ǰ�����״̬�ǣ�" + stateInfo.shortNameHash);
    }
}
