using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 控制格子拖拽
/// </summary>
public class BoxHero : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IDragHandler
{
    public int level;//自身有几个格子

    private RectTransform thisRt;//自身
    private Transform box_Pos;//占用格子信息
    private bool isClick;//是否点击 且没有进行拖拽
    private DragBox dragBox;


    void Start()
    {
        thisRt = GetComponent<RectTransform>();
        box_Pos = transform.Find("Box_Pos");

        dragBox = new DragBox();
        dragBox.pos = box_Pos;
        dragBox.isUp = false;
        dragBox.type = transform;

        EventMgr.Instance.AddEventListener<DragBox>(E_EventType.upBox, BackUpBox);
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        isClick = false;

        UpHeroOrBox upHero = new UpHeroOrBox();
        upHero.type = transform;
        upHero.e_touchState = E_TouchState.Down;
        EventMgr.Instance.EventTrigger<UpHeroOrBox>(E_EventType.placeHeroBox, upHero);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        DragBox newDragBox = new DragBox();
        newDragBox.isUp = true;
        newDragBox.pos = box_Pos;
        newDragBox.type = transform;
        EventMgr.Instance.EventTrigger<DragBox>(E_EventType.dragBox, newDragBox);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isClick) return;
        Btn_OnClick();
    }
    public void OnDrag(PointerEventData eventData)
    {
        isClick = true;
        thisRt.anchoredPosition += eventData.delta;

        EventMgr.Instance.EventTrigger<DragBox>(E_EventType.dragBox, dragBox);
    }

    //返回拖拽结束的结果
    private void BackUpBox(DragBox dragBox)
    {
        if(dragBox.type == transform)
        {
            if (dragBox.isWin)
            {
                //成功放置
                Vector3 movePos = transform.position + dragBox.deviation;
                transform.DOKill();
                transform.DOMove(movePos, 0.25f).SetEase(Ease.OutBounce);
            }
            else
            {
                UpHeroOrBox upHero = new UpHeroOrBox();
                upHero.type = transform;
                upHero.e_touchState = E_TouchState.UpPlace;
                EventMgr.Instance.EventTrigger<UpHeroOrBox>(E_EventType.placeHeroBox, upHero);
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
}

/// <summary>
/// 拖拽格子传递的参数
/// </summary>
public class DragBox{
    /// <summary>
    /// 自身位置
    /// </summary>
    public Transform type;
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
    /// <summary>
    /// 是否成功放置
    /// </summary>
    public bool isWin;
}
