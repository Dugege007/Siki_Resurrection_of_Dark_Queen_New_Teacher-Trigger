using UnityEngine;

//����
public class SwordMan : Role
{
    //��ȡ������Ϸ����
    private GameObject swordGo;
    //������Ч
    private ParticleSystem swordEffect;

    //��Ӱ���
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
    /// �л�״̬���ԣ�������������ۡ�������
    /// </summary>
    //private void ChangeStateProperties(int addNum = 1)
    //{
    //    cbc.ChangeStateProperties(addNum);
    //}

    #region ��Ӱ���
    private void PlayCleaveParticals()
    {
        GameObject itemGO = PoolManager.Instance.GetInstance<GameObject>(cleaveEffectGo);
        PoolManager.Instance.SetPosAndRot(itemGO.transform, transform.position + transform.forward, transform.rotation);
        itemGO.GetComponent<Weapon>().owner = cbc;
        itemGO.layer = gameObject.layer;
    }
    #endregion
}
