using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public enum P_stateType
{
    Idle,Run,Jump,Fall,Attack,Hurt,Die,Airattack1, Airattack2
}

[Serializable]
public class P_Paramenter
{
    [Header("角色基础参数")]
    public float health;
    public float moveSpeed;
    public float jumpForce;
    public float jumpCount;
    public bool isDead;
    public float playerScore;
    [Header("耐力条部分")]
    public float stamina;
    public float jumpStamina;
    public float attackStamina;
    public float powerfulStrikeStamina;
    public float hurtStamina;
    [Header("计时器相关")]
    public float timeCount;//计时器变量
    public float InvincibilityTime;
    public float hurtStartTime;
    public float hurtDuration;
    [Header("")]
    public float knockBack;
    public float knockUp;
    [Header("玩家的其他组件")]
    public Transform attackPoint;
    public float attackArea;
    public SpriteRenderer theSR;

    public Animator animator;
    public Rigidbody2D rigidbody2D;

    public LayerMask targetlayer;

    [Header("攻击帧事件所用到的参数")]
    public float attackMovedis = 1;//攻击移动的距离
    public AnimatorStateInfo state;
    public Transform attack1;
    public Transform attack2;
    public Transform attack3;
    [Header("打击感")]
    public float shakeTime;
    public float lightPause;
    public float lightStrength;
    public float heavyPause;
    public float heavyStrength;
    [Header("玩家受击时的震动强度")]
    public float duration;
    public float strength;
    [Header("攻击检测")]
    public bool hasTransitionedToAttack2;
    public bool hasTransitionedToAttack3;
    [Header("敌人相关")]
    public int kills;
}

public class PlayerFSM : MonoBehaviour
{
    public static PlayerFSM instance;

    public P_Paramenter paramenter;

    public IState currentState;

    private int playerLayer;//碰撞发生的图层
    private int enemyLayer;

    private Dictionary<P_stateType, IState> states = new Dictionary<P_stateType, IState>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        paramenter.animator = GetComponent<Animator>();
        paramenter.rigidbody2D = GetComponent<Rigidbody2D>();
        paramenter.theSR=GetComponent<SpriteRenderer>();

        states.Add(P_stateType.Idle, new P_IdleState(this));
        states.Add(P_stateType.Run, new P_RunState(this));
        states.Add(P_stateType.Jump, new P_JumpState(this));
        states.Add(P_stateType.Fall, new P_FallState(this));
        states.Add(P_stateType.Attack, new P_AttackState(this));
        states.Add(P_stateType.Hurt, new P_HurtState(this));
        states.Add(P_stateType.Die, new P_DieState(this));
        states.Add(P_stateType.Airattack1, new P_Airattack1(this));
        states.Add(P_stateType.Airattack2, new P_Airattack2(this));

        //添加的监听,注册的事件
        EventCenter.AddListener(EventType.P_hurt, PlayerHurt);
        EventCenter.AddListener(EventType.OpenCollision, OpenCollision);
        EventCenter.AddListener(EventType.CloseCollision, CloseCollision);
        EventCenter.AddListener(EventType.FirstAttack,FirstAttackScore);
        EventCenter.AddListener(EventType.SecondAttack,SecondAttackScore);
        EventCenter.AddListener(EventType.ThirdAttack,ThirdAttackScore);
        EventCenter.AddListener(EventType.AirAttack,AirAttackScore);
        EventCenter.AddListener(EventType.AirAttackDown,AirAttackDownScore);
        EventCenter.AddListener<bool>(EventType.SetActive,SetPlayerActive);
        //EventCenter.AddListener(EventType.ResetPlayer,ResetPlayer);

        //获取子物体
        paramenter.attack1 = transform.Find("attackCheck_1");
        paramenter.attack2 = transform.Find("attackCheck_2");
        paramenter.attack3 = transform.Find("attackCheck_3");

        //设置初始状态
        TransitionState(P_stateType.Idle);

