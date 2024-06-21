using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
public class P_HurtState :IState
{
    private PlayerFSM Manager;
    private P_Paramenter paramenter;
    private EnemyFSM enemyFSM;

    public P_HurtState(PlayerFSM manager)
    {
        this.Manager = manager;
        this.paramenter = manager.paramenter;
    }

    public void OnEnter()
    {
        enemyFSM = GameObject.FindObjectOfType<EnemyFSM>();
        
        if (paramenter != null && paramenter.animator != null)//添加空检查，阻止潜在的空引用
        {
            paramenter.theSR.color = new Color(1.0f, 0.8f, 0.8f, 0.5f);

            paramenter.animator.Play("hurt");

            paramenter.stamina -= paramenter.hurtStamina;//受伤伤时消耗的耐力
            EventCenter.Broadcast(EventType.DeltetStamina, paramenter.hurtStamina);

            //paramenter.health -= EnemyFSM.instance.paramenter.attackDamage;
            paramenter.health -= enemyFSM.paramenter.attackDamage;//受伤
            EventCenter.Broadcast(EventType.P_takeDamage);

        }

        //计算玩家相对于敌人的方向向量，并矢量单位化，这里减少引用采用attackPoint代替
        if (enemyFSM != null && enemyFSM.paramenter != null && enemyFSM.paramenter.target != null && enemyFSM.paramenter.attackPoint != null)
        {
            Vector2 direction = (enemyFSM.paramenter.target.position - enemyFSM.paramenter.attackPoint.position).normalized;
            //将矢量化应用于玩家击退
            paramenter.rigidbody2D.velocity = new Vector2(direction.x * paramenter.knockBack, direction.y * paramenter.knockUp);
            // Record the start time of the hurt state，记录受伤状态的开始时间
            paramenter.hurtStartTime = Time.time;//采用Time.time跟踪当前时间经历的方法
        }
        else
        {
            // 处理 paramenter.target 或 paramenter.attackPoint 为 null 的情况
            Debug.Log("paramenter.target 或 paramenter.attackPoint 为 null");
        }
        
    }

    public void OnUpdate()
    {
        var info = paramenter.animator.GetCurrentAnimatorStateInfo(0);

        if(paramenter.health <= 0)
        {
            Manager.TransitionState(P_stateType.Die);
        }

        if (info.normalizedTime >= 1f && CanTransitionToIdle())
        {
            //Debug.Log("Transitioning to Idle state!");
            Manager.TransitionState(P_stateType.Idle);
        }
    }

    public void OnExit()
    {
        //退出受伤状态时开始计算，无敌时间何颜色褪去的时间
        Manager.StartCoroutine(Manager.FadeOutColorOverTime(3.0f));
        Manager.StartCoroutine(Manager.InvincibilityTime(3.0f));
    }

    #region 检测是否需要退出受伤状态的时间
    private bool CanTransitionToIdle()
    {
        // Check if the specified duration has passed since entering the hurt state，检查检查从开始受伤状态经历的时间
        return Time.time - paramenter.hurtStartTime >= paramenter.hurtDuration;//经历的时间大于，持续的时间
    }
    #endregion
}
