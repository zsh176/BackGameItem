using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 远程攻击敌人
/// </summary>
public abstract class Enemy_Remote : EnemyBase
{
    private GameObject remote_Atk;//远程攻击检测范围
    protected string bulletName;//子弹资源名
    protected Transform atkPos;//子弹发射位置
    protected override void Awake()
    {
        base.Awake();
        remote_Atk = transform.Find("Remote_Atk").gameObject;
        atkPos = transform.Find("AtkPos");
    }
    protected override void Attack()
    {
        base.Attack();
    }
    protected override void PushObj()
    {
        remote_Atk.SetActive(true);
        base.PushObj();
    }
    protected override void StopMoveStartAtk()
    {
        base.StopMoveStartAtk();
        remote_Atk.SetActive(false);
    }

   
}
