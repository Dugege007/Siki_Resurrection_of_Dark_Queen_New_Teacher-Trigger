using UnityEngine;

//剑客
public class SwordMan : Role
{
    //获取剑的游戏物体
    private GameObject swordGo;
    //剑气特效
    private ParticleSystem swordEffect;

    //暗影冲击
    private GameObject cleaveEffectGo;

    protected override void Awake()
    {
        base.Awake();
        swordGo = CharacterBaseController.DeepFindChild(transform, "BigSword").gameObject;
        swordEffect = CharacterBaseController.DeepFindChild(transform, "ShadowPillarBlast").GetComponent<ParticleSystem>();
        cleaveEffectGo = Resources.Load<GameObject>("Prefabs/ShadowCleave");
    }

    private void OnEnable()
    {
        ShowOrHideSword(true);
    }

    protected override void Start()
    {
        base.Start();
        animator = cbc.animators[(int)State.Swordman];
        PoolManager.Instance.InitPool(cleaveEffectGo, 6);
        PoolManager.Instance.InitPool(cleaveEffectGo, 3);
    }

    private void OnDisable()
    {
        ShowOrHideSword(false);
    }

    private void PlaySwordEffect()
    {
        swordEffect.Play();
    }

    private void ShowOrHideSword(bool show)
    {
        swordGo.SetActive(show);
    }

    /// <summary>
    /// 切换状态属性，包括材质球、外观、控制器
    /// </summary>
    //private void ChangeStateProperties(int addNum = 1)
    //{
    //    cbc.ChangeStateProperties(addNum);
    //}

    #region 暗影冲击
    private void PlayCleaveParticals()
    {
        GameObject itemGO = PoolManager.Instance.GetInstance<GameObject>(cleaveEffectGo);
        PoolManager.Instance.SetPosAndRot(itemGO.transform, transform.position + transform.forward, transform.rotation);
        itemGO.GetComponent<Weapon>().owner = cbc;
        itemGO.layer = gameObject.layer;
    }
    #endregion
}
