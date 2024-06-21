using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class P_JumpState : IState
{
    private PlayerFSM Manager;
    private P_Paramenter paramenter;


    public P_JumpState(PlayerFSM manager)
    {
        this.Manager = manager;
        this.paramenter = manager.paramenter;
    }
    public void OnEnter()
    {
        paramenter.jumpCount ++;

        paramenter.rigidbody2D.velocity = new Vector2(paramenter.rigidbody2D.velocity.x, paramenter.jumpForce);
        
        paramenter.animator.Play("jump");
        EventCenter.Broadcast(EventType.DeltetStamina, paramenter.jumpStamina);
    }

    public void OnUpdate()
    {
        //判断耐力是否支持跳跃
        float isAllowedJump = paramenter.stamina - paramenter.jumpStamina;
        //判断耐力是否支持攻击
        float isAllowAttack = paramenter.stamina - paramenter.attackStamina;

        float velocity = paramenter.rigidbody2D.velocity.y;

        if (Input.GetButtonDown("Jump") && paramenter.jumpCount < 2 && isAllowedJump >= 0) 
        {

            paramenter.jumpCount ++;

            EventCenter.Broadcast(EventType.DeltetStamina, paramenter.jumpStamina);

            paramenter.rigidbody2D.velocity = new Vector2(paramenter.rigidbody2D.velocity.x, paramenter.jumpForce);
        }

        //角色下劈
        if (Input.GetButtonDown("Attack") && isAllowAttack >= 0) 
        {
            Manager.TransitionState(P_stateType.Airattack1);
        }

        if (velocity < 0)
        {
            Manager.TransitionState(P_stateType.Fall);
        }
        else if (velocity == 0)
        {
            paramenter.jumpCount = 0;
            
        }

    }

    public void OnExit()
    {

    }
}
