using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 王小虎
/// </summary>
public class Hero_10004 : HeroBase
{
    public override HeroType Type => HeroType.Hero_10004;

    protected override void IsOkAtk()
    {
        if (enemyAllList == null || enemyAllList.Count == 0)
            return;

        Vector3 myPos = transform.localPosition;

        //1.筛选出在攻击范围内的敌人
        float atkDistSqr = atkDistance * atkDistance; // 用平方距离比较更高效
        var inRange = enemyAllList
            .Where(e => (e.localPosition - myPos).sqrMagnitude < atkDistSqr)
            .ToList();

        if (inRange.Count == 0)
            return;  // 范围内没有敌人

        //2.按距离由近及远排序
        var sorted = inRange
            .OrderBy(e => (e.localPosition - myPos).sqrMagnitude)
            .ToList();
        //3.取前20个（若不足20则全部）
        int takeCount = Mathf.Min(20, sorted.Count);
        var nearest20 = sorted.Take(takeCount).ToList();
        //4.随机选一位并攻击
        Attack(nearest20[Random.Range(0, nearest20.Count)]);
    }
    protected override void Attack(Transform target)
    {
        base.Attack(target);

        TimeMgr.Instance.AddTime(0.45f, () =>
        {
            PoolMgr.Instance.GetObj(obj =>
            {
                obj.transform.SetParent(bullet_Base, false);
                obj.GetComponent<Bullet_Hero_10004>().Init(atkPos.position, atkValueBuff, target);
            }, bulletName, StaticFields.Bullet);
        });
    }
}
