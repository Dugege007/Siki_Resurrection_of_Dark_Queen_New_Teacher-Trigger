using System;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Master, Blademan, Swordman, Assassin, Valkyrie
}

//游戏人物中的基础控制
public class CharacterBaseController : MonoBehaviour
{
    [Header("Base")]
    [HideInInspector]
    public Animator[] animators;
    //人物根游戏物体上的胶囊体碰撞器
    private Collider biomechCollider;
    public Rigidbody rigid;
    public GameObject[] characterGOs;
    public bool isAI;

    public InputController ic;

    [Header("Target")]
    public EnemyAI enemyAI;
    //技能作用目标
    public CharacterBaseController targetTransCBC;
    //主要用于检测点击到的牢笼
    public Transform targetTrans;

    [Header("Attributes")]
    public int maxHP;
    public int currentHP;
    public int maxMP;
    public float currentMP;
    public bool isDead;

    [Header("Move")]
    private float inputH;
    private float inputV;
    public float moveSpeed;
    //private Vector3 inputDir;
    public float rotateSpeed;
    private bool isOnGround = true;
    public int moveScale;
    //冲刺
    private float timeLost;
    private bool isRunning;
    public bool canGetPlayerInputValue = true;
    public bool canMove = true;

    [Header("Jump")]
    public float jumpForce;
    //最大连跳次数
    public int maxJumpTimes;
    private int jumpCount;
    public bool canJump = true;

    [Header("Equip")]
    public bool canEquip = true;

    [Header("Attack")]
    public bool canAttack = true;
    public int combo;
    public bool startCombo;

    [Header("Equip")]
    public bool isEquip;
    public bool hasBlade;
    public bool canUseSkill = true;

    [Header("Weapon")]
    //0 大剑    1 黑暗之刃    2 长刀    3 右匕首    4 左匕首    5 右脚    6 左脚    7 法师右手
    [HideInInspector]
    public BoxCollider[] weaponColliders;

    [Header("Transfiguration")]
    private ParticleSystem transfigurationEffect;
    private GameObject transEffectGO;
    public State currentState;

    public List<MatchTargetParameters> matchTargetParameterList;
    //public bool startMatch;
    public int matchIndex;   //匹配动画参数-1

    public Transform executePartTrans;  //处决者需要具体匹配到的被处决者部位，是由其他CBC传递过来的
    public Transform executedPartTrans; //被处决者需要或缺到的被处决部位

    private AudioClip jumpClip;
    private AudioClip landClip;
    private AudioClip dieClip;
    private AudioClip getHitClip;
    private AudioClip noTargetClip;
    private AudioClip targetNotInRangeClip;

    private ConstantForce cf;

    public GameObject selectedIconGO;
    private ParticleSystem reviveParticle;

    //关闭后可以掉血，但是不会播放受击动画
    public bool canGetHit = true;
    //关闭后直接无敌
    public bool canDecreaseHP = true;

    public bool canRotarte = true;

    private bool freeViewMode;//true代表自由视角，false代表锁定视角
    private GameObject[] cameraGOs;

    private Vector3 moveDir;
    private float turnSpeed = 90;

    public bool canDestroy;

    #region 事件函数
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        biomechCollider = GetComponent<Collider>();
        animators = GetComponentsInChildren<Animator>();
        ic = GetComponent<InputController>();
        if (isAI)
            enemyAI = transform.Find("EnemyAI").GetComponent<EnemyAI>();
        cf = GetComponent<ConstantForce>();
        selectedIconGO = transform.Find("SelectedIcon").gameObject;
        ShowOrHideSelectedIcon(false);

        if (!isAI)
        {
            cameraGOs = new GameObject[2];
            cameraGOs[0] = GameObject.Find("LockView Camera");
            cameraGOs[1] = GameObject.Find("FreeView Camera");
            cameraGOs[1].SetActive(false);
        }

        jumpClip = Resources.Load<AudioClip>("AudioClips/Jump");
        landClip = Resources.Load<AudioClip>("AudioClips/Land");
        dieClip = Resources.Load<AudioClip>("AudioClips/Die");
        getHitClip = Resources.Load<AudioClip>("AudioClips/Hit");
        noTargetClip = Resources.Load<AudioClip>("AudioClips/Valkyrie/NoTarget");
        targetNotInRangeClip = Resources.Load<AudioClip>("AudioClips/Valkyrie/TargetNotInRange");

        transfigurationEffect = DeepFindChild(transform, "SpinningShadow").GetComponent<ParticleSystem>();
        //动态加载
        transEffectGO = Resources.Load<GameObject>("Prefabs/ShadowMuzzleBig");
        reviveParticle = transform.Find("AuraRingShadow").GetComponent<ParticleSystem>();

