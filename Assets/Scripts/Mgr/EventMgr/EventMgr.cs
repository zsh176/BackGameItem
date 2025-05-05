using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

/// <summary>
/// 事件中心
/// </summary>
public class EventMgr : InstanceMgr<EventMgr>
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
            (eventDic[_name] as EventInfo<T>).actions.Add(action);
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
            (eventDic[_name] as EventInfo).actions.Add(action);
        else//没有就加一个键 并切把事件添加进去
            eventDic.Add(_name, new EventInfo(action));
    }

    /// <summary>
    /// 移除事件监听
    /// </summary>
    public void RemoveEventListener<T>(E_EventType _name, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(_name))
            (eventDic[_name] as EventInfo<T>).actions.Remove(action);
    }
    /// <summary>
    /// 移除事件监听
    /// </summary>
    public void RemoveEventListener(E_EventType _name, UnityAction action)
    {
        if (eventDic.ContainsKey(_name))
            (eventDic[_name] as EventInfo).actions.Remove(action);
    }

    /// <summary>
    /// 事件触发
    /// </summary>
    /// <param name="name"></param>
    public void EventTrigger<T>(E_EventType _name, T info)
    {
        if (eventDic.ContainsKey(_name))
        {
            foreach (var action in (eventDic[_name] as EventInfo<T>).actions)
            {
                action?.Invoke(info);
            }
        }
            
    }
    /// <summary>
    /// 事件触发
    /// </summary>
    public void EventTrigger(E_EventType _name)
    {
        if (eventDic.ContainsKey(_name))
        {
            foreach (var action in (eventDic[_name] as EventInfo).actions)
            {
                action?.Invoke();
            }
        }
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
    /* HashSet 最适用于需要快速判断某个元素是否存在于集合中，且不关心元素的顺序，且不允许重复元素的场景。例如：
    * 1，需要快速查找元素的集合。
    * 2，不需要排序，但要确保元素唯一的集合
    */
    public List<UnityAction<T>> actions;

    public EventInfo(UnityAction<T> action)
    {
        actions = new List<UnityAction<T>>() { action };
    }
}

public class EventInfo : IEventInfo
{
    public List<UnityAction> actions;

    public EventInfo(UnityAction action)
    {
        actions = new List<UnityAction>() { action };
    }
}