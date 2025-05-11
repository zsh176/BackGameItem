using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����ӳ����ӵ�
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
            //Debug.Log($"�����˺�Ϊ��{atkValue}");
            enemys.Add(collision.transform);
            enemys[0].GetComponent<EnemyBase>().EnemyBeAtk(atkValue);
            enemys.Clear();

            PoolMgr.Instance.PushObj(gameObject);
        }
    }
}
