using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// �¼�����
/// </summary>
public class EventCenter : InstanceMgr<EventCenter>
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
        {
            // ��ȡ��ǰ�¼��� actions
            var eventInfo = eventDic[_name] as EventInfo<T>;
            // �������ͬ�ĺ����������
            if (eventInfo.actions == null || !eventInfo.actions.GetInvocationList().Contains(action))
                eventInfo.actions += action;
        }
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
        {
            // ��ȡ��ǰ�¼��� actions
            var eventInfo = eventDic[_name] as EventInfo;
            // �������ͬ�ĺ����������
            if (eventInfo.actions == null || !eventInfo.actions.GetInvocationList().Contains(action))
                eventInfo.actions += action;
        }
        else//û�оͼ�һ���� ���а��¼���ӽ�ȥ
            eventDic.Add(_name, new EventInfo(action));
    }

    /// <summary>
    /// �Ƴ��¼�����
    /// </summary>
    public void RemoveEventListener<T>(E_EventType _name, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(_name)) 
            (eventDic[_name] as EventInfo<T>).actions -= action;
    }
    /// <summary>
    /// �Ƴ��¼�����
    /// </summary>
    public void RemoveEventListener(E_EventType _name, UnityAction action)
    {
        if (eventDic.ContainsKey(_name)) 
            (eventDic[_name] as EventInfo).actions -= action;
    }

    /// <summary>
    /// �¼�����
    /// </summary>
    /// <param name="name"></param>
    public void EventTrigger<T>(E_EventType _name, T info)
    {
        if (eventDic.ContainsKey(_name))
            (eventDic[_name] as EventInfo<T>).actions?.Invoke(info);
    }
    /// <summary>
    /// �¼�����
    /// </summary>
    public void EventTrigger(E_EventType _name)
    {
        if (eventDic.ContainsKey(_name))
            (eventDic[_name] as EventInfo).actions?.Invoke();
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