using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// UI层级
/// </summary>
public enum E_UI_Layer
{
    Bot,
    Mid,
    Top,
    System,
}

/// <summary>
/// UI管理器
/// 1.管理所有显示的面板
/// 2.提供给外部 显示和隐藏等等接口
/// </summary>
public class UIManager : InstanceMgr<UIManager>
{
    public Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();

    private Transform bot;//底层显示
    private Transform mid;//中间显示
    private Transform top;//上层显示
    private Transform system;//系统提示弹窗 最上层显示

    //记录我们UI的Canvas父对象 方便以后外部可能会使用它
    public RectTransform canvas;
    //第一个面板要通过绑定这个回调显示，因为是异步加载，否则会出现异常
    public UnityAction startinitPanel;

    public UIManager()
    {
        //创建Canvas 让其过场景的时候 不被移除
        AddressablesMgr.Instance.LoadAssetAsync<GameObject>(obj =>
        {
            GameObject m_obj = GameObject.Instantiate(obj);
            canvas = m_obj.transform as RectTransform;
            GameObject.DontDestroyOnLoad(m_obj);
            //设置绑定摄像机
            m_obj.GetComponent<Canvas>().worldCamera = Camera.main;

            //找到各层级
            bot = canvas.Find("Bot");
            mid = canvas.Find("Mid");
            top = canvas.Find("Top");
            system = canvas.Find("System");
            startinitPanel?.Invoke();
        }, "MainCanvas", "UI");

        //创建EventSystem 让其过场景的时候 不被移除
        AddressablesMgr.Instance.LoadAssetAsync<GameObject>(obj =>
        {
            GameObject.DontDestroyOnLoad(GameObject.Instantiate(obj));
        }, "EventSystem", "UI");
    }

    /// <summary>
    /// 获取层级
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
    /// 显示面板
    /// </summary>
    /// <typeparam name="T">面板脚本类型</typeparam>
    /// <param name="panelName">面板名</param>
    /// <param name="layer">显示在哪一层</param>
    /// <param name="callBack">面板预创建成功回调 参数是面板自己的类</param>
    public void ShowPanel<T>(E_UI_Layer layer = E_UI_Layer.Mid, UnityAction<T> callBack = null) where T : BasePanel
    {
        string panelName = typeof(T).Name;//获取面板名字 面板名必须和类名一致 且标签必须是UI

        //判断有没有该面板
        if (panelDic.ContainsKey(panelName))
        {
            panelDic[panelName].ShowMe();
            // 处理面板创建完成后的逻辑
            if (callBack != null)
                callBack(panelDic[panelName] as T);
            //避免面板重复加载 如果存在该面板 即直接显示 调用回调函数后  直接return 不再处理后面的异步加载逻辑
            return;
        }
        AddressablesMgr.Instance.LoadAssetAsync<GameObject>(obj =>
        {
            //要显示在哪一层显示
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
            //设置父对象  设置相对位置和大小
            m_obj.transform.SetParent(father, false);

            m_obj.transform.localPosition = Vector3.zero;
            m_obj.transform.localScale = Vector3.one;

            (m_obj.transform as RectTransform).offsetMax = Vector2.zero;
            (m_obj.transform as RectTransform).offsetMin = Vector2.zero;

            //得到预设体身上的面板脚本
            T panel = m_obj.GetComponent<T>();
            panel.ShowMe();

            // 处理面板创建完成后的逻辑
            if (callBack != null)
                callBack(panel);

            //把面板存起来
            panelDic.Add(panelName, panel);

        }, panelName, "UI");
    }

    /// <summary>
    /// 隐藏面板
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
    /// 得到某一个已经显示的面板 方便外部使用
    /// </summary>
    public T GetPanel<T>() where T : BasePanel
    {
        string name = typeof(T).Name;

        if (panelDic.ContainsKey(name))
            return panelDic[name] as T;

        return null;
    }

    /// <summary>
    /// 给控件添加自定义事件监听
    /// </summary>
    /// <param name="control">控件对象</param>
    /// <param name="type">事件类型</param>
    /// <param name="callBack">事件的响应函数</param>
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