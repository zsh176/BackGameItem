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
    /// 计时器 结束后想干什么传委托
    /// </summary>
    /// <param name="delayTime">倒计时多久</param>
    /// <param name="action">倒计时结束回调</param>
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
    /// 传入某个计时器的ID用于移除这个计时器
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

