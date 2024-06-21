using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_HitState : IState
{
    private EnemyFSM Manager;
    private E_Paramenter paramenter;

    public E_HitState(EnemyFSM manager)
    {
        this.Manager = manager;
        this.paramenter = manager.paramenter;
    }

    public void OnEnter()
    {
        EventCenter.Broadcast(EventType.E_takeDamage);//事件广播，敌人扣血

        if (paramenter.health <= 0)
        {
            Manager.TransitionState(E_stateType.Die);
        }
        else
        {
            paramenter.animator.Play("Hit");
        }

    }

    public void OnUpdate()
    {
        var info = paramenter.animator.GetCurrentAnimatorStateInfo(0);

        if (info.normalizedTime > 0.95)
        {
            Manager.TransitionState(E_stateType.Walk);
        }
    }

    public void OnExit()
    {

    }
}
