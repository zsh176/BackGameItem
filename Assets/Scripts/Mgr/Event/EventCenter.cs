using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 事件中心
/// </summary>
public class EventCenter : InstanceMgr<EventCenter>
{
    //key(键) —— 事件的标签
    //value(值) —— 对应的监听函数
    private Dictionary<E_EventType, IEventInfo> eventDic = new Dictionary<E_EventType, IEventInfo>();

    /// <summary>
    /// 添加事件监听
    /// </summary>
    public void AddEventListener<T>(E_EventType _name, UnityAction<T> action)
    {
        //判断有没有对应的键
        if (eventDic.ContainsKey(_name))
        {
            // 获取当前事件的 actions
            var eventInfo = eventDic[_name] as EventInfo<T>;
            // 如果有相同的函数，则不添加
            if (eventInfo.actions == null || !eventInfo.actions.GetInvocationList().Contains(action))
                eventInfo.actions += action;
        }
        else//没有就加一个键 并切把事件添加进去
            eventDic.Add(_name, new EventInfo<T>(action));
    }
    /// <summary>
    /// 添加事件监听
    /// </summary>
    public void AddEventListener(E_EventType _name, UnityAction action)
    {
        //判断有没有对应的键
        if (eventDic.ContainsKey(_name))
        {
            // 获取当前事件的 actions
            var eventInfo = eventDic[_name] as EventInfo;
            // 如果有相同的函数，则不添加
            if (eventInfo.actions == null || !eventInfo.actions.GetInvocationList().Contains(action))
                eventInfo.actions += action;
        }
        else//没有就加一个键 并切把事件添加进去
            eventDic.Add(_name, new EventInfo(action));
    }

    /// <summary>
    /// 移除事件监听
    /// </summary>
    public void RemoveEventListener<T>(E_EventType _name, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(_name)) 
            (eventDic[_name] as EventInfo<T>).actions -= action;
    }
    /// <summary>
    /// 移除事件监听
    /// </summary>
    public void RemoveEventListener(E_EventType _name, UnityAction action)
    {
        if (eventDic.ContainsKey(_name)) 
            (eventDic[_name] as EventInfo).actions -= action;
    }

    /// <summary>
    /// 事件触发
    /// </summary>
    /// <param name="name"></param>
    public void EventTrigger<T>(E_EventType _name, T info)
    {
        if (eventDic.ContainsKey(_name))
            (eventDic[_name] as EventInfo<T>).actions?.Invoke(info);
    }
    /// <summary>
    /// 事件触发
    /// </summary>
    public void EventTrigger(E_EventType _name)
    {
        if (eventDic.ContainsKey(_name))
            (eventDic[_name] as EventInfo).actions?.Invoke();
    }

    /// <summary>
    /// 清空事件中心(字典) 
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }
}



/// <summary>
/// 避免装箱拆箱
/// </summary>
public interface IEventInfo
{

}

public class EventInfo<T> : IEventInfo
{
    public UnityAction<T> actions;

    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }
}

public class EventInfo : IEventInfo
{
    public UnityAction actions;

    public EventInfo(UnityAction action)
    {
        actions += action;
    }
}