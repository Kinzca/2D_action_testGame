using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;

public class E_AttackState : IState
{
    private EnemyFSM Manager;
    private E_Paramenter paramenter;

    public E_AttackState(EnemyFSM manager)
    {
        this.Manager = manager;
        this.paramenter = manager.paramenter;
    }

    public void OnEnter()
    {
        paramenter.animator.Play("Attack");

        Manager.StartCoroutine(Manager.WaitForSeconds());
    }

    public void OnUpdate()
    {
        var info = paramenter.animator.GetCurrentAnimatorStateInfo(0);

        float distance = Vector2.Distance(Manager.transform.position, paramenter.target.transform.position);

        if (info.normalizedTime > 0.95f||PlayerFSM.instance.paramenter.animator.GetCurrentAnimatorStateInfo(0).IsName("die"))//动画进度大于0.95或者玩家已死亡
        {
            Manager.TransitionState(E_stateType.Walk);
        }
    }

    public void OnExit()
    {

    }
}
