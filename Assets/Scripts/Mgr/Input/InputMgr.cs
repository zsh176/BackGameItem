using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Input�������ģ��
/// </summary>
public class InputMgr : InstanceMgr<InputMgr>
{
    /// <summary>
    /// �Ƿ�������
    /// </summary>
    private bool isStart = true;

    /* HashSet ����������Ҫ�����ж�ĳ��Ԫ���Ƿ�����ڼ����У��Ҳ�����Ԫ�ص�˳���Ҳ������ظ�Ԫ�صĳ��������磺
     * 1����Ҫ���ٲ���Ԫ�صļ��ϡ�
     * 2������Ҫ���򣬵�Ҫȷ��Ԫ��Ψһ�ļ���
     */
    private Dictionary<KeyCode, HashSet<UnityAction>> keysOnClickDic = new Dictionary<KeyCode, HashSet<UnityAction>>();

    public InputMgr()
    {
        MonoMgr.Instance.AddUpdateListener(Update);
    }

    /// <summary>
    /// �����Ƿ�������
    /// </summary>
    /// <param name="isOpen"></param>
    public void StartOrEndCheck(bool isOpen)
    {
        isStart = isOpen;
    }
    /// <summary>
    /// ��Ӽ�������
    /// </summary>
    /// <param name="key">��Ҫ������key</param>
    /// <param name="onClick"></param>
    public void AddKeyCode(KeyCode key, UnityAction onClick)
    {
        if (!keysOnClickDic.ContainsKey(key))
        {
            keysOnClickDic[key] = new HashSet<UnityAction>();
        }

        //�������ͬ�ĺ������򲻻�������
        keysOnClickDic[key].Add(onClick);
    }
    /// <summary>
    /// �Ƴ�key�ļ�������
    /// </summary>
    /// <param name="key">��Ҫȡ��������key</param>
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
