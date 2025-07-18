using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 近程攻击敌人
/// </summary>
public class Enemy_Nearly : EnemyBase
{
    protected override void Attack()
    {
        base.Attack();
        //发送事件，玩家被攻击
        BeAtkData data = new BeAtkData()
        {
            harm = atkValue,
            pos = transform.position,
        };
        EventMgr.Instance.EventTrigger<BeAtkData>(E_EventType.playerBeAtk, data);
    }
}
