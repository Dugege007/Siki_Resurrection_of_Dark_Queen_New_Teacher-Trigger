using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 创建人：Dugege
 * 功能说明：UI管理
 * 创建时间：2023年2月7日22:42:26
 */

public class UIManager : MonoBehaviour
{
    private Image imgHPFluid;
    private Image imgMPFluid;
    private int fillNameID;
    private CharacterBaseController cbc;
    private float currentHPValue;
    private float targetHPValue;
    private float currentMPValue;
    private float targetMPValue;

    private void Start()
    {
        fillNameID = Shader.PropertyToID("_FillLevel");
        imgHPFluid = CharacterBaseController.DeepFindChild(transform, "HP_Fluid").GetComponent<Image>();
        imgMPFluid = CharacterBaseController.DeepFindChild(transform, "HP_Fluid").GetComponent<Image>();
        cbc = GameObject.Find("Player").GetComponent<CharacterBaseController>();

    }

    private void Update()
    {
        targetHPValue = (float)cbc.currentHP / cbc.maxHP;
        if (currentHPValue != targetHPValue)
        {
            currentHPValue = Mathf.MoveTowards(currentHPValue, targetHPValue, Time.deltaTime);
            ApplyHPFluidValue(currentHPValue);
        }
        targetMPValue = cbc.currentMP / cbc.maxMP;
        if (currentMPValue != targetMPValue)
        {
            currentMPValue = Mathf.MoveTowards(currentMPValue, targetMPValue, Time.deltaTime);
            ApplyHPFluidValue(currentMPValue);
        }
    }

    private void ApplyHPFluidValue(float value)
    {
        imgHPFluid.material.SetFloat(fillNameID, value);
    }

    private void ApplyMPFluidValue(float value)
    {
        imgMPFluid.material.SetFloat(fillNameID, value);
    }
}
