using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.UI;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Assertions.Must;

/// <summary>
/// 角色基类
/// </summary>
public abstract class HeroBase : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IDragHandler
{
    /// <summary>
    /// 角色标签
    /// </summary>
    public abstract HeroType Type { get; }

    protected RectTransform thisRt;//自身
    protected bool isClick;//是否点击 且没有进行拖拽
    private Transform box_Pos;//角色占用格子信息

    int level;
    protected int Level
    {
        get
        {
            return level;
        }
        set
        {
            level = value;
        }
    }

    protected virtual void Awake()
    {
        thisRt = GetComponent<RectTransform>();
        box_Pos = transform.Find("Box_Pos");
        dragHero = new DragHero();
        dragHero.type = this;
        dragHero.pos = box_Pos;
        EventMgr.Instance.AddEventListener<DragHero>(E_EventType.placeMatrix, PlaceMatrix);
        EventMgr.Instance.AddEventListener<DragHero>(E_EventType.dragHero, TriggerDragHero);
    }

    DragHero dragHero;
    /// <summary>
    /// 按下
    /// </summary>
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        isClick = false;
    }
    /// <summary>
    /// 松开
    /// </summary>
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        dragHero.isUp = true;
        EventMgr.Instance.EventTrigger<DragHero>(E_EventType.dragHero, dragHero);

    }
    /// <summary>
    /// 按下并抬起
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (isClick) return;

        Btn_OnClick();

    }
    /// <summary>
    /// 拖拽中
    /// </summary>
    public virtual void OnDrag(PointerEventData eventData)
    {
        isClick = true;
        //将鼠标的增量移动添加到自身
        thisRt.anchoredPosition += eventData.delta;
        dragHero.isUp = false;
        EventMgr.Instance.EventTrigger<DragHero>(E_EventType.dragHero, dragHero);
    }


    /// <summary>
    /// 松开拖拽角色 判断是否合成升级
    /// </summary>
    protected void TriggerDragHero(DragHero _dragHero)
    {
        if (_dragHero.isUp && _dragHero.type.Type == Type && _dragHero.type != this)
        {
            RectTransform triggerRt = _dragHero.type.GetComponent<RectTransform>();
            float dist = Vector2.Distance(thisRt.position, triggerRt.position);
            //松开时距离小于0.3 视为合成
            if (dist < 0.3)
            {
                //将拖拽方失活
                _dragHero.type.gameObject.SetActive(false);
                print(gameObject.name + "升级");
            }
        }
    }

    /// <summary>
    /// 点击触发事件
    /// </summary>
    protected virtual void Btn_OnClick()
    {
        print("点击触发事件");
    }

    /// <summary>
    /// 放置在矩阵上
    /// </summary>
    protected void PlaceMatrix(DragHero _dragHero)
    {
        if (_dragHero.type == this)
        {
            transform.position += _dragHero.deviation;
        }
    }

}




//英雄类型 枚举
public enum HeroType
{
    Hero_10002, Hero_10004, Hero_10005, Hero_10006, Hero_100014, Hero_100015
}

/// <summary>
/// 用于传递拖拽中角色的信息 因为事件监听只能传一个参数
/// </summary>
public class DragHero
{
    /// <summary>
    /// 获取或拖拽方法的标签和判断是否是自己本身
    /// </summary>
    public HeroBase type;
    /// <summary>
    /// 占用格子信息
    /// </summary>
    public Transform pos;
    /// <summary>
    /// 是否松开
    /// </summary>
    public bool isUp;
    /// <summary>
    /// 放置完成的 位置偏移信息
    /// </summary>
    public Vector3 deviation;
}
