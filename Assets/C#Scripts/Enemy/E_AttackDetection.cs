using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_AttackDetection : MonoBehaviour
{
    public GameObject player;
    private PlayerFSM fsm;

    private void Start()
    {
        fsm=player.GetComponent<PlayerFSM>();
    }

    public void Update()
    {
        if (fsm == null)
        {
            fsm=player.GetComponent<PlayerFSM>();
        }
        else
        {
            return;
        }
    }

    /*
    public static E_AttackDetection Instance;

    public Transform attackArea;
    public Transform enemy;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 获取当前物体的 Transform 组件
        attackArea = transform;

        // 获取父物体A的 Transform 组件
        enemy = attackArea.parent;
    }
    */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && fsm.paramenter.InvincibilityTime<=0) 
        {
            EventCenter.Broadcast(EventType.P_hurt);

            EventCenter.Broadcast(EventType.P_HurtShake, fsm.paramenter.duration, fsm.paramenter.strength);
        }
    }
}
