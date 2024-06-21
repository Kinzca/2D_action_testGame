using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_WalkState : IState
{
    private EnemyFSM Manager;
    private E_Paramenter paramenter;

    public E_WalkState(EnemyFSM manager)
    {
        this.Manager = manager;
        this.paramenter = manager.paramenter;
    }

    public void OnEnter()
    {
        paramenter.animator.Play("Walk");
    }

    public void OnUpdate()
    {
        Vector3 targetPosition = paramenter.target.position;

        //Debug.Log("Target Position: " + targetPosition);

        if (Manager.transform.position.x < paramenter.target.position.x)//玩家在乙方的左边
        {
            targetPosition.x -= paramenter.attackArea*2;
            //Manager.transform.position = Vector2.MoveTowards(Manager.transform.position, targetPosition, paramenter.moveSpeed * Time.deltaTime);
            Manager.transform.position = Vector2.MoveTowards(Manager.transform.position, targetPosition, paramenter.moveSpeed * Time.deltaTime);

        }
        else if(Manager.transform.position.x > paramenter.target.position.x)//玩家在乙方的右边
        {
            targetPosition.x += paramenter.attackArea*2;
            //Manager.transform.position = Vector2.MoveTowards(Manager.transform.position, targetPosition, paramenter.moveSpeed * Time.deltaTime);
            Manager.transform.position = Vector2.MoveTowards(Manager.transform.position, targetPosition, paramenter.moveSpeed * Time.deltaTime);

        }

        if (Physics2D.OverlapCircle(paramenter.attackPoint.position, paramenter.attackArea, paramenter.targetlayer))
        {
            Manager.TransitionState(E_stateType.Idle);
        }

        if (Physics2D.OverlapCircle(paramenter.attackPoint.position, paramenter.attackArea, paramenter.targetlayer) && paramenter.attackCount <= 0)//进入攻击状态
        {
            Manager.TransitionState(E_stateType.Attack);
        }
    }

    public void OnExit()
    {

    }
}
