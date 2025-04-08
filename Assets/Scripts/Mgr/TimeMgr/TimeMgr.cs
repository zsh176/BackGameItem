using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Timrdata
{
    public int id;
    public float currTime;
    public float targetTime;
    public Action action;

    public Timrdata(int id, float targetTime, Action action = null)
    {
        this.id = id;
        this.targetTime = targetTime;
        this.action = action;
    }
    public void Update(float time)
    {
        currTime += time;
        if (currTime > targetTime)
        {
            action?.Invoke();
            TimeMgr.Instance.RemoveTime(id);
        }
    }
}

public class TimeMgr : BaseManager<TimeMgr>
{
    private List<Timrdata> DicTime = new List<Timrdata>();

    /// <summary>
    /// ��ʱ�� ���������ʲô��ί��
    /// </summary>
    /// <param name="delayTime">����ʱ���</param>
    /// <param name="action">����ʱ�����ص�</param>
    /// <returns></returns>
    public int AddTime(float delayTime, Action action = null)
    {
        int indexID;
        while (true)
        {
            indexID = Random.Range(0, int.MaxValue);
            if (DicTime.Find((t) => t.id == indexID) == null)
                break;
        }
        DicTime.Add(new Timrdata(indexID, delayTime, action));
        return indexID;
    }
    /// <summary>
    /// ����ĳ����ʱ����ID�����Ƴ������ʱ��
    /// </summary>
    /// <param name="id"></param>
    public void RemoveTime(int id)
    {
        Timrdata timrdata = DicTime.Find((t) => t.id == id);
        if (timrdata != null)
        {
            DicTime.Remove(timrdata);
        }
    }
    private void Update()
    {
        for (int i = 0; i < DicTime.Count; i++)
        {
            DicTime[i].Update(Time.deltaTime);
        }
    }
}

