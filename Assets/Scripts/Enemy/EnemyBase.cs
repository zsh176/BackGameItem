using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

/// <summary>
/// ���˻���
/// </summary>
public abstract class EnemyBase : MonoBehaviour
{
    /// <summary>
    /// �����ǩ ��Inspector��������
    /// </summary>
    public EnemyType type;
    public bool isBoss;//�Ƿ���Boss

    private Vector3 moveTargetPos;//�ƶ���Ŀ��λ��
    protected SkeletonGraphic spineAnim;//��������

    protected EnemyDataInfo enemyDataInfo;

    private bool stopMove;//ֹͣ�ƶ�
    private bool isDeath;//�Ƿ�����
    protected float moveSpeed;//�ƶ��ٶ�
    protected int atkValue;//������
    protected float atkCooling;//�������
    protected float timeAtkCooling;//��ʱ�������
    protected int maxHP;//���Ѫ��
    protected int nowHP;//��ǰѪ��


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
    /// �����������
    /// </summary>
    protected virtual void PushObj()
    {
        stopMove = false;
        transform.localRotation = Quaternion.identity;
        transform.localPosition = new Vector3(2000, 2000);
        EventMgr.Instance.EventTrigger<Transform>(E_EventType.enemyDeath, transform);
        PoolMgr.Instance.PushObj(gameObject);
        //�Ƴ��¼���������棬���⻹û���վ��Ƴ����¼�
        EventMgr.Instance.RemoveEventListener(E_EventType.skipLevel, Death);
    }

    /// <summary>
    /// ���𹥻�
    /// </summary>
    protected virtual void Attack()
    {
        // �Ȳ��Ź����������������Զ����Ŵ�������
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
    /// ���˱�����
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
    /// ����
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
    /// ֹͣ�ƶ���ʼ����
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
            //�����ƶ�
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
/// ���˱�ǩ
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
    /// ����
    /// </summary>
    public const string stand = "stand";
    /// <summary>
    /// �ƶ�
    /// </summary>
    public const string move = "run";
    /// <summary>
    /// ����
    /// </summary>
    public const string atk = "hit";
    /// <summary>
    /// ����
    /// </summary>
    public const string death = "die";
}
