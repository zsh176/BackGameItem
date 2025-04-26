using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// UI�㼶
/// </summary>
public enum E_UI_Layer
{
    Bot,
    Mid,
    Top,
    System,
}

/// <summary>
/// UI������
/// 1.����������ʾ�����
/// 2.�ṩ���ⲿ ��ʾ�����صȵȽӿ�
/// </summary>
public class UIManager : InstanceMgr<UIManager>
{
    public Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();

    private Transform bot;//�ײ���ʾ
    private Transform mid;//�м���ʾ
    private Transform top;//�ϲ���ʾ
    private Transform system;//ϵͳ��ʾ���� ���ϲ���ʾ

    //��¼����UI��Canvas������ �����Ժ��ⲿ���ܻ�ʹ����
    public RectTransform canvas;
    //��һ�����Ҫͨ��������ص���ʾ����Ϊ���첽���أ����������쳣
    public UnityAction startinitPanel;

    public UIManager()
    {
        //����Canvas �����������ʱ�� �����Ƴ�
        AddressablesMgr.Instance.LoadAssetAsync<GameObject>(obj =>
        {
            GameObject m_obj = GameObject.Instantiate(obj);
            canvas = m_obj.transform as RectTransform;
            GameObject.DontDestroyOnLoad(m_obj);
            //���ð������
            m_obj.GetComponent<Canvas>().worldCamera = Camera.main;

            //�ҵ����㼶
            bot = canvas.Find("Bot");
            mid = canvas.Find("Mid");
            top = canvas.Find("Top");
            system = canvas.Find("System");
            startinitPanel?.Invoke();
        }, "MainCanvas", "UI");

        //����EventSystem �����������ʱ�� �����Ƴ�
        AddressablesMgr.Instance.LoadAssetAsync<GameObject>(obj =>
        {
            GameObject.DontDestroyOnLoad(GameObject.Instantiate(obj));
        }, "EventSystem", "UI");
    }

    /// <summary>
    /// ��ȡ�㼶
    /// </summary>
    /// <returns></returns>
    public Transform GetLayer(E_UI_Layer ui_Layer)
    {
        switch (ui_Layer)
        {
            case E_UI_Layer.Bot:
                return bot;
            case E_UI_Layer.Mid:
                return mid;
            case E_UI_Layer.Top:
                return top;
            case E_UI_Layer.System:
                return system;
        }
        return null;
    }

    /// <summary>
    /// ��ʾ���
    /// </summary>
    /// <typeparam name="T">���ű�����</typeparam>
    /// <param name="panelName">�����</param>
    /// <param name="layer">��ʾ����һ��</param>
    /// <param name="callBack">���Ԥ�����ɹ��ص� ����������Լ�����</param>
    public void ShowPanel<T>(E_UI_Layer layer = E_UI_Layer.Mid, UnityAction<T> callBack = null) where T : BasePanel
    {
        string panelName = typeof(T).Name;//��ȡ������� ��������������һ�� �ұ�ǩ������UI

        //�ж���û�и����
        if (panelDic.ContainsKey(panelName))
        {
            panelDic[panelName].ShowMe();
            // ������崴����ɺ���߼�
            if (callBack != null)
                callBack(panelDic[panelName] as T);
            //��������ظ����� ������ڸ���� ��ֱ����ʾ ���ûص�������  ֱ��return ���ٴ��������첽�����߼�
            return;
        }
        AddressablesMgr.Instance.LoadAssetAsync<GameObject>(obj =>
        {
            //Ҫ��ʾ����һ����ʾ
            Transform father = bot;
            switch (layer)
            {
                case E_UI_Layer.Mid:
                    father = mid;
                    break;
                case E_UI_Layer.Top:
                    father = top;
                    break;
                case E_UI_Layer.System:
                    father = system;
                    break;
            }

            GameObject m_obj = GameObject.Instantiate(obj);
            //���ø�����  �������λ�úʹ�С
            m_obj.transform.SetParent(father, false);

            m_obj.transform.localPosition = Vector3.zero;
            m_obj.transform.localScale = Vector3.one;

            (m_obj.transform as RectTransform).offsetMax = Vector2.zero;
            (m_obj.transform as RectTransform).offsetMin = Vector2.zero;

            //�õ�Ԥ�������ϵ����ű�
            T panel = m_obj.GetComponent<T>();
            panel.ShowMe();

            // ������崴����ɺ���߼�
            if (callBack != null)
                callBack(panel);

            //����������
            panelDic.Add(panelName, panel);

        }, panelName, "UI");
    }

    /// <summary>
    /// �������
    /// </summary>
    public void HidePanel<T>() where T : BasePanel
    {
        string panelName = typeof(T).Name;

        if (panelDic.ContainsKey(panelName))
        {
            panelDic[panelName].HideMe();
            GameObject.Destroy(panelDic[panelName].gameObject);
            panelDic.Remove(panelName);
        }
    }

    /// <summary>
    /// �õ�ĳһ���Ѿ���ʾ����� �����ⲿʹ��
    /// </summary>
    public T GetPanel<T>() where T : BasePanel
    {
        string name = typeof(T).Name;

        if (panelDic.ContainsKey(name))
            return panelDic[name] as T;

        return null;
    }

    /// <summary>
    /// ���ؼ�����Զ����¼�����
    /// </summary>
    /// <param name="control">�ؼ�����</param>
    /// <param name="type">�¼�����</param>
    /// <param name="callBack">�¼�����Ӧ����</param>
    public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> callBack)
    {
        EventTrigger trigger = control.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = control.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callBack);

        trigger.triggers.Add(entry);
    }
}