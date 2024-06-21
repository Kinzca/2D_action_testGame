using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class P_HealthBar : MonoBehaviour
{
    public static P_HealthBar instance;
    public GameObject player;

    public Image hpImage;
    public Image hpEffect;
    public Image staImage;
    public Gradient colorGradient;

    public float maxHp;
    public float maxSta;
    public float effectTime;
    public float staminaTime;

    public float targetStamina;

    private bool isTakingStamina = false;

    private Coroutine updateCoroutine;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        PlayerFSM.instance =player.GetComponent<PlayerFSM>();

        if (PlayerFSM.instance != null)
        {
            maxHp = PlayerFSM.instance.paramenter.health;
            maxSta = PlayerFSM.instance.paramenter.stamina;
            hpImage.color = colorGradient.Evaluate(PlayerFSM.instance.paramenter.health / maxHp);//更新血条和耐力条的初始状态

            //Debug.Log("初始值已赋值");

            EventCenter.AddListener(EventType.P_takeDamage, UpdateHealthBar);
            EventCenter.AddListener<float>(EventType.DeltetStamina,UpdateStamana);
        }
        else
        {
            Debug.LogError("EnemyFSM.instance is null. Make sure it is properly initialized.");
        }
    }


    #region 血条代码

    private void UpdateHealthBar()
    {
        if (PlayerFSM.instance != null)
        {
            hpImage.fillAmount = PlayerFSM.instance.paramenter.health / maxHp;

            if (updateCoroutine != null)
            {
                StopCoroutine(updateCoroutine);
            }

            hpImage.color = colorGradient.Evaluate(PlayerFSM.instance.paramenter.health / maxHp);
            updateCoroutine = StartCoroutine(UpdateHpEffect());
            //Debug.Log("已更新血条");
        }
        else
        {
            Debug.LogError("EnemyFSM.instance is null. Make sure it is properly initialized.");
        }
    }

    private IEnumerator UpdateHpEffect()
    {
        float effectLength = hpEffect.fillAmount - hpImage.fillAmount;
        float elapsedTime = 0f;

        while (elapsedTime < effectTime && effectLength != 0)
        {
            float t = elapsedTime / effectTime;
            t = t * t * (3f - 2f * t);

            elapsedTime += Time.deltaTime;

            hpEffect.fillAmount = Mathf.Lerp(hpImage.fillAmount + effectLength, hpImage.fillAmount, t);

            yield return null;
        }

        hpEffect.fillAmount = hpImage.fillAmount;
        //Debug.Log("血条缓降完毕");
    }

    #endregion

    #region 耐力条代码

    public void UpdateStamana(float n) //根据动画状态的不同传入不同的n值
    {
        if (!isTakingStamina) //不在消耗耐力的协程内,同时不在死亡状态
        {
            StartCoroutine(TakeStamina(n));//则开启耐力消耗的协程

            updateCoroutine = StartCoroutine(RestoreStamina());//判断并退出，restorestamina
            if (updateCoroutine != null)//判断耐力条是否在恢复，
            {
                StopCoroutine(updateCoroutine);//如果在恢复则停止恢复,再开启新的协程
                StartCoroutine(TakeStamina(n));
            }
            else//否则直接开启新的协程
            {
                StartCoroutine(TakeStamina(n));
            }

            PlayerFSM.instance.paramenter.stamina -= n;//更新耐力条

            //Debug.Log("目前的耐力为" + PlayerFSM.instance.paramenter.stamina);
        }
    }

    IEnumerator TakeStamina(float stamina)
    {
        isTakingStamina = true;
        //Debug.Log("协程已启动");

        float targetStamina = staImage.fillAmount - stamina / maxSta;

        float elapsedTime = 0;

        while (elapsedTime < staminaTime)
        {
            staImage.fillAmount = Mathf.Lerp(staImage.fillAmount, targetStamina, elapsedTime / staminaTime);

            if (staImage.fillAmount <= 0)
            {
                staImage.fillAmount = 0;
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //Debug.Log("耐力条更新完毕");

        isTakingStamina = false;
        StartCoroutine(RestoreStamina());//启动耐力恢复
    }

    #endregion

    #region 耐力恢复

    public IEnumerator RestoreStamina()
    {
        float currentStamina = staImage.fillAmount;

        while (currentStamina < 1 && !PlayerFSM.instance.paramenter.isDead)
        {
            staImage.fillAmount = PlayerFSM.instance.paramenter.stamina / maxSta;
            PlayerFSM.instance.paramenter.stamina += Time.deltaTime/2;

            if (staImage.fillAmount >= 1)
            {
                staImage.fillAmount = maxSta;
                break; // 添加退出条件
            }

            yield return null; // 等待一帧
        }
        //Debug.Log("耐力条恢复完成");
    }
    #endregion

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.P_takeDamage,UpdateHealthBar);
        EventCenter.RemoveListener<float>(EventType.DeltetStamina,UpdateStamana);
    }
}
