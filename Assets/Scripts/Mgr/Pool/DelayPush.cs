using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�����ڶ�������
public class DelayPush : MonoBehaviour
{
    public float isTime = 1;//��ú��ٰ�����������
    //����ÿ�μ����ʱ�򶼻���ú���
    void OnEnable()
    {
        Invoke("Push", isTime);
    }

    //����������Ŷ���
    public void Push()
    {
        PoolMgr.Instance.PushObj(gameObject.name,gameObject);
    }
}
