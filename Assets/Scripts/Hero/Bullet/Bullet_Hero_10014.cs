using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 保安队长的子弹
/// </summary>
public class Bullet_Hero_10014 : BulletBase
{
    protected override void Awake()
    {
        base.Awake();

        moveSpeed = 42;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isOver) return;
        if (collision.CompareTag(StaticFields.Enemy))
        {
            isOver = true;
            //Debug.Log($"攻击伤害为：{atkValue}");
            enemys.Add(collision.transform);
            enemys[0].GetComponent<EnemyBase>().EnemyBeAtk(atkValue);
            enemys.Clear();

            PoolMgr.Instance.PushObj(gameObject);
        }
    }
}
