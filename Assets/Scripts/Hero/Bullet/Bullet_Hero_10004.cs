using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 王小虎子弹
/// </summary>
public class Bullet_Hero_10004 : BulletBase
{
    private List<Transform> enemyAllList;//场景中所有敌列表
    private bool isDown;//是否开始下降
    private float posY;
    private bool isPush;//是否回收
    protected override void Awake()
    {

    }

    public void Init(Vector3 initPos, int initAtkvalu, Transform initTarget , List<Transform> initenemyAll)
    {
        transform.position = initPos;
        transform.localRotation = Quaternion.identity;
        atkValue = initAtkvalu;
        atkTarget = initTarget;
        enemyAllList = initenemyAll;

        isDown = false;
        isPush = false;
        moveSpeed = 110;

        EventMgr.Instance.AddEventListener<List<Transform>>(E_EventType.chaEnemyList, ChaEnemyList);
    }
    /// <summary>
    /// 更新敌人数组
    /// </summary>
    private void ChaEnemyList(List<Transform> allEnemy)
    {
        enemyAllList = allEnemy;
    }

    /// <summary>
    /// 到达攻击位置，爆炸并回收自己
    /// </summary>
    private void Explosion()
    {
        if (isPush) return;
        isPush = true;
        EventMgr.Instance.RemoveEventListener<List<Transform>>(E_EventType.chaEnemyList, ChaEnemyList);
        PlayAnimMgr.Instance.PlayBulleExplosionAnim(transform.localPosition, transform.parent);
        EventMgr.Instance.EventTrigger(E_EventType.playShaking);
        foreach (var item in enemyAllList)
        {
            if (item.gameObject.activeSelf)
            {
                if(Vector3.Distance(transform.localPosition, item.localPosition) < 170)
                {
                    //在爆炸范围内的敌人，造成伤害
                    item.GetComponent<EnemyBase>().EnemyBeAtk(atkValue);
                }
            }
        }
        PoolMgr.Instance.PushObj(gameObject);
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
        else if (isDown && transform.localPosition.y < posY && !isPush)
        {
            //到达攻击点
            Explosion();
        }
    }
}
