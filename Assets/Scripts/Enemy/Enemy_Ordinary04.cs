using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// µÐÈË04 ¿¹±ÞÅÚ Ô¶³Ì¹¥»÷
/// </summary>
public class Enemy_Ordinary04 : Enemy_Remote
{
    protected override void Awake()
    {
        base.Awake();
        bulletName = "Bullet_Enemy_04";
    }
    protected override void Attack()
    {
        spineAnim.AnimationState.SetAnimation(0, EnemyAnimSpineTag.atk, false);
        spineAnim.AnimationState.AddAnimation(0, EnemyAnimSpineTag.stand, true, 0f);
        timeAtkCooling = atkCooling;
        TimeMgr.Instance.AddTime(0.2f, () =>
        {
            PoolMgr.Instance.GetObj(obj =>
            {
                obj.GetComponent<Bullet_Enemy_04>().Init(atkValue, atkPos.position);
            }, bulletName, StaticFields.Bullet);
        });
    }
}
