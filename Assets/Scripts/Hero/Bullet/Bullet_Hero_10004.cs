using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��С���ӵ�
/// </summary>
public class Bullet_Hero_10004 : BulletBase
{
    private List<Transform> enemyAllList;//���������е��б�
    private bool isDown;//�Ƿ�ʼ�½�
    private float posY;
    private bool isPush;//�Ƿ����
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
    /// ���µ�������
    /// </summary>
    private void ChaEnemyList(List<Transform> allEnemy)
    {
        enemyAllList = allEnemy;
    }

    /// <summary>
    /// ���﹥��λ�ã���ը�������Լ�
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
                    //�ڱ�ը��Χ�ڵĵ��ˣ�����˺�
                    item.GetComponent<EnemyBase>().EnemyBeAtk(atkValue);
                }
            }
        }
        PoolMgr.Instance.PushObj(gameObject);
    }

    protected override void Update()
    {
        //��Y���ƶ�
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
            //���﹥����
            Explosion();
        }
    }
}
