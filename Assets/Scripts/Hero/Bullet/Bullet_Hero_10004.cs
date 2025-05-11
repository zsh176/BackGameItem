using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 王小虎子弹
/// </summary>
public class Bullet_Hero_10004 : BulletBase
{
    private bool isDown;//是否开始下降
    private float posY;
    protected override void Awake()
    {

    }

    public void Init(Vector3 initPos, int initAtkvalu, Transform initTarget)
    {
        transform.position = initPos;
        transform.localRotation = Quaternion.identity;
        atkValue = initAtkvalu;
        atkTarget = initTarget;

        isDown = false;
        moveSpeed = 110;
    }

    protected override void Update()
    {
        //朝Y轴移动
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.Self);

        if (transform.localPosition.y > 1900 && !isDown)
        {
            isDown = true;
            transform.localRotation = transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));
            transform.localPosition = new Vector3(atkTarget.localPosition.x, transform.localPosition.y);
            posY = atkTarget.localPosition.y;
        }
        else if (isDown && transform.localPosition.y < posY)
        {
            //到达攻击点
            PlayAnimMgr.Instance.PlayBulleExplosionAnim(transform.localPosition, transform.parent);
            PoolMgr.Instance.PushObj(gameObject);
        }
    }
}
