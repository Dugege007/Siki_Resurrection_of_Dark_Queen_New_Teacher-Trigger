using System;
using UnityEngine;

//����
public class BladeMan : Role
{
    //������Ϸ����
    private GameObject equipBladeGo;
    private GameObject unEquipBladeGo;
    private GameObject darkBladeGo;
    private ParticleSystem bladeChangeEffect;

    //����ʱ�ĽŲ���Ч
    private ParticleSystem leftHitBallPS;
    private ParticleSystem rightHitBallPS;

    //������
    public int combo;

    protected override void Awake()
    {
        base.Awake();
        equipBladeGo = CharacterBaseController.DeepFindChild(transform, "Blade_Equip").gameObject;
        unEquipBladeGo = CharacterBaseController.DeepFindChild(transform, "Blade_Unequip").gameObject;
        darkBladeGo = CharacterBaseController.DeepFindChild(transform, "DarkBlade").gameObject;
        bladeChangeEffect = Resources.Load<GameObject>("Prefabs/ShadowMuzzleBig").GetComponent<ParticleSystem>();
        leftHitBallPS = CharacterBaseController.DeepFindChild(transform, "LeftHitBallPS").GetComponent<ParticleSystem>();
        rightHitBallPS = CharacterBaseController.DeepFindChild(transform, "RightHitBallPS").GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        if (cbc.hasBlade)
            ShowOrHideEquipBlade(0);
    }

    protected override void Start()
    {
        base.Start();
        animator = cbc.animators[(int)State.Blademan];
    }

    private void OnDisable()
    {
        ShowOrHideEquipBlade(2);
    }

    private void ShowOrHideEquipBlade(int show)
    {
        if (show == 0)
        {
            equipBladeGo.SetActive(false);
            unEquipBladeGo.SetActive(true);
        }
        else if (show == 1)
        {
            equipBladeGo.SetActive(true);
            unEquipBladeGo.SetActive(false);
        }
        else if (show == 2)
        {
            equipBladeGo.SetActive(false);
            unEquipBladeGo.SetActive(false);
        }
    }

    private void ShowHitBall(int isLeft)
    {
        //handBall.SetActive(true);
        if (isLeft == 1)
            leftHitBallPS.Play();
        else if (isLeft == 0)
            rightHitBallPS.Play();
    }

    private void HideHitBall(int isLeft)
    {
        //handBall.SetActive(false);
        if (isLeft == 1)
            leftHitBallPS.Stop();
        else if (isLeft == 0)
            rightHitBallPS.Stop();
    }

    private void ShowOrHideBlade(int show)
    {
        AnimationEvent animationEvent = new AnimationEvent
        {
            intParameter = show
        };

        if (animationEvent.intParameter == 0)
        {
            //��ͨ��
            equipBladeGo.SetActive(Convert.ToBoolean(animationEvent.intParameter));
            //darkBladeGo.SetActive(false);
        }
        else
        {
            //�ڰ�֮��
            darkBladeGo.SetActive(Convert.ToBoolean(animationEvent.intParameter));
            //bladeGo.SetActive(false);
        }
    }

    private void PlayChangeBladeEffect()
    {
        bladeChangeEffect.Play();
    }

    public void ShowNewBlade()
    {
        equipBladeGo.SetActive(true);
    }

    /// <summary>
    /// ��ĳ��״̬�����ʱ�������ܵ��˺�������Ҫ��һЩ��������
    /// </summary>
    protected override void ResetRoleProperties()
    {
        base.ResetRoleProperties();
        if (cbc.isEquip)
        {
            ShowOrHideBlade(System.Convert.ToInt32(cbc.isEquip));
            ShowOrHideEquipBlade(System.Convert.ToInt32(!cbc.isEquip));
            darkBladeGo.SetActive(false);
        }
    }
}
