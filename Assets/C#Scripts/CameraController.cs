using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    public Transform target;

    public Transform NT, NL, MT, FT, FL, FFT;
    public Transform NT1, NL1, MT1, FT1, FL1, FFT1;
    public Transform NT2, NL2, MT2, FT2, FL2, FFT2;

    private Vector2 lastXPos;

    public float minHeight;

    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");

        target = player.GetComponent<Transform>();
        
        lastXPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(target.position.x, Mathf.Clamp(target.transform.position.y,minHeight,Mathf.Infinity) , transform.position.z);

        Vector2 amountToMoveX = new Vector2(transform.position.x - lastXPos.x, transform.position.y - lastXPos.y);

        #region backGroundParallax
        NT.position += new Vector3(amountToMoveX.x * 0.95f, amountToMoveX.y * 0.95f, 0f) * 0.1f;
        NL.position += new Vector3(amountToMoveX.x * 0.95f, amountToMoveX.y * 0.95f, 0f) * 0.1f;

        MT.position += new Vector3(amountToMoveX.x * 0.9f, amountToMoveX.y * 0.9f, 0f) * 0.1f;

        FT.position += new Vector3(amountToMoveX.x * 0.8f, amountToMoveX.y * 0.8f, 0f) * 0.1f;
        FL.position += new Vector3(amountToMoveX.x * 0.8f, amountToMoveX.y * 0.8f, 0f) * 0.1f;

        FFT.position += new Vector3(amountToMoveX.x * 0.7f, amountToMoveX.y * 0.7f, 0f) * 0.1f;
        #endregion

        #region backGroundParallax_1
        NT1.position += new Vector3(amountToMoveX.x * 0.95f, amountToMoveX.y * 0.95f, 0f) * 0.1f;
        NL1.position += new Vector3(amountToMoveX.x * 0.95f, amountToMoveX.y * 0.95f, 0f) * 0.1f;

        MT1.position += new Vector3(amountToMoveX.x * 0.9f, amountToMoveX.y * 0.9f, 0f) * 0.1f;

        FT1.position += new Vector3(amountToMoveX.x * 0.8f, amountToMoveX.y * 0.8f, 0f) * 0.1f;
        FL1.position += new Vector3(amountToMoveX.x * 0.8f, amountToMoveX.y * 0.8f, 0f) * 0.1f;

        FFT1.position += new Vector3(amountToMoveX.x * 0.7f, amountToMoveX.y * 0.7f, 0f) * 0.1f;
        #endregion

        #region backGroundParallax_2
        NT2.position += new Vector3(amountToMoveX.x * 0.95f, amountToMoveX.y * 0.95f, 0f) * 0.1f;
        NL2.position += new Vector3(amountToMoveX.x * 0.95f, amountToMoveX.y * 0.95f, 0f) * 0.1f;

        MT2.position += new Vector3(amountToMoveX.x * 0.9f, amountToMoveX.y * 0.9f, 0f) * 0.1f;

        FT2.position += new Vector3(amountToMoveX.x * 0.8f, amountToMoveX.y * 0.8f, 0f) * 0.1f;
        FL2.position += new Vector3(amountToMoveX.x * 0.8f, amountToMoveX.y * 0.8f, 0f) * 0.1f;

        FFT2.position += new Vector3(amountToMoveX.x * 0.7f, amountToMoveX.y * 0.7f, 0f) * 0.1f;
        #endregion

        lastXPos = transform.position;
    }

}
