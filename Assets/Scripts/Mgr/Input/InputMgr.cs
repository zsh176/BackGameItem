using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Input输入控制模块
/// </summary>
public class InputMgr : InstanceMgr<InputMgr>
{
    /// <summary>
    /// 是否开启监听
    /// </summary>
    private bool isStart = true;

    /* HashSet 最适用于需要快速判断某个元素是否存在于集合中，且不关心元素的顺序，且不允许重复元素的场景。例如：
     * 1，需要快速查找元素的集合。
     * 2，不需要排序，但要确保元素唯一的集合
     */
    private Dictionary<KeyCode, HashSet<UnityAction>> keysOnClickDic = new Dictionary<KeyCode, HashSet<UnityAction>>();

    public InputMgr()
    {
        MonoMgr.Instance.AddUpdateListener(Update);
    }

    /// <summary>
    /// 设置是否开启监听
    /// </summary>
    /// <param name="isOpen"></param>
    public void StartOrEndCheck(bool isOpen)
    {
        isStart = isOpen;
    }
    /// <summary>
    /// 添加监听输入
    /// </summary>
    /// <param name="key">需要监听的key</param>
    /// <param name="onClick"></param>
    public void AddKeyCode(KeyCode key, UnityAction onClick)
    {
        if (!keysOnClickDic.ContainsKey(key))
        {
            keysOnClickDic[key] = new HashSet<UnityAction>();
        }

        //如果有相同的函数，则不会进行添加
        keysOnClickDic[key].Add(onClick);
    }
    /// <summary>
    /// 移除key的监听输入
    /// </summary>
    /// <param name="key">需要取消监听的key</param>
    /// <param name="onClick"></param>
    public void RemoveKeyCode(KeyCode key, UnityAction onClick)
    {
        if (keysOnClickDic.ContainsKey(key))
        {
            keysOnClickDic[key].Remove(onClick);
            if (keysOnClickDic[key].Count == 0)
            {
                keysOnClickDic.Remove(key);
            }
        }
    }

    private void Update()
    {
        if (!isStart) return;

        foreach (var key in keysOnClickDic)
        {
            if (Input.GetKeyDown(key.Key))
            {
                foreach (var action in key.Value)
                {
                    action?.Invoke();
                }
            }
        }
    }

}
