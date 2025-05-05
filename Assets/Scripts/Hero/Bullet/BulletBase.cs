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
    [HideInInspector]
    public RectTransform sceneMapBG;//游戏场景地图背景

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
        sceneMapBG = initsceneMapBG;
    }

    protected virtual void Update()
    {
        if (IsOutOfScreen())
        {
            //超出地图回收自己
            enemys.Clear();
            PoolMgr.Instance.PushObj(gameObject);
        }
    }
    bool IsOutOfScreen()
    {
        if (transform.localPosition.y > sceneMapBG.rect.height / 2 || transform.localPosition.y < -(sceneMapBG.rect.height / 2)
           || transform.localPosition.x > sceneMapBG.rect.width / 2 || transform.localPosition.x < -(sceneMapBG.rect.width / 2))
        {
            return true;
        }
        return false;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(StaticFields.Enemy))
        {
            Debug.Log($"攻击伤害为：{atkValue}");
            enemys.Add(collision.transform);
            enemys[0].GetComponent<EnemyBase>().EnemyBeAtk(atkValue);
            enemys.Clear();
            PoolMgr.Instance.PushObj(gameObject);
        }
    }
}
