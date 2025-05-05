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

        moveSpeed = 50;
    }

    protected override void Update()
    {
        base.Update();
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.Self);

    }
}
