using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

/// <summary>
/// 敌人基类
/// </summary>
public abstract class EnemyBase : MonoBehaviour
{
    /// <summary>
    /// 怪物标签 在Inspector窗口设置
    /// </summary>
    public EnemyType type;
    public bool isBoss;//是否是Boss

    private Vector3 moveTargetPos;//移动的目标位置
    protected SkeletonGraphic spineAnim;//骨骼动画

    protected EnemyDataInfo enemyDataInfo;

    private bool stopMove;//停止移动
    private bool isDeath;//是否死亡
    protected float moveSpeed;//移动速度
    protected int atkValue;//攻击力
    protected float atkCooling;//攻击间隔
    protected float timeAtkCooling;//记时攻击间隔
    protected int maxHP;//最大血量
    protected int nowHP;//当前血量


    protected virtual void Awake()
    {
        spineAnim = transform.Find("spine_Anim").GetComponent<SkeletonGraphic>();
        enemyDataInfo = FileSystemMgr.Instance.enemyData.typeData[type];
    }

    public virtual void Init(Vector2 birthpos, Transform hero)
    {
        transform.localScale = Vector3.one;
        transform.localPosition = birthpos;
        spineAnim.AnimationState.SetAnimation(0, EnemyAnimSpineTag.move, true);
        moveTargetPos = hero.localPosition;

        if (isBoss)
        {
            if (transform.localPosition.x < moveTargetPos.x)
                transform.localRotation = new Quaternion(0, 180, 0, 0);
        }
        else
        {
            if (transform.localPosition.x > moveTargetPos.x)
                transform.localRotation = new Quaternion(0, 180, 0, 0);
        }
        moveSpeed = enemyDataInfo.moveSpeed;
        atkCooling = enemyDataInfo.atkCooling;
        atkValue = enemyDataInfo.atkValue;
        maxHP = enemyDataInfo.maxHP;
        nowHP = maxHP;
        isDeath = false;

        //TimeMgr.Instance.AddTime(UnityEngine.Random.Range(6,12), () =>
        //{
        //    Death();
        //});

        EventMgr.Instance.AddEventListener(E_EventType.skipLevel, Death);
    }
    /// <summary>
    /// 回收至缓存池
    /// </summary>
    protected virtual void PushObj()
    {
        stopMove = false;
        transform.localRotation = Quaternion.identity;
        transform.localPosition = new Vector3(2000, 2000);
        EventMgr.Instance.EventTrigger<Transform>(E_EventType.enemyDeath, transform);
        PoolMgr.Instance.PushObj(gameObject);
        //移除事件放在最后面，避免还没回收就移除了事件
        EventMgr.Instance.RemoveEventListener(E_EventType.skipLevel, Death);
    }

    /// <summary>
    /// 发起攻击
    /// </summary>
    protected virtual void Attack()
    {
        // 先播放攻击动画，结束后自动播放待机动画
        spineAnim.AnimationState.SetAnimation(0, EnemyAnimSpineTag.atk, false);
        spineAnim.AnimationState.AddAnimation(0, EnemyAnimSpineTag.stand, true, 0f);
        BeAtkData data = new BeAtkData()
        {
            harm = atkValue,
            pos = transform.position,
        };
        EventMgr.Instance.EventTrigger<BeAtkData>(E_EventType.playerBeAtk, data);

        timeAtkCooling = atkCooling;
    }
    /// <summary>
    /// 敌人被攻击
    /// </summary>
    public virtual void EnemyBeAtk(int harm)
    {
        if (isDeath) return;
        nowHP -= harm;
        if (nowHP <= 0)
        {
            Death();
        }
    }
    /// <summary>
    /// 死亡
    /// </summary>
    public virtual void Death()
    {
        isDeath = true;
        spineAnim.AnimationState.SetAnimation(0, EnemyAnimSpineTag.death, false).Complete += trackEntry =>
        {
            PushObj();
        };
    }
    
    /// <summary>
    /// 停止移动开始攻击
    /// </summary>
    protected virtual void StopMoveStartAtk()
    {
        stopMove = true;
        Attack();
    }

    protected virtual void Update()
    {
        if (isDeath) return;

        if (!stopMove)
        {
            //匀速移动
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, moveTargetPos, Time.deltaTime * moveSpeed);
        }
        else
        {
            if((timeAtkCooling -= Time.deltaTime) < 0)
            {
                Attack();
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag(StaticFields.MapHeroBGTag))
        {
            StopMoveStartAtk();
        }
    }

}

/// <summary>
/// 敌人标签
/// </summary>
public enum EnemyType
{
    Enemy_Ordinary01,
    Enemy_Ordinary02, 
    Enemy_Ordinary03,
    Enemy_Ordinary04,
    Enemy_Ordinary05,
    Enemy_Boss01,
    Enemy_Boss02,
}

public struct EnemyAnimSpineTag
{
    /// <summary>
    /// 待机
    /// </summary>
    public const string stand = "stand";
    /// <summary>
    /// 移动
    /// </summary>
    public const string move = "run";
    /// <summary>
    /// 攻击
    /// </summary>
    public const string atk = "hit";
    /// <summary>
    /// 死亡
    /// </summary>
    public const string death = "die";
}
