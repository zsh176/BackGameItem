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
    protected Transform atkTarget;//����Ŀ��
    protected bool isOver;//��������
    protected bool isStrike;//�Ƿ񱩻�

    protected float mapBGheight = 1500;//��Ϸ���������߶ȵ� 2/1
    protected float mapBGwidth = 1500;//��Ϸ����������ȵ� 2/1

    protected List<Transform> enemys;

    protected virtual void Awake()
    {
        enemys = new List<Transform>();
    }

    public virtual void Init(Vector3 initPos, Quaternion initRotation, int atkvalus, RectTransform initsceneMapBG,bool initIsStrike)
    {
        transform.position = initPos;
        transform.rotation = initRotation;
        atkValue = atkvalus;
        mapBGheight = (initsceneMapBG.rect.height / 2) + 200;
        mapBGwidth = (initsceneMapBG.rect.width / 2) + 200;
        isOver = false;
        isStrike = initIsStrike;
    }

    protected virtual void Update()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.Self);
        if (IsOutOfScreen())
        {
            //������ͼ�����ͻ����Լ�
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
}
