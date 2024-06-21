using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScene : MonoBehaviour
{
    private static AttackScene instance;

    private bool isPaused;

    private PlayerFSM fsm;

    public static AttackScene Instance
    {
        get
        {
            if(instance==null)
                instance=Transform.FindAnyObjectByType<AttackScene>();
            return instance;
        }
    }

    private void Start()
    {
        EventCenter.AddListener<float, float>(EventType.P_HurtShake, CamreaShake);
        
        GameObject player = GameObject.FindWithTag("Player");
        fsm= player.GetComponent<PlayerFSM>();
    }

    #region 攻击帧停顿
    public void HitPause(float duration)
    {
        if (!isPaused)
        {
            StartCoroutine(Pause(duration));
        }
    }

    IEnumerator Pause(float duration)
    {
        isPaused = true;

        float originalSpeed = fsm.paramenter.animator.speed;
        fsm.paramenter.animator.speed = 0;

        float elapsedTime = duration;

        yield return new WaitForSeconds(duration);

        fsm.paramenter.animator.speed = originalSpeed;
        isPaused = false;

        //Debug.Log("当前暂停的时间为" + duration);
    }



    #endregion

    #region 相机震动

    private bool isShake;

    public void CamreaShake(float duration,float strength)
    {
        if(!isShake)
        {
            StartCoroutine(Shake(duration, strength));
        }
    }

    IEnumerator Shake(float duration,float strength)
    {
        isShake = true;
        Transform camera =Camera.main.transform;
        Vector3 startPosition = camera.position;

        while(duration > 0) 
        {
            camera.position = UnityEngine.Random.insideUnitSphere * strength + startPosition;

            duration -=Time.deltaTime;

            yield return null;
        }
        camera.position = startPosition;

        isShake=false;
    }

    #endregion

    private void OnDestroy()
    {
        EventCenter.RemoveListener<float, float>(EventType.P_HurtShake, CamreaShake);
    }
}
