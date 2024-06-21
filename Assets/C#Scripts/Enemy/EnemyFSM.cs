using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public enum E_stateType
{
    Walk, Attack, Hit, Die, Idle
}

[Serializable]
public class E_Paramenter
{
    [Header("敌人基础设置")]
    public float health;
    public float moveSpeed;
    public float attackDamage;
    public float spanInterval;
    public int destroyTime;//敌人死亡后的销毁时间
    [Header("敌人的一些组件")]
    public Animator animator;
    public LayerMask targetlayer;
    public Transform attackPoint;
    public Transform target;
    public GameObject player;

    public Image bg;
    public Image hp;
    public Image eff;

    public Rigidbody2D rigidbody2D;
    public SpriteRenderer spriteRenderer;
    [Header("敌人其他设置")]
    public float attackArea;
    public float attackCount;
    public float attackTimer;
    public bool isDead;
    [Header("敌人血条设置")]
    public Image hpImage;
    public Image hpEffect;

    public float maxHp;
    public float effectTime;

    public Coroutine updateCoroutine;

    public bool wasSetActive=false;

}

public class EnemyFSM : MonoBehaviour
{
    //实例化会导致数据共享，从而导致一系列的问题，需要避免通过实例化获取状态机的方法
   // public static EnemyFSM instance;

    public E_Paramenter paramenter;

    private IState currentState;

    //public ObjectPool<GameObject> pool; //创建对象池

    private Dictionary<E_stateType, IState> states = new Dictionary<E_stateType, IState>();

    // Start is called before the first frame update
    void Start()
    {
        paramenter.animator = GetComponent<Animator>();
        paramenter.rigidbody2D = GetComponent<Rigidbody2D>();
        paramenter.spriteRenderer = GetComponent<SpriteRenderer>();

        states.Add(E_stateType.Walk, new E_WalkState(this));
        states.Add(E_stateType.Attack, new E_AttackState(this));
        states.Add(E_stateType.Idle, new E_IdleState(this));
        states.Add(E_stateType.Hit, new E_HitState(this));
        states.Add(E_stateType.Die, new E_DieState(this));

        paramenter.maxHp = paramenter.health;
        EventCenter.AddListener(EventType.E_takeDamage, UpdateHealthBar);

        paramenter.player = GameObject.FindGameObjectWithTag("Player");//获取场景中tag带Player标签的对象
        paramenter.target = paramenter.player.transform;//将对象的值赋值给target

        Transform healthBar = transform.Find("HealthBar");
        Transform hp = healthBar.Find("E_hp");//获取hp组件
        paramenter.hpImage = hp.GetComponent<Image>();//初始化hpimage

        Transform eff = healthBar.Find("E_eff");//获取eff组件
        paramenter.hpEffect = eff.GetComponent<Image>();//初始化hpEff
        //Debug.Log("组件初始化完毕");
        
        
        
        TransitionState(E_stateType.Walk);

    }

    // Update is called once per frame
    void Update()
    {
        currentState.OnUpdate();
        FlixTo(paramenter.target);
        
        RandomTran();
    }

    public void TransitionState(E_stateType type)
    {
        if (currentState != null)
        {
            currentState.OnExit();
        }
        currentState = states[type];
        currentState.OnEnter();
    }

