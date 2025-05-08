using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bullet_Hero_10005 : BulletBase
{
    private List<Transform> enemyAllList;//���������е��б�
    private SkeletonGraphic spineAnim;//��������
    private Animator _animator;
    private bool isPlayAtkTimr;//�����������ٿ�ʼ���Ƽ�ʱ����
    private float atkCooling = 2.1f;//�������
    private float timeAtkCooling;//��ʱ�������
    private int atkFew = 4;//�������κ�����Լ�
    private int atkFewTime;//��¼�����˼���

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

        });//�ȿ����

        EventMgr.Instance.EventTrigger<Transform>(E_EventType.addBulletAll, transform);
        EventMgr.Instance.AddEventListener<List<Transform>>(E_EventType.chaEnemyList, ChaEnemyList);
    }
    //�����Լ�
    private void PushObj()
    {
        //��¼���д�죬�ڹ���ʱͳһ���գ���Ϊ����Ƴ��¼��ͻ����Լ���ͬһ�������ᱨ�����⴦��
        EventMgr.Instance.EventTrigger<Transform>(E_EventType.removeBulletAll, transform);

        EventMgr.Instance.RemoveEventListener<List<Transform>>(E_EventType.chaEnemyList, ChaEnemyList);
        PlayAnimMgr.Instance.PlayBullePushAnim(transform.localPosition, transform.parent);
        PoolMgr.Instance.PushObj(gameObject);
    }
    //���⴦��
    public void PushObjGanme()
    {
        EventMgr.Instance.RemoveEventListener<List<Transform>>(E_EventType.chaEnemyList, ChaEnemyList);
        PlayAnimMgr.Instance.PlayBullePushAnim(transform.localPosition, transform.parent);
        PoolMgr.Instance.PushObj(gameObject);
    }

    /// <summary>
    /// ���µ�������
    /// </summary>
    private void ChaEnemyList(List<Transform> allEnemy)
    {
        enemyAllList = allEnemy;
    }
    /// <summary>
    /// ���Ŷ�������
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

        //�������ɽ���Զ����
        var sorted = enemyAllList
            .OrderBy(e => (e.localPosition - myPos).sqrMagnitude)
            .ToList();
        //ȡǰ4
        int takeCount = Mathf.Min(4, sorted.Count);
        //������ʱ����
        var nearest20 = sorted.Take(takeCount).ToList();
        //�����ȡ
        Attack(nearest20[Random.Range(0, nearest20.Count)]);
    }
    /// <summary>
    /// ����
    /// </summary>
    private void Attack(Transform target)
    {
        if (target == null || !target.gameObject.activeSelf)
        {   //���ȥ��·�ϵ��˾����ˣ�������һ��Ŀ��
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
            //�ƶ���������� �ص�
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
                //������ͬ��
                TimeMgr.Instance.AddTime(0.35f, () =>
                {
                    if (target != null && target.gameObject.activeSelf)
                        target.GetComponent<EnemyBase>().EnemyBeAtk(atkValue);
                });
                target.GetComponent<EnemyBase>().EnemyBeAtk(atkValue);
                timeAtkCooling = atkCooling;
                //Debug.Log($"�������˺� {atkValue}");
            }
            else
            {
                //���ȥ��·�ϵ��˾����ˣ���ȴ����һ��
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
