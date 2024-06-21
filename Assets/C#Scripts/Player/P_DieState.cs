using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_DieState : IState
{
    private PlayerFSM Manager;
    private P_Paramenter paramenter;

    public P_DieState(PlayerFSM Manager)
    {
        this.Manager = Manager;
        this.paramenter = Manager.paramenter;
    }

    public void OnEnter()
    {
        paramenter.animator.Play("die");
        paramenter.isDead = true;
        
        EventCenter.Broadcast(EventType.DeleteData);
        EventCenter.Broadcast(EventType.SavePlayerRank);
        
        //Debug.Log("Enter Die State");
    }

    public void OnUpdate()
    {
        var info = paramenter.animator.GetCurrentAnimatorStateInfo(0);

        if (info.normalizedTime >= 1f ) 
        {
            EventCenter.Broadcast(EventType.FinalMenu);
        }
    }


    public void OnExit()
    {

    }
}
