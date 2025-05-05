using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Íõ¶«¶«
/// </summary>
public class Hero_10002 : HeroBase
{
    public override HeroType Type => HeroType.Hero_10002;

    protected override void Attack(Transform target)
    {
        base.Attack(target);

        PoolMgr.Instance.GetObj(obj =>
        {
            obj.transform.SetParent(bullet_Base, false);

            Vector3 spawnPos = atkPos.position;
            Vector3 targetPos = target.position;
            Vector3 dir = (target.position - atkPos.position).normalized;

            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, dir);

            obj.GetComponent<Bullet_Hero_10002>().Init(atkPos.position, rotation, atkValueBuff , sceneMapBG);

        }, bulletName, StaticFields.Bullet);
    }
}
