using UnityEngine;

//��ʦ
public class Master : Role
{
    //��Ӱħ����
    private GameObject shadowProjectileGo;
    private Transform leftHandTrans;
    private GameObject leftHandBall;
    private Transform rightHandTrans;
    private GameObject rightHandBall;

    //��Ӱ���
    private GameObject cleaveEffectGo;
    //���ذ�Ӱ���
    //��Ӱ����
    private GameObject shadowShieldGo;
    //��Ӱ���
    private GameObject bigShadowProjectileGo;

    protected override void Awake()
    {
        base.Awake();
        shadowProjectileGo = Resources.Load<GameObject>("Prefabs/ShadowProjectileMega");
        leftHandTrans = CharacterBaseController.DeepFindChild(transform, "HandLeft");
        rightHandTrans = CharacterBaseController.DeepFindChild(transform, "HandRight");
        leftHandBall = CharacterBaseController.DeepFindChild(transform, "LeftHandBallPS").gameObject;
        rightHandBall = CharacterBaseController.DeepFindChild(transform, "RightHandBallPS").gameObject;
        //��Ӱն���ڻ�����
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

    #region ��Ӱħ����
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

    //��Ӱն�������ڻ�����

    #region ��Ӱ���
    private void PlayCleaveParticals()
    {
        GameObject itemGO = PoolManager.Instance.GetInstance<GameObject>(cleaveEffectGo);
        PoolManager.Instance.SetPosAndRot(itemGO.transform, transform.position + transform.forward, transform.rotation);

        itemGO.GetComponent<Weapon>().owner = cbc;
        itemGO.layer = gameObject.layer;
    }
    #endregion

    #region ��Ӱ����
    private void PlayShadowShield()
    {
        shadowShieldGo.SetActive(true);
    }
    #endregion

    #region ��Ӱ���
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
    /// �л�״̬���ԣ�������������ۡ�������
    /// </summary>
    //private void ChangeStateProperties(int addNum = 1)
    //{
    //    cbc.ChangeStateProperties(addNum);
    //}

    /// <summary>
    /// ��ĳ��״̬�����ʱ�������ܵ��˺�������Ҫ��һЩ��������
    /// </summary>
    protected override void ResetRoleProperties()
    {
        base.ResetRoleProperties();
        HideBall(0);
        HideBall(1);
    }
}
