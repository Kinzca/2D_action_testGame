using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class P_AttackState : IState
{
    private PlayerFSM Manager;
    private P_Paramenter paramenter;

    public P_AttackState(PlayerFSM manager)
    {
        this.Manager = manager;
        this.paramenter = manager.paramenter;
    }

    public void OnEnter()
    {
        EventCenter.Broadcast(EventType.OpenCollision);//开启碰撞检测

        paramenter.animator.Play("attack1");

        EventCenter.Broadcast(EventType.DeltetStamina, paramenter.attackStamina);

        paramenter.hasTransitionedToAttack2 = false;
        paramenter.hasTransitionedToAttack3 = false;

        //paramenter.rigidbody2D.velocity = Vector2.zero;

    }

    public void OnUpdate()
    {
        //判断耐力是否支持攻击
        float isAllowAttack = paramenter.stamina - paramenter.attackStamina;
        float isAllowHeavyAttack = paramenter.stamina - paramenter.powerfulStrikeStamina;

        var info = paramenter.animator.GetCurrentAnimatorStateInfo(0);

        if (!paramenter.hasTransitionedToAttack2 && info.IsName("attack1") && Input.GetButtonDown("Attack") && info.normalizedTime >= 0.4f && isAllowAttack >= 0)
        {
            paramenter.hasTransitionedToAttack2 = true;
            paramenter.animator.CrossFadeInFixedTime("attack2", 0.1f);

            EventCenter.Broadcast(EventType.DeltetStamina, paramenter.attackStamina);
        }
        else if (!paramenter.hasTransitionedToAttack3 && info.IsName("attack2") && Input.GetButtonDown("Attack") && info.normalizedTime >= 0.4f && isAllowHeavyAttack >= 0)  
        {
            paramenter.hasTransitionedToAttack3 = true;
            paramenter.animator.CrossFadeInFixedTime("attack3", 0.15f);

            EventCenter.Broadcast(EventType.DeltetStamina, paramenter.powerfulStrikeStamina);
        }

        if (info.normalizedTime >= 1)
        {
            Manager.TransitionState(P_stateType.Idle);
        }
    }

    public void OnExit()
    {
        EventCenter.Broadcast(EventType.CloseCollision);
    }
}
