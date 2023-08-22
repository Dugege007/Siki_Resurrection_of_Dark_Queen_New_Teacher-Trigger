using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    //public CharacterBaseController playerCBC;
    private NavMeshAgent nav;
    private CharacterBaseController cbc;
    public LayerMask targetLayer;

    public bool startAI;
    private float attackDistance;
    private bool isMoving;
    private float attackTimer;
    public float attackCD;
    public bool canCombo;
    private bool isCombo;
    private Vector3 initPos;
    private bool startReturning;

    public bool useAI;
    public bool isBoss;

    private void Start()
    {
        nav = GetComponentInParent<NavMeshAgent>();
        cbc = GetComponentInParent<CharacterBaseController>();
        //useAI = true;

        if (!cbc.isAI)
        {
            nav.enabled = false;
            gameObject.SetActive(false);
        }

        switch (cbc.currentState)
        {
            case State.Master:
                attackDistance = 5;
                attackCD = 2f;
                break;
            case State.Blademan:
                attackDistance = 2f;
                attackCD = 0.3f;
                break;
            case State.Swordman:
                attackDistance = 2f;
                attackCD = 0.3f;
                break;
            case State.Assassin:
                attackDistance = 2f;
                attackCD = 0.3f;
                break;
            default:
                break;
        }

        if (isBoss)
        {
            attackDistance *= 2;
            attackCD /= 2;
        }

        if (LayerMask.LayerToName(transform.parent.gameObject.layer) == "Player")
        {
            targetLayer = LayerMask.NameToLayer("Enemy");
        }
        else
        {
            targetLayer = LayerMask.NameToLayer("Player");
        }

        initPos = transform.position;
    }

    private void Update()
    {
        if (!useAI)
        {
            return;
        }

        if (startAI)
        {
            cbc.ic.SetDefaultValue();
            if (startReturning)
            {
                if (cbc.targetTransCBC)
                    startReturning = false;
                else
                {
                    if (Vector3.Distance(transform.position,initPos)>0.2f)
                        ReturnToInitPos();
                    else
                        startReturning = false;
                }
                return;
            }
            if (isMoving)
            {
                AIMove();
            }
            else
            {
                if (cbc.targetTransCBC)
                {
                    if (Vector3.Distance(cbc.targetTransCBC.transform.position, transform.position) > attackDistance)
                    {
                        nav.isStopped = false;
                        isMoving = true;
                        cbc.ic.SetInputValue(InputCode.VerticalMoveValue, 1);
                        isCombo = false;
                    }
                    else
                    {
                        AIAttack();
                    }
                }
            }
        }
        else
        {
            nav.isStopped = true;
        }
    }

    /// <summary>
    /// AI的攻击逻辑
    /// </summary>
    private void AIAttack()
    {
        if (!cbc.targetTransCBC)
        {
            return;
        }

        if (!cbc.isEquip)
        {
            Debug.Log("Equip");
            cbc.ic.SetInputValue(InputCode.EquipState, true);
        }
        else
        {
            //AI可以连击
            if (canCombo)
            {
                if (isCombo)
                {
                    if (Time.time - attackTimer >= attackCD)
                    {
                        isCombo = false;
                        cbc.startCombo = false;
                    }
                }
                else
                {
                    if (Time.time - attackTimer >= attackCD)
                    {
                        AttackBehaviour();
                        isCombo = true;
                        cbc.startCombo = true;
                    }
                }
            }
            //AI不可连击
            else
            {
                if (Time.time - attackTimer >= attackCD)
                {
                    AttackBehaviour();
                }
            }
        }
    }

    /// <summary>
    /// AI的攻击行为
    /// </summary>
    private void AttackBehaviour()
    {
        int random = Random.Range(0, 15);
        if (cbc.currentState == State.Master)
        {
            if (random > 0 && random < 7)
            {
                cbc.ic.SetInputValue(InputCode.SkillsState[random], true);
            }
            else
            {
                if (Vector3.Distance(cbc.targetTransCBC.transform.position, transform.position) <= 1.5f)
                {
                    cbc.ic.SetInputValue(InputCode.SkillsState[2], true);
                }
                else
                {
                    cbc.ic.SetInputValue(InputCode.SkillsState[1], true);
                }
            }
        }
        else
        {
            if (random <= 3)
            {
                cbc.ic.SetInputValue(InputCode.SkillsState[1], true);
            }
            else if (random > 3)
            {
                cbc.ic.SetInputValue(InputCode.AttackState, true);
            }
        }
        cbc.transform.LookAt(new Vector3(cbc.targetTransCBC.transform.position.x, cbc.transform.position.y, cbc.targetTransCBC.transform.position.z));
        attackTimer = Time.time;
    }

    /// <summary>
    /// AI的移动逻辑
    /// </summary>
    private void AIMove()
    {
        //未到达攻击距离并且距离很远，快速奔跑状态
        if (JudgeDistance(10))
        {
            if (cbc.isEquip)
            {
                //收刀
                cbc.ic.SetInputValue(InputCode.EquipState, true);
                nav.isStopped = true;
            }
            else
            {
                MoveBehaviour();
            }
        }
        //未到达攻击距离并且距离一般，慢跑状态
        else if (JudgeDistance(8))
        {
            if (!cbc.isEquip)
            {
                //如果没有装备刀，则装备
                cbc.ic.SetInputValue(InputCode.EquipState, true);
                nav.isStopped = true;
            }
            else
            {
                cbc.ic.SetInputValue(InputCode.MoveSpeedState, true);
                MoveBehaviour();
            }
        }
        //未到达攻击距离并且距离较近，行走状态
        else if (JudgeDistance(1))
        {
            if (!cbc.isEquip)
            {
                //如果没有装备刀，则装备
                cbc.ic.SetInputValue(InputCode.EquipState, true);
                nav.isStopped = true;
            }
            else
            {
                MoveBehaviour();
            }
        }
        else
        {
            //到达攻击距离
            //转入攻击状态
            nav.isStopped = true;
            isMoving = false;
            //attackTimer = Time.time;
        }
    }

    /// <summary>
    /// AI的移动行为
    /// </summary>
    private void MoveBehaviour()
    {
        if (cbc.ic.GetInputBoolValue(InputCode.RunFastEndState))
        {
            cbc.ic.SetInputValue(InputCode.RunFastEndState, true);
        }
        nav.SetDestination(cbc.targetTransCBC.transform.position);
        //Debug.Log(playerCBC.transform.position);
        cbc.ic.SetInputValue(InputCode.VerticalMoveValue, 1);
        nav.isStopped = false;
        nav.speed = cbc.moveScale * cbc.moveSpeed;
    }

    private bool JudgeDistance(float distanceScale)
    {
        return Vector3.Distance(cbc.targetTransCBC.transform.position, transform.position) > attackDistance * distanceScale;
    }

    /// <summary>
    /// 攻击目标进入视线
    /// </summary>
    /// <param name="other">攻击目标</param>
    private void OnTriggerEnter(Collider other)
    {
        if (!cbc.targetTransCBC)
        {
            SearchTarget(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!cbc.targetTransCBC)
        {
            SearchTarget(other);
        }
    }

    /// <summary>
    /// 攻击目标离开视线
    /// </summary>
    /// <param name="other">攻击目标</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == targetLayer)
        {
            MissTarget();
        }
    }

    /// <summary>
    /// 丢失目标
    /// </summary>
    public void MissTarget()
    {
        if (!useAI)
        {
            return;
        }

        Debug.Log("丢失目标");
        cbc.targetTransCBC = null;
        isMoving = false;
        cbc.targetTransCBC = null;
        startReturning = true;
    }

    /// <summary>
    /// 在视野范围内搜索攻击目标
    /// </summary>
    private void SearchTarget(Collider other)
    {
        if (!useAI)
        {
            return;
        }

        if (other.gameObject.layer == targetLayer && cbc.targetTransCBC == null)
        {
            CharacterBaseController targetCBC= other.GetComponent<CharacterBaseController>();
            if (targetCBC)
            {
                if (!targetCBC.isDead)
                {
                    cbc.targetTransCBC = targetCBC;
                    startAI = true;
                    isMoving = true;
                    nav.isStopped = false;
                }
            }
        }
    }

    private void ReturnToInitPos()
    {
        nav.isStopped = false;
        nav.SetDestination(initPos);
        cbc.ic.SetInputValue(InputCode.VerticalMoveValue, 1);
    }
}