using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoController : MonoBehaviour
{
    public event UnityAction updateEvent;

    void Start()
    {
        //过场景不移除
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
    /// 给外部提供的 添加帧更新事件的函数
    /// </summary>
    public void AddUpdateListener(UnityAction fun)
    {
        updateEvent += fun;
    }

    /// <summary>
    /// 提供给外部 用于移除帧更新事件函数
    /// </summary>
    public void RemoveUpdateListener(UnityAction fun)
    {
        updateEvent -= fun;
    }
}
