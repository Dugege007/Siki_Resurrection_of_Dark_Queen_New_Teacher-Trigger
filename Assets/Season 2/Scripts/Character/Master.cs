using UnityEngine;

//法师
public class Master : Role
{
    //暗影魔法球
    private GameObject shadowProjectileGo;
    private Transform leftHandTrans;
    private GameObject leftHandBall;
    private Transform rightHandTrans;
    private GameObject rightHandBall;

    //暗影冲击
    private GameObject cleaveEffectGo;
    //多重暗影打击
    //暗影护盾
    private GameObject shadowShieldGo;
    //暗影轰击
    private GameObject bigShadowProjectileGo;

    protected override void Awake()
    {
        base.Awake();
        shadowProjectileGo = Resources.Load<GameObject>("Prefabs/ShadowProjectileMega");
        leftHandTrans = CharacterBaseController.DeepFindChild(transform, "HandLeft");
        rightHandTrans = CharacterBaseController.DeepFindChild(transform, "HandRight");
        leftHandBall = CharacterBaseController.DeepFindChild(transform, "LeftHandBallPS").gameObject;
        rightHandBall = CharacterBaseController.DeepFindChild(transform, "RightHandBallPS").gameObject;
        //暗影斩已在基类中
        cleaveEffectGo = Resources.Load<GameObject>("Prefabs/ShadowCleave");
        shadowShieldGo = CharacterBaseController.DeepFindChild(transform, "ShadowShield").gameObject;
        bigShadowProjectileGo = Resources.Load<GameObject>("Prefabs/ShadowImpactNormal");
    }

    private void OnEnable()
    {
        ShowBall(0);
        ShowBall(1);
    }

    protected override void Start()
    {
        base.Start();
        animator = cbc.animators[(int)State.Master];
        PoolManager.Instance.InitPool(shadowProjectileGo, 6);
        PoolManager.Instance.InitPool(cleaveEffectGo, 6);
        PoolManager.Instance.InitPool(bigShadowProjectileGo, 4);
    }

    private void OnDisable()
    {
        HideBall(0);
        HideBall(1);
    }

    #region 暗影魔法球
    private void CreateShadowProjectile(int isLeft)
    {
        GameObject itemGO;
        if (isLeft == 1)
        {
            itemGO = PoolManager.Instance.GetInstance<GameObject>(shadowProjectileGo);
            PoolManager.Instance.SetPosAndRot(itemGO.transform, leftHandTrans.position, transform.rotation);
        }
        else
        {
            itemGO = PoolManager.Instance.GetInstance<GameObject>(shadowProjectileGo);
            PoolManager.Instance.SetPosAndRot(itemGO.transform, rightHandTrans.position, transform.rotation);
        }
        itemGO.GetComponent<Weapon>().owner = cbc;
        itemGO.layer = gameObject.layer;
        if (cbc.targetTransCBC)
            itemGO.transform.LookAt(cbc.targetTransCBC.transform.position + Vector3.up * 0.8f);
    }

    private void ShowBall(int isLeft)
    {
        if (isLeft == 1)
            leftHandBall.SetActive(true);
        else if (isLeft == 0)
            rightHandBall.SetActive(true);
    }

    private void HideBall(int isLeft)
    {
        if (isLeft == 1)
            leftHandBall.SetActive(false);
        else if (isLeft == 0)
            rightHandBall.SetActive(false);
    }
    #endregion

    //暗影斩方法已在基类中

    #region 暗影冲击
    private void PlayCleaveParticals()
    {
        GameObject itemGO = PoolManager.Instance.GetInstance<GameObject>(cleaveEffectGo);
        PoolManager.Instance.SetPosAndRot(itemGO.transform, transform.position + transform.forward, transform.rotation);

        itemGO.GetComponent<Weapon>().owner = cbc;
        itemGO.layer = gameObject.layer;
    }
    #endregion

    #region 暗影护盾
    private void PlayShadowShield()
    {
        shadowShieldGo.SetActive(true);
    }
    #endregion

    #region 暗影轰击
    private void CreateBigShadowProjectile()
    {
        GameObject itemGO = PoolManager.Instance.GetInstance<GameObject>(bigShadowProjectileGo);
        PoolManager.Instance.SetPosAndRot(itemGO.transform, (leftHandTrans.position + rightHandTrans.position) * 0.5f, transform.rotation);

        itemGO.GetComponent<Weapon>().owner = cbc;
        itemGO.layer = gameObject.layer;
        if (cbc.targetTransCBC)
            itemGO.transform.LookAt(cbc.targetTransCBC.transform.position + Vector3.up * 0.8f);
    }
    #endregion

    /// <summary>
    /// 切换状态属性，包括材质球、外观、控制器
    /// </summary>
    //private void ChangeStateProperties(int addNum = 1)
    //{
    //    cbc.ChangeStateProperties(addNum);
    //}

    /// <summary>
    /// 当某个状态被打断时（人物受到伤害），需要把一些属性重置
    /// </summary>
    protected override void ResetRoleProperties()
    {
        base.ResetRoleProperties();
        HideBall(0);
        HideBall(1);
    }
}
