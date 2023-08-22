using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 创建人：Dugege
 * 功能说明：摄像机视角控制
 * 创建时间：2023年2月8日11:48:49
 */

public class CameraController : MonoBehaviour
{
    //观察点
    private Transform watchPoint;
    //玩家
    private Transform playerTrans;
    //观察点高度
    public float watchPointHeight = 1.3f;
    //观察距离
    public float currentDistance;
    public float maxDistance = 5;
    public float minDistance = 1;
    //距离变化步长
    public float lerpDistanceSpeed = 0.3f;
    //水平旋转
    public float rotationV;//旋转角度
    public float rotationVSpeed = 1.5f;//旋转速度
    //垂直旋转
    public float angleHLerpValue = 0.11f;//当前垂直旋转的差值
    public float maxAngleH = 80;
    public float minAngleH = -40;
    public float angleHSpeed = 0.02f;//旋转差值速度(系数变化的速度)
    //最终偏移向量
    private Vector3 finalOffsetPos;


    private void Start()
    {
        playerTrans = GameObject.Find("Player").transform;
        watchPoint = new GameObject().transform;
    }

    private void FixedUpdate()
    {
        ChangeDistance();
        ChangeRotationV();
        ChangeAngle();
        SetFinalCameraPos();
    }

    /// <summary>
    /// 改变距离
    /// </summary>
    private void ChangeDistance()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            currentDistance += lerpDistanceSpeed;
            if (currentDistance > maxDistance)
            {
                currentDistance = maxDistance;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            currentDistance -= lerpDistanceSpeed;
            if (currentDistance < minDistance)
            {
                currentDistance = minDistance;
            }
        }
    }

    /// <summary>
    /// 水平旋转
    /// </summary>
    private void ChangeRotationV()
    {
        rotationV = Input.GetAxis("Mouse X") * rotationVSpeed;
        watchPoint.Rotate(0, rotationV, 0);
    }

    /// <summary>
    /// 垂直旋转
    /// </summary>
    private void ChangeAngle()
    {
        angleHLerpValue -= Input.GetAxis("Mouse Y") * angleHSpeed;
        if (angleHLerpValue > maxAngleH / 90)
        {
            angleHLerpValue = maxAngleH / 90;
        }
        else if (angleHLerpValue < minAngleH / 90)
        {
            angleHLerpValue = minAngleH / 90;
        }

        if (angleHLerpValue > 0)
        {
            finalOffsetPos = Vector3.Lerp(-watchPoint.forward, watchPoint.up, angleHSpeed);
        }
        else if (angleHLerpValue < 0)
        {
            finalOffsetPos = Vector3.Lerp(-watchPoint.forward, -watchPoint.up, -angleHSpeed);
        }
        finalOffsetPos.Normalize();
        finalOffsetPos *= currentDistance;
    }

    /// <summary>
    /// 设置摄像机最终位置
    /// </summary>
    private void SetFinalCameraPos()
    {
        if (!playerTrans)
        {
            return;
        }
        Vector3 pointPos = playerTrans.position;
        pointPos.y += watchPointHeight;
        watchPoint.position = Vector3.Lerp(watchPoint.position, pointPos, 0.9f);
        Vector3 cameraPos = watchPoint.position + finalOffsetPos;
        transform.position = Vector3.Lerp(transform.position, cameraPos, 0.2f);
        transform.LookAt(watchPoint.position);
    }
}
