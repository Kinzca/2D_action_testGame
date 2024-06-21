using UnityEngine;

public class E_DieState : IState
{
    private EnemyFSM Manager;
    private E_Paramenter paramenter;
    public E_DieState(EnemyFSM manager)
    {
        this.Manager = manager;
        this.paramenter = manager.paramenter;
    }

    public void OnEnter()
    {
        if (!paramenter.isDead)
        {
            paramenter.animator.Play("Dead");
            paramenter.isDead = true;

            PlayerFSM.instance.paramenter.kills++;
        }
    }

    public void OnUpdate()
    {
        paramenter.rigidbody2D.simulated = false;
        Manager.StartDestroy();
    }

    public void OnExit()
    {

    }
}
