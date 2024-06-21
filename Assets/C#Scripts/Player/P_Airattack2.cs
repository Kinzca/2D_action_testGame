using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Airattack2 : IState
{
    private PlayerFSM Manager;
    private P_Paramenter paramenter;

    public P_Airattack2(PlayerFSM manager)
    {
        this.Manager = manager;
        this.paramenter = manager.paramenter;
    }

    public void OnEnter()
    {
        paramenter.animator.Play("air_attack_down");

        EventCenter.Broadcast(EventType.DeltetStamina, paramenter.powerfulStrikeStamina);

    }

    public void OnUpdate()
    {
        var info = paramenter.animator.GetCurrentAnimatorStateInfo(0);

        if (info.normalizedTime > 0.95)
        {
            Manager.TransitionState(P_stateType.Idle);
        }
    }

    public void OnExit()
    {
        EventCenter.Broadcast(EventType.CloseCollision);
    }
}
