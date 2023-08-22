using System.Collections.Generic;
using UnityEngine;

// ������ƣ������AI
public class InputController : MonoBehaviour
{
    private CharacterBaseController cbc;

    public Dictionary<string, bool> inputBoolValueDict;
    public Dictionary<string, float> inputFloatValueDict;

    private RaycastHit raycastHit;

    private void Start()
    {
        cbc = GetComponent<CharacterBaseController>();

        inputBoolValueDict = new Dictionary<string, bool>()
        {
            {InputCode.MoveSpeedState, false },
            {InputCode.RunFastStartState, false },
            {InputCode.RunFastEndState, false },
            {InputCode.JunpState, false },
            {InputCode.EquipState, false },
            {InputCode.ChangeState, false },
            {InputCode.AttackState, false },
            {InputCode.MoveRotateState, false },
        };
        for (int i = 0; i < 7; i++)
        {
            inputBoolValueDict.Add(InputCode.SkillsState[i], false);
        }
        inputFloatValueDict = new Dictionary<string, float>()
        {
            {InputCode.HorizontalMoveValue, 0f },
            {InputCode.VerticalMoveValue, 0f },
            {InputCode.HorizontalRotateValue, 0f },
            {InputCode.VerticalRotateValue, 0f },
        };
    }

    private void Update()
    {
        if (!cbc.isAI)
        {
            SetInputValue(InputCode.HorizontalMoveValue, Input.GetAxis("Horizontal"));
            SetInputValue(InputCode.VerticalMoveValue, Input.GetAxis("Vertical"));
            SetInputValue(InputCode.HorizontalRotateValue, Input.GetAxis("Mouse X"));
            SetInputValue(InputCode.VerticalRotateValue, Input.GetAxis("Mouse Y"));

            SetInputValue(InputCode.MoveSpeedState, Input.GetKey(KeyCode.LeftShift));
            SetInputValue(InputCode.MoveRotateState, Input.GetMouseButton(1));
            SetInputValue(InputCode.RunFastStartState, Input.GetButtonDown("Vertical"));
            SetInputValue(InputCode.RunFastEndState, Input.GetButtonUp("Vertical"));
            SetInputValue(InputCode.JunpState, Input.GetKeyDown(KeyCode.Space));
            SetInputValue(InputCode.EquipState, Input.GetKeyDown(KeyCode.E));
            SetInputValue(InputCode.ChangeState, Input.GetKeyDown(KeyCode.C));
            SetInputValue(InputCode.AttackState, Input.GetMouseButtonDown(0));

            for (int i = 0; i < 7; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                    SetInputValue(InputCode.SkillsState[i], true);
                else
                    SetInputValue(InputCode.SkillsState[i], false);
            }
            if (cbc.currentState == State.Master || cbc.currentState == State.Valkyrie)
            {
                if (Input.GetMouseButton(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out raycastHit))
                    {
                        if (raycastHit.transform.CompareTag("Character") && raycastHit.transform.name != "Player")
                        {
                            if (cbc.targetTransCBC)
                            {
                                cbc.targetTransCBC.ShowOrHideSelectedIcon(false);
                            }
                            cbc.targetTransCBC = raycastHit.transform.GetComponent<CharacterBaseController>();
                            if (cbc.targetTransCBC)
                            {
                                //��������
                                cbc.targetTransCBC.ShowOrHideSelectedIcon(true);
                                cbc.LookAtAttackTarget();
                            }
                            else
                            {
                                //��������
                                cbc.targetTrans = raycastHit.transform;
                                cbc.LookAtCage();
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// ���õ�ǰĳ������״̬��ֵ
    /// </summary>
    /// <param name="inputCode"></param>
    /// <param name="inputValue"></param>
    public void SetInputValue(string inputCode, bool inputValue)
    {
        if (inputBoolValueDict.ContainsKey(inputCode))
            inputBoolValueDict[inputCode] = inputValue;
        else
            Debug.Log("������������󣬴�����Ϊ��" + inputCode);
    }

    /// <summary>
    /// ���õ�ǰĳ������״̬��ֵ
    /// </summary>
    /// <param name="inputCode"></param>
    /// <param name="inputValue"></param>
    public void SetInputValue(string inputCode, float inputValue)
    {
        if (inputFloatValueDict.ContainsKey(inputCode))
            inputFloatValueDict[inputCode] = inputValue;
        else
            Debug.Log("������������󣬴�����Ϊ��" + inputCode);
    }

    /// <summary>
    /// ��ȡ��ǰĳ�������״̬
    /// </summary>
    /// <param name="inputCode"></param>
    /// <returns></returns>
    public bool GetInputBoolValue(string inputCode)
    {
        if (inputBoolValueDict.ContainsKey(inputCode))
            return inputBoolValueDict[inputCode];
        else
        {
            Debug.Log("��ȡ��������󣬴�����Ϊ��" + inputCode);
            return false;
        }
    }

    /// <summary>
    /// ��ȡ��ǰĳ�������״̬
    /// </summary>
    /// <param name="inputCode"></param>
    /// <returns></returns>
    public float GetInputFloatValue(string inputCode)
    {
        if (inputFloatValueDict.ContainsKey(inputCode))
            return inputFloatValueDict[inputCode];
        else
        {
            Debug.Log("��ȡ��������󣬴�����Ϊ��" + inputCode);
            return 0f;
        }
    }

    public void SetDefaultValue()
    {
        SetInputValue(InputCode.HorizontalMoveValue, 0);
        SetInputValue(InputCode.VerticalMoveValue, 0);

        SetInputValue(InputCode.MoveSpeedState, false);
        SetInputValue(InputCode.RunFastStartState, false);
        SetInputValue(InputCode.RunFastEndState, false);
        SetInputValue(InputCode.JunpState, false);
        SetInputValue(InputCode.EquipState, false);
        SetInputValue(InputCode.ChangeState, false);
        SetInputValue(InputCode.AttackState, false);

        for (int i = 0; i < 7; i++)
        {
            SetInputValue(InputCode.SkillsState[i], false);
        }
    }
}

public static class InputCode
{
    //�ƶ�
    public const string HorizontalMoveValue = "HorizontalMoveValue";
    public const string VerticalMoveValue = "VerticalMoveValue";
    public const string MoveSpeedState = "MoveSpeedScale";
    public const string RunFastStartState = "RunFastStartState";
    public const string RunFastEndState = "RunFastEndState";
    public const string MoveRotateState = "MoveRotateState";    //�Ƿ������ת����
    public const string HorizontalRotateValue = "HorizontalRotateValue";    //������ת
    public const string VerticalRotateValue = "VerticalRotateValue";        //������ת
    //��Ծ
    public const string JunpState = "JumpState";
    //װ��
    public const string EquipState = "EquipState";
    //����
    public const string ChangeState = "ChangeState";
    //����
    public const string AttackState = "AttackState";
    //�ż���
    public static string[] SkillsState = new string[] { "SkillState0", "SkillState1", "SkillState2", "SkillState3", "SkillState4", "SkillState5", "SkillState6" };
}
