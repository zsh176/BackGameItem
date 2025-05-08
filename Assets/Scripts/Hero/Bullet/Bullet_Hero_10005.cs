using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bullet_Hero_10005 : BulletBase
{
    private List<Transform> enemyAllList;//场景中所有敌列表
    private SkeletonGraphic spineAnim;//骨骼动画
    private Animator _animator;
    private bool isPlayAtkTimr;//攻击结束后再开始倒计计时攻击
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
        target = initTarget;
        enemyAllList = initenemyAll;

        Vector3 targetPos = target.localPosition;

        isPlayAtkTimr = false;
        timeAtkCooling = atkCooling;
        atkFewTime = atkFew;

        if (target.position.x < transform.position.x)
            transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
        else
            transform.localRotation = Quaternion.identity;

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance < 350)
            moveSpeed = 0.8f;
        else if (distance >= 350 && distance < 650)
            moveSpeed = 1.2f;
        else
            moveSpeed = 1.6f;

        _animator.enabled = true;
        spineAnim.AnimationState.SetAnimation(0, EnemyAnimSpineTag.move, true);
        transform.DOLocalMove(targetPos, moveSpeed).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            spineAnim.AnimationState.SetAnimation(0, EnemyAnimSpineTag.stand, true);
            TimeMgr.Instance.AddTime(0.8f, () =>
            {
                Attack(target);
            });

        });//先快后慢

        EventMgr.Instance.EventTrigger<Transform>(E_EventType.addBulletAll, transform);
        EventMgr.Instance.AddEventListener<List<Transform>>(E_EventType.chaEnemyList, ChaEnemyList);
    }
    //回收自己
    private void PushObj()
    {
        //记录所有大鹅，在过关时统一回收，因为大鹅移除事件和回收自己在同一个方法会报错特殊处理
        EventMgr.Instance.EventTrigger<Transform>(E_EventType.removeBulletAll, transform);

        EventMgr.Instance.RemoveEventListener<List<Transform>>(E_EventType.chaEnemyList, ChaEnemyList);
        PlayAnimMgr.Instance.PlayBullePushAnim(transform.localPosition, transform.parent);
        PoolMgr.Instance.PushObj(gameObject);
    }
    //特殊处理
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
        {   //如果去的路上敌人就死了，重新找一个目标
            IsOkAtk();
            return;
        }

        if (target.position.x < transform.position.x)
            transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
        else
            transform.localRotation = Quaternion.identity;

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance < 100)
            moveSpeed = 0.8f;
        else if (distance >= 100 && distance < 350)
            moveSpeed = 1.6f;
        else if (distance >= 350 && distance < 650)
            moveSpeed = 2.2f;
        else
            moveSpeed = 3.8f;

        spineAnim.AnimationState.SetAnimation(0, EnemyAnimSpineTag.move, true);
        transform.DOLocalMove(target.localPosition, moveSpeed).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            //移动到敌人身边 回调
            isPlayAtkTimr = true;
            atkFewTime--;
            if (target != null && target.gameObject.activeSelf)
            {
                spineAnim.AnimationState.SetAnimation(0, EnemyAnimSpineTag.atk, false).Complete += trackEntry =>
                {
                    if (atkFewTime <= 0)
                    {
                        TimeMgr.Instance.AddTime(0.5f, () =>
                        {
                            PushObj();
                        });
                    }
                    spineAnim.AnimationState.AddAnimation(0, EnemyAnimSpineTag.stand, true, 0f);
                };
                //跟动画同步
                TimeMgr.Instance.AddTime(0.35f, () =>
                {
                    if (target != null && target.gameObject.activeSelf)
                        target.GetComponent<EnemyBase>().EnemyBeAtk(atkValue);
                });
                target.GetComponent<EnemyBase>().EnemyBeAtk(atkValue);
                timeAtkCooling = atkCooling;
                //Debug.Log($"大鹅造成伤害 {atkValue}");
            }
            else
            {
                //如果去的路上敌人就死了，冷却缩减一半
                timeAtkCooling = atkCooling / 2;
            }
        });
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
    }
}
