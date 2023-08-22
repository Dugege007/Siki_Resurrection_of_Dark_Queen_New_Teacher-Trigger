using System;
using System.Collections.Generic;
using UnityEngine;

//刺客
public class Assassin : Role
{
    private GameObject unEquipLeftDaggerGO;
    private GameObject unEquipRightDaggerGO;
    private GameObject leftDaggerGO;
    private GameObject rightDaggerGO;

    public Transform targetTrans;
    public bool startMatch;

    protected override void Awake()
    {
        base.Awake();
        unEquipLeftDaggerGO = CharacterBaseController.DeepFindChild(transform, "UnEquipDaggerLeft").gameObject;
        unEquipRightDaggerGO = CharacterBaseController.DeepFindChild(transform, "UnEquipDaggerRight").gameObject;
        leftDaggerGO = CharacterBaseController.DeepFindChild(transform, "DaggerLeft").gameObject;
        rightDaggerGO = CharacterBaseController.DeepFindChild(transform, "DaggerRight").gameObject;
    }

    private void OnEnable()
    {
        ShowOrHideUnEquipDagger(0, true);
        ShowOrHideUnEquipDagger(1, true);
    }

    protected override void Start()
    {
        base.Start();
        animator = cbc.animators[(int)State.Assassin];
    }

    private void Update()
    {
        if (startMatch)
        {
            // TransformPoint() 局部坐标转世界坐标
            animator.MatchTarget(cbc.executePartTrans.TransformPoint(
                cbc.matchTargetParameterList[cbc.matchIndex].offset.x * Vector3.right +
                cbc.matchTargetParameterList[cbc.matchIndex].offset.y * Vector3.up +
                cbc.matchTargetParameterList[cbc.matchIndex].offset.z * Vector3.forward
                ),
                cbc.executePartTrans.rotation,
                cbc.matchTargetParameterList[cbc.matchIndex].executeMan,
                new MatchTargetWeightMask(Vector3.one, 1),
                cbc.matchTargetParameterList[cbc.matchIndex].startTime,
                cbc.matchTargetParameterList[cbc.matchIndex].targetTime);
        }
    }

    protected override void OnAnimatorMove()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("ExecutionState"))
        {
            cbc.transform.position += new Vector3(animator.deltaPosition.x, 0, animator.deltaPosition.z);
            if (!cbc.matchTargetParameterList[cbc.matchIndex].lockRotateXZ)
            {
                Vector3 eulerAngle = animator.deltaRotation.eulerAngles;
                cbc.transform.eulerAngles += new Vector3(0, eulerAngle.y, 0);
            }
        }
        else if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("NeedNotUpdate"))
        {
            cbc.transform.position += animator.deltaPosition;
            cbc.transform.rotation *= animator.deltaRotation;   // 四元数要用 *=
            //Vector3 eulerAngle = animator.deltaRotation.eulerAngles;
            //cbc.transform.eulerAngles += new Vector3(0, eulerAngle.y, 0);
        }

        //if (Input.GetKeyDown(KeyCode.I))
        //    animator.MatchTarget(targetTrans.position, targetTrans.rotation, AvatarTarget.LeftHand, new MatchTargetWeightMask(Vector3.one, 1), 0.07f, 0.15f);
    }

    private void OnDisable()
    {
        ShowOrHideUnEquipDagger(0, false);
        ShowOrHideUnEquipDagger(1, false);
        ShowOrHideDagger(0, false);
        ShowOrHideDagger(1, false);
    }

    /// <summary>
    /// 显示与隐藏手上的匕首
    /// </summary>
    /// <param name="animationEvent"></param>
    private void ShowOrHideDagger(AnimationEvent animationEvent)
    {
        if (animationEvent.intParameter == 0)
            rightDaggerGO.SetActive(Convert.ToBoolean(animationEvent.stringParameter));
        else
            leftDaggerGO.SetActive(Convert.ToBoolean(animationEvent.stringParameter));
    }

    private void ShowOrHideDagger(int handID, bool show)
    {
        if (handID == 0)
            rightDaggerGO.SetActive(show);
        else
            leftDaggerGO.SetActive(show);
    }

    /// <summary>
    /// 显示与隐藏腰上的匕首
    /// </summary>
    /// <param name="animationEvent"></param>
    private void ShowOrHideUnEquipDagger(AnimationEvent animationEvent)
    {
        if (animationEvent.intParameter == 0)
            unEquipRightDaggerGO.SetActive(Convert.ToBoolean(animationEvent.stringParameter));
        else
            unEquipLeftDaggerGO.SetActive(Convert.ToBoolean(animationEvent.stringParameter));
    }

    private void ShowOrHideUnEquipDagger(int handID, bool show)
    {
        if (handID == 0)
            unEquipRightDaggerGO.SetActive(show);
        else
            unEquipLeftDaggerGO.SetActive(show);
    }

    /// <summary>
    /// 切换状态属性，包括材质球、外观、控制器
    /// </summary>
    //private void ChangeStateProperties(int addNum = 1)
    //{
    //    cbc.ChangeStateProperties(addNum);
    //}

    private void SetStartMatchState(int state)
    {
        startMatch = Convert.ToBoolean(state);
    }

    /// <summary>
    /// 当某个状态被打断时（人物受到伤害），需要把一些属性重置
    /// </summary>
    protected override void ResetRoleProperties()
    {
        base.ResetRoleProperties();
        ShowOrHideDagger(0, !cbc.isEquip);
        ShowOrHideDagger(1, !cbc.isEquip);
        ShowOrHideUnEquipDagger(0, cbc.isEquip);
        ShowOrHideUnEquipDagger(1, cbc.isEquip);
    }

    private void StartExecute()
    {
        cbc.rigid.isKinematic = true;
    }

    private void EndExecute()
    {
        cbc.rigid.isKinematic = false;
    }
}