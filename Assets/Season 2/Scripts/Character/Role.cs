using UnityEngine;

//角色职业基类
public class Role : MonoBehaviour
{
    //角色控制器
    protected CharacterBaseController cbc;
    //动画控制器
    protected Animator animator;

    //暗影斩
    private GameObject slashEffectGo;
    // 0 脖子
    // 1 身体（肚子）
    public Transform[] parts;
    //溅血粒子
    private GameObject bloodGO;

    private AudioClip changeStartClip;
    private AudioClip changeEndClip;
    private AudioClip[] moveClips;
    private bool pickItem;
    private bool isPicking;
    private float weightValue;
    private Transform targetIKTrans;
    private Transform clavicleRightTrans;

    protected virtual void Awake()
    {
        cbc = GetComponentInParent<CharacterBaseController>();

        slashEffectGo = Resources.Load<GameObject>("Prefabs/ShadowSlash");
        parts = new Transform[2];
        parts[0] = CharacterBaseController.DeepFindChild(transform, "neck_01");
        parts[1] = CharacterBaseController.DeepFindChild(transform, "pelvis");
        bloodGO = Resources.Load<GameObject>("Prefabs/Blood");
        changeStartClip = Resources.Load<AudioClip>("AudioClips/ChangeStart");
        changeEndClip = Resources.Load<AudioClip>("AudioClips/ChangeEnd");
        moveClips = new AudioClip[4];
        for (int i = 0; i < moveClips.Length; i++)
        {
            moveClips[i] = Resources.Load<AudioClip>("AudioClips/Walk" + (i + 1).ToString());
        }
        clavicleRightTrans = CharacterBaseController.DeepFindChild(transform, "clavicle_r");
    }

    protected virtual void Start()
    {
        PoolManager.Instance.InitPool(slashEffectGo, 10);
        PoolManager.Instance.InitPool(bloodGO, 8);
    }

    protected virtual void OnAnimatorIK()
    {
        if (cbc.isDead)
        {
            return;
        }
        if (animator.GetLayerWeight(1) > 0
            && animator.GetCurrentAnimatorStateInfo(0).IsName("Move")
            && animator.GetCurrentAnimatorStateInfo(2).IsName("Default"))
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0.1f);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, clavicleRightTrans.position);
        }
        else
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
        }

        if (IfLookAt())
        {
            animator.SetLookAtWeight(1);
            animator.SetLookAtPosition(new Vector3(cbc.targetTransCBC.transform.position.x, transform.position.y, cbc.targetTransCBC.transform.position.z) + Vector3.up);
        }
        else
        {
            animator.SetLookAtWeight(0);
        }
        //开始捡起物品
        if (pickItem)
        {
            if (isPicking)
            {
                weightValue += Time.deltaTime * 2f;
                if (weightValue >= 1 || !targetIKTrans)
                {
                    isPicking = false;
                    return;
                }
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, weightValue);
                animator.SetIKPosition(AvatarIKGoal.RightHand, targetIKTrans.position);
            }
            else
            {
                weightValue = 0;
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                pickItem = false;
            }

        }
    }

    protected virtual void OnAnimatorMove()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("NeedNotUpdate"))
        {
            cbc.transform.position += animator.deltaPosition;
            cbc.transform.rotation *= animator.deltaRotation;   // 四元数要用 *=
        }
    }

    #region 攻击
    private void StartComboState()
    {
        cbc.startCombo = true;
    }

    private void EndComboState()
    {
        cbc.startCombo = false;
    }

    private void ResetCombo()
    {
        cbc.combo = 0;
        animator.SetInteger("Combo", cbc.combo);
        EndComboState();
    }

    private void PlayParticals(float angle)
    {
        PoolManager.Instance.SetPosAndRot(PoolManager.Instance.GetInstance<GameObject>(slashEffectGo).transform, transform.position + new Vector3(transform.forward.x, transform.forward.y + 1, transform.forward.z * 1.8f), Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, angle)));
    }
    #endregion

    /// <summary>
    /// 显示与隐藏用于检测的武器碰撞器
    /// </summary>
    /// <param name="animationEvent"></param>
    public void ShowOrHideWeaponColliders(AnimationEvent animationEvent)
    {
        cbc.ShowOrHideWeaponColliders(animationEvent);
    }

    public void ShowOrHideWeaponColliders(bool show)
    {
        cbc.ShowOrHideWeaponColliders(show);
    }

    /// <summary>
    /// 变身
    /// </summary>
    /// <param name="show"></param>
    public void PlayTransfigurationParticles(int show)
    {
        if (System.Convert.ToBoolean(show))
        {
            AudioSource.PlayClipAtPoint(changeStartClip, transform.position);
        }
        else
        {
            AudioSource.PlayClipAtPoint(changeEndClip, transform.position);
        }

        cbc.PlayTransfigurationParticles(show);
    }

    /// <summary>
    /// 切换状态属性，包括材质球、外观、控制器
    /// </summary>
    private void ChangeStateProperties(int addNum = 1)
    {
        cbc.ChangeStateProperties(addNum);
    }

    /// <summary>
    /// 处决时匹配的部位
    /// </summary>
    public void GetExecutedPart()
    {
        int partID = (int)cbc.matchTargetParameterList[cbc.matchIndex].executedMan;
        cbc.executedPartTrans = parts[partID];
    }

    /// <summary>
    /// 溅血
    /// </summary>
    private void CreateBlood(int weaponID)
    {
        if (bloodGO)
        {
            PoolManager.Instance.SetPosAndRot(PoolManager.Instance.GetInstance<GameObject>(bloodGO).transform, cbc.weaponColliders[weaponID].transform.position, bloodGO.transform.rotation);
        }
    }

    private void PlaySound(Object audioClip)
    {
        AudioSource.PlayClipAtPoint((AudioClip)audioClip, transform.position);
    }

    private void PlayMoveSound()
    {
        if (cbc.ValkyrieIsFlying())
        {
            return;
        }
        AudioSource.PlayClipAtPoint(moveClips[Random.Range(1, 4)], transform.position);
    }

    private void Revive()
    {
        cbc.Revive();
        ResetRoleProperties();
    }

    /// <summary>
    /// 当某个状态被打断时（人物受到伤害），需要把一些属性重置
    /// </summary>
    protected virtual void ResetRoleProperties()
    {
        ShowOrHideWeaponColliders(false);
        cbc.StopTransfigurationEffect();
    }

    private bool IfLookAt()
    {
        bool lookAt = false;
        if (cbc.targetTransCBC && !cbc.isDead)
        {
            if (cbc.JudgeCanAttack(cbc.targetTransCBC.transform.position) && cbc.JudgeDotToTarget(cbc.targetTransCBC.transform.position))
            {
                lookAt = true;
            }
        }
        else
        {
            lookAt = false;
        }
        return lookAt;
    }

    private void StartPickUp(Transform target)
    {
        targetIKTrans = target;
        pickItem = isPicking = true;
    }
}