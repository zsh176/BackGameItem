using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// ������
/// </summary>
public class Hero_10005 : HeroBase
{
    public override HeroType Type => HeroType.Hero_10005;

    protected override void IsOkAtk()
    {
        if (enemyAllList == null || enemyAllList.Count == 0)
            return;

        Vector3 myPos = transform.localPosition;

        //1.ɸѡ���ڹ�����Χ�ڵĵ���
        float atkDistSqr = atkDistance * atkDistance; // ��ƽ������Ƚϸ���Ч
        var inRange = enemyAllList
            .Where(e => (e.localPosition - myPos).sqrMagnitude < atkDistSqr)
            .ToList();

        if (inRange.Count == 0)
            return;  // ��Χ��û�е���

        //2.�������ɽ���Զ����
        var sorted = inRange
            .OrderBy(e => (e.localPosition - myPos).sqrMagnitude)
            .ToList();
        //3.ȡǰ20����������20��ȫ����
        int takeCount = Mathf.Min(20, sorted.Count);                        
        var nearest20 = sorted.Take(takeCount).ToList();  
        //4.���ѡһλ������
        Attack(nearest20[Random.Range(0, nearest20.Count)]);
    }

    protected override void Attack(Transform target)
    {
        base.Attack(target);
        TimeMgr.Instance.AddTime(0.3f, () =>
        {
            PoolMgr.Instance.GetObj(obj =>
            {
                obj.transform.SetParent(bullet_Base, false);
                obj.GetComponent<Bullet_Hero_10005>().Init(atkPos.position, atkValueBuff, target, enemyAllList);
            }, bulletName, StaticFields.Bullet);
        });
        
    }
}
