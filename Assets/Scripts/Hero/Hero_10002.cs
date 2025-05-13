using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������
/// </summary>
public class Hero_10002 : HeroBase
{
    public override HeroType Type => HeroType.Hero_10002;

    protected override void Attack(Transform target)
    {
        base.Attack(target);

        //�Ƿ񱩻�
        bool isStrike = Random.value > 0.8f;

        //�ӳ������ӵ���������ͬ��
        TimeMgr.Instance.AddTime(0.2f, () =>
        {
            PoolMgr.Instance.GetObj(obj =>
            {
                obj.transform.SetParent(bullet_Base, false);

                Vector3 dir = (target.position - atkPos.position).normalized;

                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, dir);

                obj.GetComponent<Bullet_Hero_10002>().Init(atkPos.position, rotation, atkValueBuff, sceneMapBG, isStrike);

            }, bulletName, StaticFields.Bullet);
        });
    }
}
