using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ӵ�����
/// </summary>
public abstract class BulletBase : MonoBehaviour
{
    protected float moveSpeed;//�ƶ��ٶ�
    protected int atkValue;//������
    protected Transform target;//����Ŀ��
    [HideInInspector]
    public RectTransform sceneMapBG;//��Ϸ������ͼ����

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
            //������ͼ�����Լ�
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
            Debug.Log($"�����˺�Ϊ��{atkValue}");
            enemys.Add(collision.transform);
            enemys[0].GetComponent<EnemyBase>().EnemyBeAtk(atkValue);
            enemys.Clear();
            PoolMgr.Instance.PushObj(gameObject);
        }
    }
}
