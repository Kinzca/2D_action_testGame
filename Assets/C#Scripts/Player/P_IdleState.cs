using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_IdleState : IState
{
    private PlayerFSM Manager;

    private P_Paramenter paramenter;

    public P_IdleState(PlayerFSM manager)
    {
        this.Manager = manager;
        this.paramenter = manager.paramenter;
    }

    public void OnEnter()
    {
        paramenter.animator.Play("Idle");
    }

    public void OnUpdate()
    {
        //判断耐力是否支持跳跃
        float isAllowedJump = paramenter.stamina - paramenter.jumpStamina;
        //判断耐力是否支持攻击
        float isAllowAttack = paramenter.stamina - paramenter.attackStamina;
        float inputX = Input.GetAxisRaw("Horizontal");

        if (inputX != 0)
        {
            Manager.TransitionState(P_stateType.Run);
        }

        if (Input.GetButtonDown("Jump") && isAllowedJump >= 0) 
        {
            Manager.TransitionState(P_stateType.Jump);
        }

        if (Input.GetButtonDown("Attack") && isAllowAttack >= 0) 
        {
            Manager.TransitionState(P_stateType.Attack);
        }
    }


    

    public void OnExit()
    {

    }
}
