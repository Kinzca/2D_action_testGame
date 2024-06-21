using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_RunState : IState
{
    private PlayerFSM Manager;

    private P_Paramenter paramenter;

    public P_RunState(PlayerFSM manager)
    {
        this.Manager = manager;
        this.paramenter = manager.paramenter;
    }

    public void OnEnter()
    {
        paramenter.animator.Play("run");
    }

    public void OnUpdate()
    {

        //判断耐力是否支持跳跃
        float isAllowedJump = paramenter.stamina - paramenter.jumpStamina;
        //判断耐力是否支持攻击
        float isAllowAttack = paramenter.stamina - paramenter.attackStamina;
        float inputX = Input.GetAxisRaw("Horizontal");

        Vector2 playerVel = new Vector2(inputX, paramenter.rigidbody2D.velocity.y).normalized;

        paramenter.rigidbody2D.velocity = playerVel * paramenter.moveSpeed;

        if (paramenter.rigidbody2D.velocity==Vector2.zero)
        {
            Manager.TransitionState(P_stateType.Idle);
        }

        if (Input.GetButtonDown("Jump") && isAllowedJump >= 0) 
        {
            Manager.TransitionState(P_stateType.Jump);
        }

        if (Input.GetKeyDown(KeyCode.J) && isAllowAttack >= 0)
        {
            Manager.TransitionState(P_stateType.Attack);
        }
    }

    public void OnExit()
    {

    }
}
