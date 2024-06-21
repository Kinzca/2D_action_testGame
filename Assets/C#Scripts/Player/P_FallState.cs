using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class P_FallState : IState
{
    private PlayerFSM Manager;
    private P_Paramenter paramenter;

    public P_FallState(PlayerFSM manager)
    {
        this.Manager = manager;
        this.paramenter = manager.paramenter;
    }
    public void OnEnter()
    {
        paramenter.animator.Play("fall");
    }

    public void OnUpdate()
    {
        float velocity = paramenter.rigidbody2D.velocity.y;
        //判断耐力是否支持跳跃
        float isAllowedJump = paramenter.stamina - paramenter.jumpStamina;
        if (Input.GetButtonDown("Jump") && paramenter.jumpCount < 2 && isAllowedJump >= 0)
        {
            Manager.TransitionState(P_stateType.Jump);
        }

        if (Input.GetButtonDown("Attack"))
        {
            Manager.TransitionState(P_stateType.Airattack1);
        }

        if (velocity == 0)
        {
            float inputX = Input.GetAxisRaw("Horizontal");
            if (inputX==0)
            {
                Manager.TransitionState(P_stateType.Idle);

                paramenter.jumpCount = 0;
            }
            else
            {
                Manager.TransitionState(P_stateType.Run);

                paramenter.jumpCount = 0;
            }
            
        }
    }

    public void OnExit()
    {

    }
}
