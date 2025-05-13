using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Boss02 : Enemy_Remote
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
