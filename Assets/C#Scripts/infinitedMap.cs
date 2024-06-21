using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteMap : MonoBehaviour
{
    Camera mainCam;
    float detectionX;//储存当前地图的X值
    float mapLength = 14;
    float totalLength;
    float mapNums = 3;

    float lastCamX; // 用于记录上一帧的摄像机X坐标

    private void Start()
    {
        mainCam = Camera.main;
        lastCamX = mainCam.transform.position.x;
    }

    private void Update()
    {
        detectionX = transform.position.x;
        totalLength = mapLength * mapNums;

        // 记录摄像机的运动方向
        float camDirection = Mathf.Sign(mainCam.transform.position.x - lastCamX);

        if (mainCam.transform.position.x > detectionX + totalLength / 2 && camDirection > 0)
        {
            //Debug.Log("Moving Right - transform:" + gameObject.name);
            detectionX += totalLength;
            transform.position = new Vector3(detectionX, transform.position.y, transform.position.z);
        }
        else if (mainCam.transform.position.x < detectionX - totalLength / 2 && camDirection < 0)
        {
            //Debug.Log("Moving Left - transform:" + gameObject.name);
            detectionX -= totalLength;
            transform.position = new Vector3(detectionX, transform.position.y, transform.position.z);
        }

        lastCamX = mainCam.transform.position.x;
    }
}
