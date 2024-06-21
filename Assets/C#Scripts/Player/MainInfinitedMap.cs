using UnityEngine;

public class MainInfinitedMap : MonoBehaviour
{
    Camera mainCam;
    float detectionX; // 储存当前地图的X值
    float mapLength = 14;
    float totalLength;
    float mapNums = 3;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        detectionX = transform.position.x; // 记录当前地图的坐标位置
        totalLength = mapLength * mapNums;
    }

    // Update is called once per frame
    void Update()
    {
        // 检测摄像机位置和地图边界的关系
        if (mainCam.transform.position.x < detectionX - totalLength / 2)
        {
            detectionX -= totalLength; // 减去需要移动的距离
            transform.position = new Vector3(detectionX, transform.position.y, transform.position.z);
        }
        else if (mainCam.transform.position.x > detectionX + totalLength / 2)
        {
            detectionX += totalLength; // 加上需要移动的距离
            transform.position = new Vector3(detectionX, transform.position.y, transform.position.z);
        }
    }
}