    public void FlixTo(Transform target)
    {
        var info = paramenter.animator.GetCurrentAnimatorStateInfo(0);

        if (target != null && !info.IsName("Dead"))
        {
            if (target.position.x < transform.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (target.position.x > transform.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(paramenter.attackPoint.position, paramenter.attackArea);
    }

    #region 敌人攻击冷却计时器
    public IEnumerator WaitForSeconds()
    {
        paramenter.attackCount = paramenter.attackTimer;

        while (paramenter.attackCount > 0)
        {
            yield return new WaitForSeconds(1);
            paramenter.attackCount -= 1;
        }
    }
    #endregion

    #region 等待2s关闭rigibody2d
    /*
    public void CloseTheRBForSecond()
    {
        StartCoroutine(AfterSecond(1));
    }

    private IEnumerator AfterSecond(int time)
    {
        yield return WaitForSeconds(time);

        paramenter.rigidbody2D.simulated = false;

    }
    */
    #endregion

    #region 敌人死亡后逐渐消失
    public void StartDestroy()
    {
        StartCoroutine(FadeAndDestroy());
    }

    private IEnumerator FadeAndDestroy()
    {
        // 透明度渐变的时间间隔
        float elapsedTime = 0f;

        while (elapsedTime < paramenter.destroyTime)
        {
            if (paramenter != null && paramenter.spriteRenderer != null)
            {
                Color startColor = paramenter.spriteRenderer.color; //获取刚开始的颜色
            
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / paramenter.destroyTime);

                Color newColor = new Color(startColor.r, startColor.g, startColor.b, alpha);

                paramenter.spriteRenderer.color = newColor;
                paramenter.hp.color = newColor;
                paramenter.eff.color = newColor;
                paramenter.bg.color = newColor;
            }

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        Destroy(gameObject);

        //pool.Release(gameObject);

        //EventCenter.RemoveListener(EventType.E_takeDamage, UpdateHealthBar);
    }

    #endregion

    #region 敌人血条代码

    public void UpdateHealthBar()
    {
        if (paramenter != null && paramenter.hpImage != null)
        {
            paramenter.hpImage.fillAmount = paramenter.health / paramenter.maxHp;

            if (paramenter.updateCoroutine != null)
            {
                StopCoroutine(paramenter.updateCoroutine);
            }

            paramenter.updateCoroutine = StartCoroutine(UpdateHpEffect());
            //Debug.Log("已更新血条");
        }
    }


    private IEnumerator UpdateHpEffect()
    {
        float effectLength = paramenter.hpEffect.fillAmount - paramenter.hpImage.fillAmount;
        float elapsedTime = 0f;

        while (elapsedTime < paramenter.effectTime && effectLength != 0)
        {
            float t = elapsedTime / paramenter.effectTime;
            t = t * t * (3f - 2f * t);

            elapsedTime += Time.deltaTime;

            paramenter.hpEffect.fillAmount = Mathf.Lerp(paramenter.hpImage.fillAmount + effectLength, paramenter.hpImage.fillAmount, t);

            yield return null;
        }

        paramenter.hpEffect.fillAmount = paramenter.hpImage.fillAmount;
        //Debug.Log("血条缓降完毕");
    }
    #endregion

    #region 敌人超出一定范围后传送过来

    public void RandomTran()
    {
        if (transform.position.y < -10)
        {
            float transX = paramenter.target.transform.position.x;

            float randomX = (UnityEngine.Random.Range(0, 2) == 0) ? UnityEngine.Random.Range(transX - 15, transX - 7) : UnityEngine.Random.Range(transX + 7, transX + 15);

            Vector3 spawnPosition = new Vector3(randomX, 0, transform.position.x);

            transform.position = spawnPosition;
        }
    }

    #endregion
    
/*
    public void RestEnemy()
    {
        var wave = MonsterPool.instance.thisWave;

        // Debugging information
        Debug.Log($"Current states in the dictionary: {string.Join(", ", states.Keys)}");

        // Check if 'Walk' key is present
        if (!states.ContainsKey(E_stateType.Walk))
        {
            Debug.LogError($"'Walk' key is not present in the dictionary.");
            
            states.Add(E_stateType.Walk, new E_WalkState(this));
            return;
        }

        // Rest of the code
        Color newColor = new Color(1, 1, 1, 1);
        paramenter.spriteRenderer = GetComponent<SpriteRenderer>();

        paramenter.spriteRenderer.color = newColor;
        paramenter.hp.color = newColor;
        paramenter.eff.color = newColor;
        paramenter.bg.color = newColor;

        //重置状态
        TransitionState(E_stateType.Walk);
        paramenter.rigidbody2D.simulated = true;
        paramenter.isDead = false;
        
    }
    */

    private void ResetEnemy()
    {
        if (PlayerFSM.instance.paramenter.isDead)
        {
            paramenter.health = 10;
            paramenter.moveSpeed = 1;
            
        }
    }
}
