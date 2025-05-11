using DG.Tweening;
using Spine.Unity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 养鹅叔子弹
/// </summary>
public class Bullet_Hero_10005 : BulletBase
{
    private List<Transform> enemyAllList;//场景中所有敌列表
    private SkeletonGraphic spineAnim;//骨骼动画
    private Animator _animator;
    private bool isPlayAtkTimr;//攻击结束后再开始倒计计时攻击
    private bool moveAtkTarget;//是否处于移动到攻击目标过程中
    private float atkCooling = 2.1f;//攻击间隔
    private float timeAtkCooling;//记时攻击间隔
    private int atkFew = 4;//攻击几次后回收自己
    private int atkFewTime;//记录攻击了几次

    protected override void Awake()
    {
        spineAnim = transform.Find("spine_Anim").GetComponent<SkeletonGraphic>();
        _animator = GetComponent<Animator>();
    }

    public void Init(Vector3 initPos, int initatkvalus, Transform initTarget, List<Transform> initenemyAll)
    {
        transform.position = initPos;
        atkValue = initatkvalus;
        atkTarget = initTarget;
        enemyAllList = initenemyAll;


        isPlayAtkTimr = false;
        timeAtkCooling = atkCooling;
        atkFewTime = atkFew;

        SetRotation();
         moveSpeed = GetMoveSpeed(2.6f);
        _animator.enabled = true;
        spineAnim.AnimationState.SetAnimation(0, EnemyAnimSpineTag.move, true);
        transform.DOLocalMove(atkTarget.localPosition, moveSpeed).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            if (atkTarget.gameObject.activeSelf)
            {
                spineAnim.AnimationState.SetAnimation(0, EnemyAnimSpineTag.stand, true);
                TimeMgr.Instance.AddTime(0.7f, () =>
                {
                    if (atkTarget.gameObject.activeSelf)
                        Attack(atkTarget);
                    else
                        StopAtk();
                });
            }
            else
                StopAtk();
        });//先快后慢

        EventMgr.Instance.EventTrigger<Transform>(E_EventType.addBulletAll, transform);
        EventMgr.Instance.AddEventListener<List<Transform>>(E_EventType.chaEnemyList, ChaEnemyList);
    }
    //回收自己
    private void PushObj()
    {
        //记录所有大鹅，因为大鹅移除事件和回收自己在同一个方法会报错特殊处理
        EventMgr.Instance.EventTrigger<Transform>(E_EventType.removeBulletAll, transform);

        EventMgr.Instance.RemoveEventListener<List<Transform>>(E_EventType.chaEnemyList, ChaEnemyList);
        PlayAnimMgr.Instance.PlayBullePushAnim(transform.localPosition, transform.parent);
        PoolMgr.Instance.PushObj(gameObject);
    }
    //特殊处理，在过关时统一回收
    public void PushObjGanme()
    {
        EventMgr.Instance.RemoveEventListener<List<Transform>>(E_EventType.chaEnemyList, ChaEnemyList);
        PlayAnimMgr.Instance.PlayBullePushAnim(transform.localPosition, transform.parent);
        PoolMgr.Instance.PushObj(gameObject);
    }

    /// <summary>
    /// 更新敌人数组
    /// </summary>
    private void ChaEnemyList(List<Transform> allEnemy)
    {
        enemyAllList = allEnemy;
    }
    /// <summary>
    /// 缩放动画结束
    /// </summary>
    public void OnCilckAnimSale()
    {
        _animator.enabled = false;
    }

    /// <summary>
    /// 选择攻击目标
    /// </summary>
    protected void IsOkAtk()
    {
        if (enemyAllList == null || enemyAllList.Count == 0)
            return;
        Vector3 myPos = transform.localPosition;
        //按距离由近及远排序
        var sorted = enemyAllList
            .OrderBy(e => (e.localPosition - myPos).sqrMagnitude)
            .ToList();
        //取前4
        int takeCount = Mathf.Min(4, sorted.Count);
        //放入临时数组
        var nearest20 = sorted.Take(takeCount).ToList();
        //随机获取
        Attack(nearest20[Random.Range(0, nearest20.Count)]);
    }
    /// <summary>
    /// 攻击
    /// </summary>
    private void Attack(Transform target)
    {
        if (target == null || !target.gameObject.activeSelf)
        {   //如果敌人就死了，重新找一个目标
            IsOkAtk();
            return;
        }
        atkTarget = target;

        SetRotation();

        if (IsDistanceMin())
            StartAtk();
        else
        {
            moveAtkTarget = true;
            moveSpeed = GetMoveSpeed();
            spineAnim.AnimationState.SetAnimation(0, EnemyAnimSpineTag.move, true);
            transform.DOLocalMove(target.localPosition, moveSpeed).SetEase(Ease.OutQuad);
        }
    }
    /// <summary>
    /// 发起攻击
    /// </summary>
    private void StartAtk()
    {
        SetRotation();
        isPlayAtkTimr = true;
        moveAtkTarget = false;
        if (atkTarget != null && atkTarget.gameObject.activeSelf)
        {
            atkFewTime--;
            spineAnim.AnimationState.SetAnimation(0, EnemyAnimSpineTag.atk, false).Complete += trackEntry =>
            {
                if (atkFewTime <= 0)
                {
                    TimeMgr.Instance.AddTime(0.5f, () =>
                    {
                        PushObj();
                    });
                }
                //动画结束再造成伤害
                if (atkTarget.gameObject.activeSelf)
                    atkTarget.GetComponent<EnemyBase>().EnemyBeAtk(atkValue);
                spineAnim.AnimationState.AddAnimation(0, EnemyAnimSpineTag.stand, true, 0f);
            };
            timeAtkCooling = atkCooling;
        }
        else
            StopAtk();
    }
    /// <summary>
    /// 如果去的路上敌人就死了，冷却缩减一半
    /// </summary>
    private void StopAtk()
    {
        //停止移动动画
        transform.DOKill();
        moveAtkTarget = false;
        isPlayAtkTimr = true;
        timeAtkCooling = atkCooling / 2;
        spineAnim.AnimationState.SetAnimation(0, EnemyAnimSpineTag.stand, true);
    }
    /// <summary>
    /// 设置方向
    /// </summary>
    private void SetRotation()
    {
        if (atkTarget.position.x < transform.position.x)
            transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
        else
            transform.localRotation = Quaternion.identity;
    }
    /// <summary>
    /// 根据自身和敌人的距离，获取移动速度
    /// </summary>
    private float GetMoveSpeed(float maxSpedd = 4.5f)
    {
        float distance = Vector3.Distance(transform.localPosition, atkTarget.localPosition);
        // 把距离 0~2000 映射到 0~1 区间，如果距离超过2000 就按照2000计算
        float t = Mathf.Clamp01(distance / 2000f);
        // 再把 0~1 映射 到0~4.5 的移动速度
        return Mathf.Lerp(0, maxSpedd, t);
    }
    /// <summary>
    /// 和攻击目标的距离是否小于150
    /// </summary>
    /// <returns></returns>
    private bool IsDistanceMin()
    {
        return Vector3.Distance(transform.localPosition, atkTarget.localPosition) < 200;
    }

    protected override void Update()
    {
        if (isPlayAtkTimr)
        {
            if (timeAtkCooling > 0)
                timeAtkCooling -= Time.deltaTime;
            else
            {
                isPlayAtkTimr = false;
                IsOkAtk();
            }
        }
        if (moveAtkTarget)
        {
            if (!atkTarget.gameObject.activeSelf)
                StopAtk();
            else if (IsDistanceMin())
                StartAtk();
        }
    }
}
