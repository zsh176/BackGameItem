using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 子弹基类
/// </summary>
public abstract class BulletBase : MonoBehaviour
{
    protected float moveSpeed;//移动速度
    protected int atkValue;//攻击力
    protected Transform target;//攻击目标
    protected bool isOver;//攻击结束

    protected float mapBGheight = 1500;//游戏场景背景高度的 2/1
    protected float mapBGwidth = 1500;//游戏场景背景宽度的 2/1

    protected List<Transform> enemys;

    protected virtual void Awake()
    {
        enemys = new List<Transform>();
    }

    public virtual void Init(Vector3 initPos, Quaternion initRotation, int atkvalus, RectTransform initsceneMapBG)
    {
        transform.position = initPos;
        transform.rotation = initRotation;
        atkValue = atkvalus;
        mapBGheight = (initsceneMapBG.rect.height / 2) + 200;
        mapBGwidth = (initsceneMapBG.rect.width / 2) + 200;
        isOver = false;
    }

    protected virtual void Update()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.Self);
        if (IsOutOfScreen())
        {
            //超出地图背景就回收自己
            enemys.Clear();
            PoolMgr.Instance.PushObj(gameObject);
        }
    }
    bool IsOutOfScreen()
    {
        if (transform.localPosition.y > mapBGheight || transform.localPosition.y < -mapBGheight
           || transform.localPosition.x > mapBGwidth || transform.localPosition.x < -mapBGwidth)
        {
            return true;
        }
        return false;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
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
