using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;
using TMPro;

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
    private Transform box_Pos;//角色占用格子信息
    protected TextMeshProUGUI tet_Level;//显示等级
    private DragHero dragHero;//拖拽时传递的参数
    public bool isSyn;//标记自己为被合成对象


    protected bool isClick;//是否点击 且没有进行拖拽


    private int level;
    protected int Level
    {
        get
        {
            return level;
        }
        set
        {
            level = value;
            tet_Level.text = level.ToString();
            if (level != 1)//升级特效
            {
                PlayAnimMgr.Instance.PlayAnim("HeroUPAnim", "hec", transform.position);
            } 
        }
    }

    protected virtual void Awake()
    {
        thisRt = GetComponent<RectTransform>();
        box_Pos = transform.Find("Box_Pos");
        tet_Level = transform.Find("level/tet_level").GetComponent<TextMeshProUGUI>();

        dragHero = new DragHero();
        dragHero.type = this;
        dragHero.pos = box_Pos;

        Level = 1;
        
        EventMgr.Instance.AddEventListener<DragHero>(E_EventType.placeMatrix, PlaceMatrix);
        EventMgr.Instance.AddEventListener<DragHero>(E_EventType.dragHero, UpDragHero);
    }
    private void OnDisable()
    {
        EventMgr.Instance.RemoveEventListener<DragHero>(E_EventType.placeMatrix, PlaceMatrix);
        EventMgr.Instance.RemoveEventListener<DragHero>(E_EventType.dragHero, UpDragHero);
    }

    /// <summary>
    /// 按下
    /// </summary>
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        isClick = false;

        UpHeroOrBox upHero = new UpHeroOrBox();
        upHero.type = transform;
        upHero.e_touchState = E_TouchState.Down;
        EventMgr.Instance.EventTrigger<UpHeroOrBox>(E_EventType.placeHeroBox, upHero);

    }
    /// <summary>
    /// 松开
    /// </summary>
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        //解决处理事件时会互相干扰
        DragHero newDragHero = new DragHero();
        newDragHero.type = this;
        newDragHero.pos = box_Pos;
        newDragHero.isUpHero = true;
        EventMgr.Instance.EventTrigger<DragHero>(E_EventType.dragHero, newDragHero);
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

        EventMgr.Instance.EventTrigger<DragHero>(E_EventType.dragHero, dragHero);
    }


    /// <summary>
    /// 松开拖拽角色 判断是否合成升级
    /// </summary>
    protected void UpDragHero(DragHero _dragHero)
    {
        if (_dragHero.isUpHero && _dragHero.type.Type == Type)
        {
            RectTransform triggerRt = _dragHero.type.GetComponent<RectTransform>();
            float dist = Vector2.Distance(thisRt.position, triggerRt.position);
            
            // 1、先判断是否可以合成
            if (dist < 6.5f && _dragHero.type != this && _dragHero.type.level == level && !_dragHero.type.isSyn)
            {
                _dragHero.type.isSyn = true;
                _dragHero.type.transform.DOKill();
                _dragHero.type.transform.DOMove(transform.position, 0.25f).OnComplete(() =>
                {
                    //动画播完回调
                    _dragHero.type.gameObject.SetActive(false);
                    Level++;
                });
            }
            else if (_dragHero.type == this)
            {
                DragHero newDragHero = new DragHero();
                newDragHero.type = this;
                newDragHero.pos = box_Pos;
                newDragHero.isUpBox = true;
                newDragHero.isUpHero = false;
                newDragHero.UpHeroOnCilck = () =>
                {
                    //解决被合成方 先执行 放入备战区，等判断完是否可以合成，再执行放回备战区
                    StartCoroutine(InvokeDragHero());
                };
                // 2、再发送事件，判断是否可以放置在矩阵上
                EventMgr.Instance.EventTrigger<DragHero>(E_EventType.dragHero, newDragHero);
            }
        }
    }
    IEnumerator InvokeDragHero()
    {
        yield return null;
        if (!isSyn)//加锁，防止两个动画叠加
        {
            // 3、如果都没有，就将自己放回备战区
            UpHeroOrBox upHero = new UpHeroOrBox();
            upHero.type = transform;
            upHero.e_touchState = E_TouchState.UpPlace;
            EventMgr.Instance.EventTrigger<UpHeroOrBox>(E_EventType.placeHeroBox, upHero);
        }
    }
    /// <summary>
    /// 成功放置在矩阵上，角色上阵
    /// </summary>
    protected void PlaceMatrix(DragHero _dragHero)
    {
        if (_dragHero.type == this)
        {
            UpHeroOrBox upHero = new UpHeroOrBox();
            upHero.type = transform;
            upHero.e_touchState = E_TouchState.UpField;
            EventMgr.Instance.EventTrigger<UpHeroOrBox>(E_EventType.placeHeroBox, upHero);
            Vector3 movePos = transform.position + _dragHero.deviation;
            transform.DOKill();
            transform.DOMove(movePos, 0.25f).SetEase(Ease.OutBounce);
        }
    }

    /// <summary>
    /// 点击触发事件
    /// </summary>
    protected virtual void Btn_OnClick()
    {
        print("点击触发事件");
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
    /// 是否松开 用于判断是否合成
    /// </summary>
    public bool isUpHero;
    /// <summary>
    /// 是否松开 用于判断是否放置矩阵
    /// </summary>
    public bool isUpBox;
    /// <summary>
    /// 放置完成的 位置偏移信息
    /// </summary>
    public Vector3 deviation;
    /// <summary>
    /// 松开拖拽回调
    /// </summary>
    public Action UpHeroOnCilck;
}

/// <summary>
/// 触摸状态
/// </summary>
public enum E_TouchState{
    Down, UpPlace, UpField
}


/// <summary>
/// 用于传递按下和松开角色或格子时的参数
/// </summary>
public class UpHeroOrBox
{
    public Transform type;
    /// <summary>
    /// 是否上阵
    /// </summary>
    public E_TouchState e_touchState;
}
