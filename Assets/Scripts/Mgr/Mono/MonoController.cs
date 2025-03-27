using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoController : MonoBehaviour
{
    public event UnityAction updateEvent;

    void Start()
    {
        //���������Ƴ�
        DontDestroyOnLoad(gameObject);
    }
    void Update()
    {
        if (updateEvent != null)
        {
            updateEvent();
        }
    }

    /// <summary>
    /// ���ⲿ�ṩ�� ���֡�����¼��ĺ���
    /// </summary>
    public void AddUpdateListener(UnityAction fun)
    {
        updateEvent += fun;
    }

    /// <summary>
    /// �ṩ���ⲿ �����Ƴ�֡�����¼�����
    /// </summary>
    public void RemoveUpdateListener(UnityAction fun)
    {
        updateEvent -= fun;
    }
}