        //初始化图层
        playerLayer = LayerMask.NameToLayer("Player");
        enemyLayer = LayerMask.NameToLayer("Enemy");

    }

    private void Update()
    {
        if (!paramenter.animator.GetCurrentAnimatorStateInfo(0).IsName("die") && !UIManager.instance.isPause) //动画不在死亡，并且不在暂停状态中
        {
            currentState.OnUpdate();
            FlixTo();
        }else if (paramenter.animator.GetCurrentAnimatorStateInfo(0).IsName(("die")))//如果在死亡状态中
        {
            currentState.OnUpdate();
        }
    }

    public void TransitionState(P_stateType type)
    {
        if (currentState != null)
        {
            currentState.OnExit();
        }

        currentState = states[type];
        currentState.OnEnter();
    }

    public void FlixTo()
    {
        float inputX = Input.GetAxisRaw("Horizontal");

        if (inputX == -1)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (inputX == 1)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(paramenter.attackPoint.position, paramenter.attackArea);
    }

    /*
    public IEnumerator WaitForAnimationEndCoroutine( Animator animator, string animName)
    {
        if (isAnimationComplete(animator, animName))
        {
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f);

            animator.Play(animName);
        }
    }

    private bool isAnimationComplete(Animator animator,string animName)
    {
        var info = animator.GetCurrentAnimatorStateInfo(0);

        return info.IsName(animName) && info.normalizedTime >= 1;
    }
    */
    /*
    #region 玩家攻击冷却计时器
    public IEnumerator WaitForSeconds(float time)
    {
        paramenter.timeCount = time;

        while (paramenter.timeCount > 0)
        {
            yield return new WaitForSeconds(1);
            paramenter.timeCount -= 1;
        }

        Debug.Log("可以攻击");
    }
    #endregion
    */

    #region 玩家受到攻击无敌冷却计时器
    public IEnumerator InvincibilityTime(float time) 
    {
        paramenter.InvincibilityTime = time;

        while (paramenter.InvincibilityTime > 0)
        {
            yield return new WaitForSeconds(1);
            paramenter.InvincibilityTime -= 1;
        }

        //Debug.Log("无敌时间结束");
    }
    #endregion
    
    #region 玩家转入受伤状态
    public void PlayerHurt()
    {
        TransitionState(P_stateType.Hurt);
    }
    #endregion

    #region 玩家从受伤状态逐步恢复正常

    public IEnumerator FadeOutColorOverTime(float fadeTime)
    {
        float elapsedTime = 0f;
        Color startColor = paramenter.theSR.color;
        Color targetColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        while (elapsedTime < fadeTime)
        {
            paramenter.theSR.color = Color.Lerp(startColor, targetColor, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // 在循环结束后，确保最终颜色为目标颜色
        paramenter.theSR.color = targetColor;
    }

    #endregion

    #region 玩家攻击帧事件
    public void Attackenemy()
    {
        paramenter.state = paramenter.animator.GetCurrentAnimatorStateInfo(0);
        float lookAtDir = gameObject.transform.localScale.x;

        float inputX = Input.GetAxisRaw("Horizontal");

        if (inputX == 0)
        {
            if (paramenter.state.IsName("attack1") || paramenter.state.IsName("attack2") || paramenter.state.IsName("attack3"))
            {
                paramenter.rigidbody2D.velocity = new Vector3(paramenter.attackMovedis * 1 * lookAtDir / Mathf.Abs(lookAtDir), 0, 0);//按住方向键大移动
            }
        }
        else
        {
            paramenter.rigidbody2D.velocity=new Vector3(paramenter.attackMovedis*2*inputX, 0, 0);//不按方向键，小移动
        }


    }

    #endregion

    #region 开启玩家与敌人的碰撞

    public void OpenCollision()
    {
        //开启碰撞
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
        
    }
    public IEnumerator OpenCollisionAfterOneFrame()
    {
        yield return null; // 等待下一帧

        EventCenter.Broadcast(EventType.OpenCollision);//当玩家不能相应第一次碰撞时，可能是会导致在动画的第一帧之前执行碰撞检测，此时动画可能尚未处于正确的状态，所以延迟一帧检测
    }
    
    #endregion

    #region 关闭玩家与敌人的碰撞

    public void CloseCollision()
    {
        //关闭碰撞
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
    }

    #endregion

    #region 玩家下劈冷却计时器
    public IEnumerator AttackDown(float time)
    {
        paramenter.timeCount = time;

        while (paramenter.timeCount > 0)
        {
            yield return new WaitForSeconds(1);
            paramenter.timeCount -= 1;
        }

        Debug.Log("可以跳跃");
    }
    #endregion

    #region 玩家分数的计算部分

    private void FirstAttackScore()
    {
        if (LevelManager.instance.thisWave <= 0) return;
        var healthAmount = paramenter.health / P_HealthBar.instance.maxHp;
        if (healthAmount == 1.0f)
        {
            paramenter.playerScore += 30 * 1.2f;
        }
        else
        {
            paramenter.playerScore += 30;
        }

    }
    
    private void SecondAttackScore()
    {
        if (LevelManager.instance.thisWave <= 0) return;
        var healthAmount = paramenter.health / P_HealthBar.instance.maxHp;
        if (healthAmount == 1.0f)
        {
            paramenter.playerScore += 50 * 1.2f;
        }
        else
        {
            paramenter.playerScore += 50;
        }

    }
    
    private void ThirdAttackScore()
    {
        if (LevelManager.instance.thisWave <= 0) return;
        var healthAmount = paramenter.health / P_HealthBar.instance.maxHp;
        if (healthAmount == 1.0f)
        {
            paramenter.playerScore += 80 * 1.2f;
        }
        else
        {
            paramenter.playerScore += 80;
        }
    }
    
    private void AirAttackScore()
    {
        if (LevelManager.instance.thisWave <= 0) return;
        var healthAmount = paramenter.health / P_HealthBar.instance.maxHp;
        if (healthAmount == 1.0f)
        {
            paramenter.playerScore += 60 * 1.2f;
        }
        else
        {
            paramenter.playerScore += 60;
        }
    }
    
    private void AirAttackDownScore()
    {
        if (LevelManager.instance.thisWave <= 0) return;
        var healthAmount = paramenter.health / P_HealthBar.instance.maxHp;
        if (healthAmount == 1.0f)
        {
            paramenter.playerScore += 90 * 1.2f;
        }
        else
        {
            paramenter.playerScore += 90;
        }
    }
    
    #endregion

    #region 设置物体的Active

    private void SetPlayerActive(bool state)
    {
        gameObject.SetActive(state);
    }

    #endregion

    #region 重置玩家角色数据
    /*
    private void ResetPlayer()
    {
        paramenter.health = 20;
        paramenter.stamina = 15;
        
        P_HealthBar.instance.hpImage.fillAmount = 1f;
        P_HealthBar.instance.hpImage.color = Color.green;
        
        P_HealthBar.instance.staImage.fillAmount =0.95f;
        TransitionState(P_stateType.Idle);
    }
*/
    #endregion
    
    private void OnDestroy()
    {
        // 取消事件监听
        EventCenter.RemoveListener(EventType.P_hurt, PlayerHurt);
        EventCenter.RemoveListener(EventType.OpenCollision, OpenCollision);
        EventCenter.RemoveListener(EventType.CloseCollision, CloseCollision);
        EventCenter.RemoveListener(EventType.FirstAttack,FirstAttackScore);
        EventCenter.RemoveListener(EventType.SecondAttack,SecondAttackScore);
        EventCenter.RemoveListener(EventType.ThirdAttack,ThirdAttackScore);
        EventCenter.RemoveListener(EventType.AirAttack,AirAttackScore);
        EventCenter.RemoveListener(EventType.AirAttackDown,AirAttackDownScore);
        EventCenter.RemoveListener<bool>(EventType.SetActive,SetPlayerActive);
        //EventCenter.RemoveListener(EventType.ResetPlayer,ResetPlayer);
        // 停止所有 Coroutine
        StopAllCoroutines();
    }

}