        characterGOs = new GameObject[5];
        characterGOs[0] = DeepFindChild(transform, "Master").gameObject;
        characterGOs[1] = DeepFindChild(transform, "BladeMan").gameObject;
        characterGOs[2] = DeepFindChild(transform, "SwordMan").gameObject;
        characterGOs[3] = DeepFindChild(transform, "Assassin").gameObject;
        characterGOs[4] = DeepFindChild(transform, "Valkyrie").gameObject;

        weaponColliders = new BoxCollider[8];

        currentHP = maxHP;
        currentMP = maxMP;
        combo = 0;

        animators[(int)currentState].SetBool("IsOnGround", true);
        if (isAI)
            if (isEquip)
                animators[(int)currentState].SetBool("Equip", true);

        matchTargetParameterList = new List<MatchTargetParameters>()
        {
            new MatchTargetParameters()
            {
                executeMan = AvatarTarget.LeftHand,
                executedMan = ExecutedManPart.NECK,
                startTime = 0,
                targetTime = 0.12f,
                lockRotateXZ = true,
                offset = new Vector3(-0.13f, -0.215f, 0)
                //offset = new Vector3(0.0125f, -0.125f, 0)
            },
            new MatchTargetParameters()
            {
                executeMan = AvatarTarget.LeftHand,
                executedMan = ExecutedManPart.BODY,
                startTime = 0,
                targetTime = 0.12f,
                lockRotateXZ = true,
                offset = new Vector3(0, -0.18f, 0)
                //offset = new Vector3(0, -0.245f, -0.25f)
            },
            new MatchTargetParameters()
            {
                executeMan = AvatarTarget.RightHand,
                executedMan = ExecutedManPart.NECK,
                startTime = 0.175f,
                targetTime = 0.5f,
                offset = new Vector3(-1.03f, 0.125f, -0.39f)
                //offset = new Vector3(-0.6f, -0.5f, 0.225f)
            },
            new MatchTargetParameters()
            {
                executeMan = AvatarTarget.LeftHand,
                executedMan = ExecutedManPart.BODY,
                startTime = 0.35f,
                targetTime = 0.4f,
                lockRotateXZ = true,
                offset = new Vector3(-0.48f, 0.97f, 0.23f)
                //offset = new Vector3(-0.575f, 0.435f, 0.965f)
            }
        };
    }

    private void Start()
    {
        PoolManager.Instance.InitPool(transEffectGO, 2);
        GetAllWeaponColliders();
        ChangeStateProperties(0);
    }

    private void Update()
    {
        if (!isDead)
        {
            if (Input.GetKeyDown(KeyCode.Tab) && !isAI)
            {
                ChangeViewMode();
            }

            if (canRotarte && !isAI && freeViewMode)
            {
                ControlViewAndRotate();
            }

            SetEnemyAIState(true);
            PlayerInput();
            if (currentMP < 100)
            {
                currentMP += Time.deltaTime * 2;
            }
            else
            {
                currentMP = 100;
            }
        }
        else
        {
            SetEnemyAIState(false);
            Invoke("EnableEnemyAI", 3);
        }
    }

    private bool JudgeMPEnough(float value)
    {
        if (isAI)
        {
            return true;
        }
        else
        {
            if (currentMP >= value)
            {
                currentMP -= value;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!canGetPlayerInputValue)
            return;
        Move();
    }

    private void OnCollisionEnter(Collision collision)
    {
        JudgeLandGround(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        JudgeLandGround(collision);
    }

    /// <summary>
    /// 判断是否落地
    /// </summary>
    private void JudgeLandGround(Collision collision)
    {
        if (!isOnGround)
        {
            if (!collision.gameObject.CompareTag("Ground"))
            {
                return;
            }
            //判断是否是脚部碰撞到地面
            //if (collision.GetContact(0).point.y <= transform.position.y)
            //{
            //    AudioSource.PlayClipAtPoint(landClip, transform.position);
            //    isOnGround = true;
            //    animators[(int)currentState].SetBool("IsOnGround", isOnGround);
            //    jumpCount = 0;
            //}

            if (!Physics.Raycast(transform.position, -Vector3.up, out RaycastHit raycastHit, 0.3f))
            {
                return;
            }
            if (!raycastHit.transform.CompareTag("Ground"))
            {
                return;
            }
            rigid.drag = 10;
            AudioSource.PlayClipAtPoint(landClip, transform.position);
            isOnGround = true;
            animators[(int)currentState].SetBool("IsOnGround", isOnGround);
            jumpCount = 0;

            if (ValkyrieIsFlying())
            {
                ValkyrieEquip();
            }
            if (currentState == State.Valkyrie && isDead)
            {
                Die();
            }
        }
    }

    /// <summary>
    /// 设置某个状态的播放速度（此状态已经设置为受动画参数影响的状态）
    /// </summary>
    /// <param name="speed"></param>
    //private void SetAnimationPlaySpeed(float speed)
    //{
    //    animators[(int)currentState].SetFloat("AnimationPlaySpeed", speed);
    //}
    #endregion

    #region 玩家输入
    /// <summary>
    /// 玩家输入
    /// </summary>
    private void PlayerInput()
    {
        if (!canGetPlayerInputValue)
        {
            moveDir = Vector3.zero;
            inputH = inputV = 0;
            SetEnemyAIState(false);
            return;
        }
        PlayerMoveInput();
        PlayerJumpInput();
        PlayerEquipInput();
        PlayerAttackInput();
        PlayerSkillInput();
    }

    public void UnLockAll()
    {
        canMove = canJump = canEquip = canAttack = canUseSkill = canGetPlayerInputValue = true;
        startCombo = false;
        ShowOrHideWeaponColliders(false);
    }

    /// <summary>
    /// 切换装备或职业形态
    /// </summary>
    private void PlayerEquipInput()
    {
        if (!canEquip)
            return;

        //切换装备形态
        if (!ic.GetInputBoolValue(InputCode.EquipState))
        {
        }
        else
        {
            if (currentState == State.Master)
                return;

            if (currentState == State.Blademan)
            {
                if (hasBlade)
                {
                    isEquip = !isEquip;
                    animators[(int)currentState].SetBool("Equip", isEquip);
                }
            }
            else if (currentState == State.Swordman)
            {
                isEquip = !isEquip;
                animators[(int)currentState].SetBool("Equip", isEquip);
                if (isEquip)
                {
                    animators[(int)currentState].CrossFade("EquipSword", 0.1f);
                }
            }
            else if (currentState == State.Assassin)
            {
                isEquip = !isEquip;
                animators[(int)currentState].SetBool("Equip", isEquip);
            }
            else if (currentState == State.Valkyrie)
            {
                ValkyrieEquip();
            }
        }

        //改变当前职业形态
        if (ic.GetInputBoolValue(InputCode.ChangeState) && currentState != State.Valkyrie)
        {
            if (JudgeMPEnough(15))
            {
                animators[(int)currentState].CrossFade("Transfiguration", 0.1f);
            }
        }
    }

    /// <summary>
    /// 瓦格里转换飞行与落地状态属性
    /// </summary>
    private void ValkyrieEquip()
    {
        isEquip = !isEquip;
        rigid.useGravity = isEquip;
        cf.enabled = isEquip;
        if (!isEquip)
        {
            //起飞
            isOnGround = false;
            transform.position += new Vector3(0, 0.35f, 0);
        }
        else
        {
            //落地
            rigid.drag = 1;
            isOnGround = true;
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
        animators[(int)currentState].SetBool("Equip", isEquip);
    }

    /// <summary>
    /// 获得新武器
    /// </summary>
    public void HasNewBlade()
    {
        hasBlade = true;
        if (currentState == State.Blademan)
        {
            animators[(int)currentState].CrossFade("EquipState", 0.1f);
            animators[(int)currentState].SetBool("Equip", true);
            isEquip = true;
            BroadcastMessage("ShowNewBlade", 0.5f);
        }
    }

    #region 攻击
    /// <summary>
    /// 玩家攻击输入
    /// </summary>
    private void PlayerAttackInput()
    {
        if (!canAttack || currentState == State.Master || currentState == State.Valkyrie)
            return;

        if (isEquip)
        {
            if (!ic.GetInputBoolValue(InputCode.AttackState))
            {
                return;
            }
            if (animators[(int)currentState].GetInteger("Combo") == 0 && !startCombo)
            {
                combo++;
                animators[(int)currentState].SetInteger("Combo", combo);
            }

            if (startCombo)
            {
                combo++;
                animators[(int)currentState].SetInteger("Combo", combo);
                if (combo > 3)
                {
                    startCombo = false;
                    combo = 0;
                    animators[(int)currentState].SetInteger("Combo", combo);
                }
                startCombo = false;
            }

            Debug.Log("当前连击次数：" + combo);
        }
    }

    /// <summary>
    /// 检测玩家的技能输入
    /// </summary>
    private void PlayerSkillInput()
    {
        if (!canUseSkill)
            return;

        if (currentState == State.Blademan || currentState == State.Swordman || currentState == State.Assassin)
        {
            if (!isEquip)
                return;
        }

        if (currentState == State.Master || currentState == State.Valkyrie)
        {
            for (int i = 0; i < 7; i++)
            {
                if (!ic.GetInputBoolValue(InputCode.SkillsState[i]))
                {
                    continue;
                }
                if (ValkyrieIsFlying() && !HasAttackTarget())
                {
                    return;
                }

                if (ValkyrieIsFlying())
                    animators[(int)currentState].CrossFade("FlyingSkill" + i.ToString(), 0.1f);
                else
                {
                    if (JudgeMPEnough(i * 3))
                    {
                        animators[(int)currentState].CrossFade("Skill" + i.ToString(), 0.1f);
                    }
                }
            }
        }
        else
        {
            if (!ic.GetInputBoolValue(InputCode.SkillsState[1]))
                return;
            if (JudgeMPEnough(15))
            {
                animators[(int)currentState].CrossFade("Skill1", 0.1f);
            }
        }
    }
    #endregion

    #region 移动
    /// <summary>
    /// 玩家移动输入
    /// </summary>
    private void PlayerMoveInput()
    {
        if (!canMove)
        {
            moveDir = Vector3.zero;
            inputH = inputV = 0;
            SetEnemyAIState(false);
            return;
        }

        if (isRunning)
            moveScale = 3;
        else
        {
            if (ic.GetInputBoolValue(InputCode.MoveSpeedState))
                moveScale = 1;
            else
                moveScale = 2;
        }

        if (ic.GetInputBoolValue(InputCode.RunFastStartState))
        {
            if (Time.time - timeLost < 0.5f && DoubleClickForward())
            {
                //Debug.Log("第二次按下向前");
                isRunning = true;
            }
            //Debug.Log("第一次按下向前");
            timeLost = Time.time;
        }
        else if (ic.GetInputBoolValue(InputCode.RunFastEndState))
        {
            isRunning = false;
        }

        inputH = ic.GetInputFloatValue(InputCode.HorizontalMoveValue) * moveScale;
        inputV = ic.GetInputFloatValue(InputCode.VerticalMoveValue) * moveScale;

        //ControlViewAndRotate();

        if (currentState == State.Assassin)
        {
            if (inputH != 0 || (inputV == 0 && inputH != 0))
            {
                animators[(int)currentState].SetFloat("MoveState", 1);
                //Debug.Log("MoveState = 1");
            }
            else
            {
                animators[(int)currentState].SetFloat("MoveState", 0);
                //Debug.Log("MoveState = 0");
            }
        }

        if (freeViewMode)
        {
            moveDir.Set(inputH, 0, inputV);
            moveDir = cameraGOs[1].transform.TransformDirection(moveDir);
            Vector3.ProjectOnPlane(moveDir, Vector3.up);
            moveDir.Normalize();
        }
    }

    private void ControlViewAndRotate()
    {
        float mouseY = 0;
        float mouseX = ic.GetInputFloatValue(InputCode.HorizontalRotateValue) * moveScale;

        if (ValkyrieIsFlying())
        {
            mouseY = ic.GetInputFloatValue(InputCode.VerticalRotateValue) * moveScale;
        }

        //人物旋转
        if (ic.GetInputBoolValue(InputCode.MoveRotateState))
        {
            if (mouseX != 0 || mouseY != 0)
            {
                float targetYRotation = rotateSpeed * mouseX;
                float targetXRotation = -rotateSpeed * mouseY;
                transform.eulerAngles = Vector3.up * Mathf.Lerp(transform.eulerAngles.y, transform.eulerAngles.y + targetYRotation, Time.deltaTime) + Vector3.right * Mathf.Lerp(transform.eulerAngles.x, transform.eulerAngles.x + targetXRotation, Time.deltaTime);
            }
        }
        else
        {
            if (inputV != 0 && inputH != 0)
            {
                float targetYRotation = rotateSpeed * inputH;
                transform.eulerAngles = Vector3.up * Mathf.Lerp(transform.eulerAngles.y, transform.eulerAngles.y + targetYRotation, Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// 玩家跳跃输入
    /// </summary>
    private void PlayerJumpInput()
    {
        if (!canJump || ValkyrieIsFlying())
            return;

        if (ic.GetInputBoolValue(InputCode.JunpState) && jumpCount < maxJumpTimes)
        {
            if (isOnGround)
            {
                isOnGround = false;
                rigid.drag = 1;
                animators[(int)currentState].SetBool("IsOnGround", isOnGround);
            }

            if (jumpCount == 0)
            {
                AudioSource.PlayClipAtPoint(jumpClip, transform.position);
                animators[(int)currentState].CrossFade("Jump", 0.1f);
            }
            else
                animators[(int)currentState].CrossFade("Double_Jump", 0.1f);

            jumpCount++;
            rigid.AddForce(Vector3.up * jumpForce);
        }
    }

    /// <summary>
    /// 判断玩家在规定时间内是否按下两次向前按键
    /// </summary>
    /// <returns></returns>
    private bool DoubleClickForward()
    {
        return inputV > 0 && ic.GetInputFloatValue(InputCode.VerticalMoveValue) > 0 && currentState != State.Master && currentState != State.Valkyrie;
    }

    /// <summary>
    /// 移动
    /// </summary>
    private void Move()
    {
        //锁定（之前的）视角
        if (!freeViewMode)
        {
            //人物移动
            if (inputV != 0)
            {
                if (!isAI)
                    rigid.MovePosition(transform.position + inputV * moveSpeed * Time.deltaTime * transform.forward);
                if (ValkyrieIsFlying())
                {
                    if (inputV > 0)
                        inputV = 2;
                    else if (inputV < 0)
                        inputV = -2;
                }
                animators[(int)currentState].SetFloat("InputH", 0);
                animators[(int)currentState].SetFloat("InputV", inputV);
            }
            else
            {
                if (inputH != 0)
                {
                    if (!isAI)
                        rigid.MovePosition(transform.position + inputH * moveSpeed * Time.deltaTime * transform.right);
                    if (ValkyrieIsFlying())
                    {
                        if (inputH > 0)
                            inputH = 2;
                        else if (inputH < 0)
                            inputH = -2;
                    }
                    if (ValkyrieIsFlying())
                    {
                        if (inputH > 0)
                            inputH = 2;
                        else
                            inputH = -2;
                    }
                    animators[(int)currentState].SetFloat("InputH", inputH);
                    animators[(int)currentState].SetFloat("InputV", 0);
                }
                else
                {
                    //不移动
                    animators[(int)currentState].SetFloat("InputV", inputV);
                    animators[(int)currentState].SetFloat("InputH", inputH);
                }
            }
        }
        //自由视角
        else
        {
            if (ValkyrieIsFlying())
            {
                moveScale = 2;
            }

            if (!isAI && moveDir.magnitude != 0)
            {
                rigid.MovePosition(rigid.position + moveDir * moveSpeed * Time.fixedDeltaTime * moveScale);
                Vector3 desiredForward = Vector3.RotateTowards(transform.forward, moveDir, turnSpeed * Time.fixedDeltaTime, 0);
                rigid.MoveRotation(Quaternion.LookRotation(desiredForward));
                Debug.DrawLine(transform.position, transform.position);
            }
            animators[(int)currentState].SetFloat("InputV", moveDir.magnitude * moveScale);
        }
    }
    #endregion

    /// <summary>
    /// 改变视野模式
    /// </summary>
    private void ChangeViewMode()
    {
        freeViewMode = !freeViewMode;
        for (int i = 0; i < cameraGOs.Length; i++)
        {
            cameraGOs[i].SetActive(false);
        }
        cameraGOs[System.Convert.ToInt32(freeViewMode)].gameObject.SetActive(true);
    }
    #endregion

    #region 受到伤害
    public void TakeDamage(int damageValue, Vector3 hitPos)
    {
        if (!canDecreaseHP)
        {
            return;
        }
        currentHP -= damageValue;
        if (currentHP <= 0)
        {
            //死亡
            Die();
            animators[(int)currentState].SetBool("Die", true);
            return;
        }
        SetInjuredState();
        if (!canGetHit)
        {
            return;
        }
        ShowOrHideWeaponColliders(false);

        //打断攻击
        ResetAttackBehaviour();

        float x = Vector3.Dot(transform.right, hitPos);
        float y = Vector3.Dot(transform.forward, hitPos - transform.position);
        animators[(int)currentState].SetTrigger("Hit");
        BroadcastMessage("ResetRoleProperties");
        if (ForwardBehindOrLeftRight(hitPos))
        {
            if (y > 0)
            {
                //Debug.Log("伤害源在前方");
                animators[(int)currentState].SetFloat("HitY", 1);
            }
            else
            {
                //Debug.Log("伤害源在后方");
                animators[(int)currentState].SetFloat("HitY", -1);
            }
        }
        else
        {
            if (x > 0)
            {
                //Debug.Log("伤害源在右方");
                animators[(int)currentState].SetFloat("HitX", 1);
            }
            else
            {
                //Debug.Log("伤害源在左方");
                animators[(int)currentState].SetFloat("HitX", -1);
            }
        }
        AudioSource.PlayClipAtPoint(getHitClip, transform.position);
    }

    /// <summary>
    /// 判断并设置残血动画
    /// </summary>
    private void SetInjuredState()
    {
        if (currentHP <= maxHP / 3)
        {
            //残血
            animators[(int)currentState].SetLayerWeight(1, 1);
        }
    }

    private bool ForwardBehindOrLeftRight(Vector3 targetPos)
    {
        float distanceZ = Mathf.Abs(transform.position.z - targetPos.z);
        float distanceX = Mathf.Abs(transform.position.x - targetPos.x);
        if (distanceZ >= distanceX)
            return true;
        else
            return false;
    }

    private void Die()
    {
        if (isAI)
            enemyAI.MissTarget();

        if (ValkyrieIsFlying())
        {
            isDead = true;
            ValkyrieEquip();
        }
        else
        {
            Debug.Log(gameObject.name + "死亡");
            AudioSource.PlayClipAtPoint(dieClip, transform.position);
            isDead = true;
            moveDir = Vector3.zero;
            inputH = inputV = 0;
            biomechCollider.enabled = false;
            rigid.isKinematic = true;
            animators[(int)currentState].SetLayerWeight(1, 0);
            animators[(int)currentState].SetLayerWeight(2, 0);
            currentHP = 0;
            currentMP = 0;

            ShowOrHideWeaponColliders(false);
        }
        if (canDestroy)
        {
            Invoke(nameof(DestroyThisGameObject), 3);
            canDestroy = false;
        }
    }

    private void DestroyThisGameObject()
    {
        Destroy(gameObject);
    }

    public void Revive()
    {
        SetReviveState();
        PlayOrStopReviveEffect(false);
    }

    private void SetReviveState()
    {
        isDead = false;
        biomechCollider.enabled = true;
        rigid.isKinematic = false;
        isEquip = true;
        animators[(int)currentState].SetBool("Equip", true);
        currentHP = maxHP;
        if (!Physics.Raycast(transform.position, -Vector3.up, 1))
        {
            transform.position = new Vector3(-85f, 0.5f, 0f);
        }
        animators[(int)currentState].SetLayerWeight(2, 1);
    }

    public void PlayReviveAnimation()
    {
        animators[(int)currentState].SetBool("Die", false);
        animators[(int)currentState].CrossFade("Revive", 0.1f);
    }

    /// <summary>
    /// 显示与隐藏用于检测的武器碰撞器
    /// </summary>
    /// <param name="animationEvent"></param>
    public void ShowOrHideWeaponColliders(AnimationEvent animationEvent)
    {
        weaponColliders[animationEvent.intParameter].enabled = Convert.ToBoolean(animationEvent.stringParameter);
    }

    public void ShowOrHideWeaponColliders(bool show)
    {
        for (int i = 0; i < weaponColliders.Length; i++)
        {
            weaponColliders[i].enabled = show;
        }
    }

    #region 处决
    /// <summary>
    /// 处决
    /// </summary>
    /// <param name="executeID">处决动画的ID</param>
    public void Execute(int executeID, CharacterBaseController cbc)
    {
        if (isDead)
            return;
        matchIndex = executeID - 1;
        executePartTrans = cbc.executedPartTrans;
        ResetAnimatorParameters();
        //startMatch = true;
        animators[(int)currentState].CrossFade("Execute" + executeID, 0.1f);
    }

    public void BeExecuted(int executeID, Transform executerTrans)
    {
        //Time.timeScale = 0.5f;
        Die();
        ResetAnimatorParameters();
        executerTrans.LookAt(new Vector3(transform.position.x, executerTrans.position.y, transform.position.z));
        if (executeID == 1)
            transform.forward = executerTrans.forward;
        else
            transform.forward = -executerTrans.forward;
        animators[(int)currentState].CrossFade("Executed" + executeID, 0.1f);

        BroadcastMessage("GetExecutedPart");
    }

    public bool CanExecute()
    {
        return currentHP <= maxHP / 4;
    }

    #endregion

    /// <summary>
    /// 重置攻击和处决动画执行的条件
    /// </summary>
    private void ResetAnimatorParameters()
    {
        animators[(int)currentState].SetBool("Attack", false);
        animators[(int)currentState].ResetTrigger("AttackCombo");
        animators[(int)currentState].SetFloat("InputH", 0);
        animators[(int)currentState].SetFloat("InputV", 0);
        canGetPlayerInputValue = false;
        ResetAttackBehaviour();
    }

    private void ResetAttackBehaviour()
    {
        startCombo = false;
        combo = 0;
        animators[(int)currentState].SetInteger("Combo", combo);
    }
    #endregion

    #region 变身
    /// <summary>
    /// 播放变身粒子特效
    /// </summary>
    /// <param name="show">是否开启</param>
    public void PlayTransfigurationParticles(int show)
    {
        bool showState = Convert.ToBoolean(show);
        if (showState)
        {
            transfigurationEffect.gameObject.SetActive(true);
            transfigurationEffect.Play();
        }
        else
        {
            StopTransfigurationEffect();
            PoolManager.Instance.SetPosAndRot(PoolManager.Instance.GetInstance<GameObject>(transEffectGO).transform, transform.position, Quaternion.Euler(new Vector3(90, 0, 0)));
        }
    }

    /// <summary>
    /// 关闭变身特效
    /// </summary>
    public void StopTransfigurationEffect()
    {
        transfigurationEffect.Stop();
    }

    /// <summary>
    /// 切换状态属性，包括材质球、外观、控制器
    /// </summary>
    public void ChangeStateProperties(int addNum = 1)
    {
        currentState += addNum;
        if (addNum != 0)
        {
            if (Convert.ToInt32(currentState) > 3)
                currentState = State.Master;
        }

        Debug.Log(currentState);

        ResetState();
        switch (currentState)
        {
            case State.Master:
                characterGOs[0].SetActive(true);
                maxJumpTimes = 1;
                isEquip = true;
                animators[(int)currentState].SetBool("Equip", isEquip);
                LockCursor(false);
                break;
            case State.Blademan:
                characterGOs[1].SetActive(true);
                LockCursor(true);
                break;
            case State.Swordman:
                characterGOs[2].SetActive(true);
                LockCursor(true);
                break;
            case State.Assassin:
                characterGOs[3].SetActive(true);
                LockCursor(true);
                break;
            case State.Valkyrie:
                characterGOs[4].SetActive(true);
                maxJumpTimes = 1;
                isEquip = true;
                ValkyrieEquip();
                animators[(int)currentState].SetBool("Equip", isEquip);
                LockCursor(false);
                break;
            default:
                break;
        }
        if (addNum != 0)
        {
            animators[(int)currentState].CrossFade("ChangeState", 0f);
        }
        SetInjuredState();
    }

    /// <summary>
    /// 是否锁定鼠标
    /// </summary>
    /// <param name="isLocked"></param>
    private void LockCursor(bool isLocked)
    {
        if (isAI)
        {
            return;
        }
        if (isLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    /// <summary>
    /// 重置所有职业的状态
    /// </summary>
    private void ResetState()
    {
        maxJumpTimes = 2;
        canGetPlayerInputValue = true;
        isEquip = false;
        animators[(int)currentState].SetBool("Equip", isEquip);
        animators[(int)currentState].SetFloat("MoveState", 0);

        for (int i = 0; i < characterGOs.Length; i++)
        {
            characterGOs[i].SetActive(false);
        }
        ResetAttackTarget();
    }
    #endregion

    /// <summary>
    /// 深度查找子对象transform引用
    /// </summary>
    /// <param name="root">父对象</param>
    /// <param name="childName">具体查找的子对象名称</param>
    /// <returns></returns>
    public static Transform DeepFindChild(Transform root, string childName)
    {
        Transform result;
        result = root.Find(childName);
        if (result == null)
        {
            foreach (Transform item in root)
            {
                result = DeepFindChild(item, childName);
                if (result != null)
                {
                    return result;
                }
            }
        }
        return result;
    }

    /// <summary>
    /// 获取所有武器碰撞器组件
    /// 0 大剑    1 黑暗之刃    2 长刀    3 右匕首    4 左匕首    5 右脚    6 左脚    7 法师右手
    /// </summary>
    private void GetAllWeaponColliders()
    {
        weaponColliders[0] = DeepFindChild(characterGOs[2].transform, "BigSword").GetComponent<BoxCollider>();
        weaponColliders[1] = DeepFindChild(characterGOs[1].transform, "DarkBlade").GetComponent<BoxCollider>();
        weaponColliders[2] = DeepFindChild(characterGOs[1].transform, "Blade_Equip").GetComponent<BoxCollider>();
        weaponColliders[3] = DeepFindChild(characterGOs[3].transform, "DaggerRight").GetComponent<BoxCollider>();
        weaponColliders[4] = DeepFindChild(characterGOs[3].transform, "DaggerLeft").GetComponent<BoxCollider>();
        weaponColliders[5] = DeepFindChild(characterGOs[1].transform, "KickRight").GetComponent<BoxCollider>();
        weaponColliders[6] = DeepFindChild(characterGOs[1].transform, "KickLeft").GetComponent<BoxCollider>();
        weaponColliders[7] = DeepFindChild(characterGOs[0].transform, "HandWeapon").GetComponent<BoxCollider>();
    }

    /// <summary>
    /// 开启或关闭敌人AI
    /// </summary>
    /// <param name="state"></param>
    private void SetEnemyAIState(bool state)
    {
        if (enemyAI)
        {
            enemyAI.startAI = state;
        }
    }

    /// <summary>
    /// 看向（锁定）敌人
    /// </summary>
    public void LookAtAttackTarget()
    {
        if (ValkyrieIsFlying())
            transform.LookAt(targetTransCBC.transform.position);
        else
            transform.LookAt(new Vector3(targetTransCBC.transform.position.x, transform.position.y, targetTransCBC.transform.position.z));
    }

    /// <summary>
    /// 看向牢笼
    /// </summary>
    public void LookAtCage()
    {
        if (ValkyrieIsFlying())
            transform.LookAt(targetTrans.position);
        else
            transform.LookAt(new Vector3(targetTrans.position.x, transform.position.y, targetTrans.position.z));
    }

    /// <summary>
    /// 瓦格里是否处于飞行状态
    /// </summary>
    /// <returns></returns>
    public bool ValkyrieIsFlying()
    {
        return currentState == State.Valkyrie && !isEquip;
    }

    /// <summary>
    /// 显示或隐藏当前选中标志
    /// </summary>
    /// <param name="state"></param>
    public void ShowOrHideSelectedIcon(bool state)
    {
        selectedIconGO.SetActive(state);
    }

    public void PlayOrStopReviveEffect(bool play)
    {
        if (play)
        {
            reviveParticle.gameObject.SetActive(true);
            reviveParticle.Play();
        }
        else
            reviveParticle.Stop();
    }

    public bool HasAttackTarget()
    {
        bool canAttack;

        if (!targetTrans && !targetTransCBC)
        {
            canAttack = false;
            AudioSource.PlayClipAtPoint(noTargetClip, transform.position);
        }
        else if (targetTrans)
        {
            //目标是牢笼
            canAttack = JudgeCanAttack(targetTrans.position);
            if (!canAttack)
            {
                AudioSource.PlayClipAtPoint(targetNotInRangeClip, transform.position);
            }
        }
        else if (targetTransCBC)
        {
            //目标是人物
            canAttack = JudgeCanAttack(targetTrans.position);
            if (!canAttack)
            {
                AudioSource.PlayClipAtPoint(targetNotInRangeClip, transform.position);
            }
        }
        else
        {
            canAttack = false;
        }
        return canAttack;
    }

    /// <summary>
    /// 是否在攻击范围内
    /// </summary>
    /// <param name="targetPos">目标位置</param>
    /// <returns>true为可以攻击</returns>
    public bool JudgeCanAttack(Vector3 targetPos)
    {
        return Vector3.Distance(transform.position, targetPos) <= 15;
    }

    public bool JudgeDotToTarget(Vector3 targetPos)
    {
        return Vector3.Dot(transform.forward, targetPos) > 0;
    }

    /// <summary>
    /// 转成法师或者瓦格里时，清空上一个职业选择的目标
    /// </summary>
    public void ResetAttackTarget()
    {
        if (targetTransCBC)
        {
            targetTransCBC.ShowOrHideSelectedIcon(false);
        }
        if (targetTrans)
        {
            targetTrans = null;
        }
    }

    public void PickUpItem(Transform target)
    {
        BroadcastMessage("StartPickUp", target);
        Invoke(nameof(HasNewBlade), 1);
    }

    private void EnableEnemyAI()
    {
        if (LayerMask.LayerToName(transform.parent.gameObject.layer) == "Enemy")
        {
            enemyAI.gameObject.SetActive(false);
            enabled = false;
        }
    }
}

public struct MatchTargetParameters
{
    public AvatarTarget executeMan;
    public ExecutedManPart executedMan;
    public float startTime;
    public float targetTime;
    public bool lockRotateXZ;
    public Vector3 offset;
}

public enum ExecutedManPart
{
    NECK,
    BODY
}