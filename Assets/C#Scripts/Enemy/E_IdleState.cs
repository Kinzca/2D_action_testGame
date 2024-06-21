using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_IdleState : IState
{
    private EnemyFSM Manager;
    private E_Paramenter paramenter;

    public E_IdleState(EnemyFSM manager)
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
        if (!Physics2D.OverlapCircle(paramenter.attackPoint.position, paramenter.attackArea, paramenter.targetlayer))
        {
            Manager.TransitionState(E_stateType.Walk);
        }

        if (Physics2D.OverlapCircle(paramenter.attackPoint.position, paramenter.attackArea, paramenter.targetlayer) && paramenter.attackCount <= 0)
        {
            Manager.TransitionState(E_stateType.Attack);
        }
    }

    public void OnExit()
    {

    }
}
