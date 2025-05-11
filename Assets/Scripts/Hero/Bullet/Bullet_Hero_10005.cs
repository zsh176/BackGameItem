using DG.Tweening;
using Spine.Unity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// �������ӵ�
/// </summary>
public class Bullet_Hero_10005 : BulletBase
{
    private List<Transform> enemyAllList;//���������е��б�
    private SkeletonGraphic spineAnim;//��������
    private Animator _animator;
    private bool isPlayAtkTimr;//�����������ٿ�ʼ���Ƽ�ʱ����
    private bool moveAtkTarget;//�Ƿ����ƶ�������Ŀ�������
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
        });//�ȿ����

        EventMgr.Instance.EventTrigger<Transform>(E_EventType.addBulletAll, transform);
        EventMgr.Instance.AddEventListener<List<Transform>>(E_EventType.chaEnemyList, ChaEnemyList);
    }
    //�����Լ�
    private void PushObj()
    {
        //��¼���д�죬��Ϊ����Ƴ��¼��ͻ����Լ���ͬһ�������ᱨ�����⴦��
        EventMgr.Instance.EventTrigger<Transform>(E_EventType.removeBulletAll, transform);

        EventMgr.Instance.RemoveEventListener<List<Transform>>(E_EventType.chaEnemyList, ChaEnemyList);
        PlayAnimMgr.Instance.PlayBullePushAnim(transform.localPosition, transform.parent);
        PoolMgr.Instance.PushObj(gameObject);
    }
    //���⴦���ڹ���ʱͳһ����
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

    /// <summary>
    /// ѡ�񹥻�Ŀ��
    /// </summary>
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
        {   //������˾����ˣ�������һ��Ŀ��
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
    /// ���𹥻�
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
                //��������������˺�
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
    /// ���ȥ��·�ϵ��˾����ˣ���ȴ����һ��
    /// </summary>
    private void StopAtk()
    {
        //ֹͣ�ƶ�����
        transform.DOKill();
        moveAtkTarget = false;
        isPlayAtkTimr = true;
        timeAtkCooling = atkCooling / 2;
        spineAnim.AnimationState.SetAnimation(0, EnemyAnimSpineTag.stand, true);
    }
    /// <summary>
    /// ���÷���
    /// </summary>
    private void SetRotation()
    {
        if (atkTarget.position.x < transform.position.x)
            transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
        else
            transform.localRotation = Quaternion.identity;
    }
    /// <summary>
    /// ��������͵��˵ľ��룬��ȡ�ƶ��ٶ�
    /// </summary>
    private float GetMoveSpeed(float maxSpedd = 4.5f)
    {
        float distance = Vector3.Distance(transform.localPosition, atkTarget.localPosition);
        // �Ѿ��� 0~2000 ӳ�䵽 0~1 ���䣬������볬��2000 �Ͱ���2000����
        float t = Mathf.Clamp01(distance / 2000f);
        // �ٰ� 0~1 ӳ�� ��0~4.5 ���ƶ��ٶ�
        return Mathf.Lerp(0, maxSpedd, t);
    }
    /// <summary>
    /// �͹���Ŀ��ľ����Ƿ�С��150
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
