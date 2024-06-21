using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class P_AttackDection : MonoBehaviour
{
    //public static P_AttackDection instance;
    public GameObject player;
    public Transform Player;
    public Transform Enemy;
    private Animator animator;
    private AnimatorStateInfo state;

    private EnemyFSM fsm;
    private PlayerFSM playerFSM;

    public float atkItemsBack = 1;
    public float atkItemsUp = 1;
    public float playerSpeedInfectBack = 1;

    private void Awake()
    {
        //instance = this;
    }

    private void Start()
    {
        playerFSM=player.GetComponent<PlayerFSM>();
        animator=Player.GetComponent<Animator>();
        fsm = Enemy.GetComponent<EnemyFSM>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" && !fsm.paramenter.isDead) 
        {

            //获取人物与敌人的向量
            Vector3 v = collision.transform.position - Player.position;
            //冻结z轴
            v.z = 0;
            //获取横轴，速度影响击退力度
            float h = Input.GetAxis("Horizontal");

            //如果处于attack2时则额外施加向上的力
            state = animator.GetCurrentAnimatorStateInfo(0);

            if (state.IsName("attack1"))
            {
                v.y += (atkItemsBack * 10 * atkItemsUp);

                ApplyAttackEffects(2, collision);
                EventCenter.Broadcast(EventType.FirstAttack);
            }
            else if (state.IsName("attack2"))
            {
                ApplyAttackEffects(4, collision);
                EventCenter.Broadcast(EventType.SecondAttack);
            }
            else if (state.IsName("attack3"))
            {
                ApplyAttackEffects(8, collision);
                EventCenter.Broadcast(EventType.ThirdAttack);
            }
            else if (state.IsName("air_attack"))
            {
                ApplyAttackEffects(5, collision);
                EventCenter.Broadcast(EventType.AirAttack);
            }
            else if (state.IsName("air_attack_down"))
            {
                ApplyAttackEffects(8, collision);
                EventCenter.Broadcast(EventType.AirAttackDown);
            }

            collision.GetComponent<Rigidbody2D>().velocity = v * atkItemsBack + Vector3.right * h * playerSpeedInfectBack * 10;

        }
    }

    private void ApplyAttackEffects(float damage, Collider2D enemy)
    {
        // 减少敌人生命值
        enemy.GetComponent<EnemyFSM>().paramenter.health -= damage;//碰撞体的EnemyFSM的health变量，以免出现变量错误
        //EnemyFSM.instance.paramenter.health -= damage;

        // 触发敌人状态切换到"Hurt"
        enemy.GetComponent<EnemyFSM>().TransitionState(E_stateType.Hit);//获取当前碰撞物体的FSM组件，从当前物体中转到Hit动画
        //EnemyFSM.instance.TransitionState(E_stateType.Hit);

        if (!state.IsName("air_attack_down")&&!state.IsName("attack3"))
        {
            AttackScene.Instance.HitPause(playerFSM.paramenter.lightPause);//玩家攻击停顿

            AttackScene.Instance.CamreaShake(playerFSM.paramenter.shakeTime, playerFSM.paramenter.lightStrength);//相机震动
        }
        else
        {
            AttackScene.Instance.HitPause(playerFSM.paramenter.heavyPause);//玩家攻击停顿

            AttackScene.Instance.CamreaShake(playerFSM.paramenter.shakeTime, playerFSM.paramenter.heavyStrength);//相机震动
        }

        //Debug.Log("造成了" + damage + "伤害");
    }

}
