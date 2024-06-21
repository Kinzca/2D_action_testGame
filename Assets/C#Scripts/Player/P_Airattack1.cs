using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Airattack1 : IState
{
    private PlayerFSM Manager;
    private P_Paramenter paramenter;

    public P_Airattack1(PlayerFSM manager)
    {
        this.Manager = manager;
        this.paramenter = manager.paramenter;
    }

    public void OnEnter()
    {
        EventCenter.Broadcast(EventType.OpenCollision);

        paramenter.animator.Play("air_attack");

        EventCenter.Broadcast(EventType.DeltetStamina, paramenter.attackStamina);
    }

    public void OnUpdate()
    {
        if(Physics2D.OverlapCircle(paramenter.attackPoint.position, paramenter.attackArea, paramenter.targetlayer))
        {
            Manager.TransitionState(P_stateType.Airattack2);
        }
    }

    public void OnExit()
    {

    }
}
