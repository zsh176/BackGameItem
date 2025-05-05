using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

/// <summary>
/// �¼�����
/// </summary>
public class EventMgr : InstanceMgr<EventMgr>
{
    //key(��) ���� �¼��ı�ǩ
    //value(ֵ) ���� ��Ӧ�ļ�������
    private Dictionary<E_EventType, IEventInfo> eventDic = new Dictionary<E_EventType, IEventInfo>();

    /// <summary>
    /// ����¼�����
    /// </summary>
    public void AddEventListener<T>(E_EventType _name, UnityAction<T> action)
    {
        //�ж���û�ж�Ӧ�ļ�
        if (eventDic.ContainsKey(_name))
            (eventDic[_name] as EventInfo<T>).actions.Add(action);
        else//û�оͼ�һ���� ���а��¼���ӽ�ȥ
            eventDic.Add(_name, new EventInfo<T>(action));
    }
    /// <summary>
    /// ����¼�����
    /// </summary>
    public void AddEventListener(E_EventType _name, UnityAction action)
    {
        //�ж���û�ж�Ӧ�ļ�
        if (eventDic.ContainsKey(_name))
            (eventDic[_name] as EventInfo).actions.Add(action);
        else//û�оͼ�һ���� ���а��¼���ӽ�ȥ
            eventDic.Add(_name, new EventInfo(action));
    }

    /// <summary>
    /// �Ƴ��¼�����
    /// </summary>
    public void RemoveEventListener<T>(E_EventType _name, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(_name))
            (eventDic[_name] as EventInfo<T>).actions.Remove(action);
    }
    /// <summary>
    /// �Ƴ��¼�����
    /// </summary>
    public void RemoveEventListener(E_EventType _name, UnityAction action)
    {
        if (eventDic.ContainsKey(_name))
            (eventDic[_name] as EventInfo).actions.Remove(action);
    }

    /// <summary>
    /// �¼�����
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
    /// �¼�����
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
    /// ����¼�����(�ֵ�) 
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }
}



/// <summary>
/// ����װ�����
/// </summary>
public interface IEventInfo
{

}

public class EventInfo<T> : IEventInfo
{
    /* HashSet ����������Ҫ�����ж�ĳ��Ԫ���Ƿ�����ڼ����У��Ҳ�����Ԫ�ص�˳���Ҳ������ظ�Ԫ�صĳ��������磺
    * 1����Ҫ���ٲ���Ԫ�صļ��ϡ�
    * 2������Ҫ���򣬵�Ҫȷ��Ԫ��Ψһ�ļ���
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