using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 控制格子拖拽
/// </summary>
public class BoxHero : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IDragHandler
{

    private RectTransform thisRect;//自身
    private Transform box_Pos;//占用格子信息
    private GameObject ad_lock;//广告遮罩
    private bool isClick;//是否点击 且没有进行拖拽
    private DragBox dragBox;

    [HideInInspector]
    public bool isAdLock;//是否为广告解锁

    public HeroType type;//类型标签

    void Start()
    {
        thisRect = GetComponent<RectTransform>();
        box_Pos = transform.Find("Box_Pos");
        ad_lock = transform.Find("Ad_lock").gameObject;


        dragBox = new DragBox();
        dragBox.pos = box_Pos;
        dragBox.isUp = false;
        dragBox.type = transform;

        ad_lock.SetActive(isAdLock);
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameStatus.Instance.offDrag) return;

        isClick = false;

        if (isAdLock) return;

        UpHeroOrBox upHero = new UpHeroOrBox();
        upHero.type = transform;
        upHero.e_touchState = E_TouchState.Down;
        EventMgr.Instance.EventTrigger<UpHeroOrBox>(E_EventType.placeHeroBox, upHero);
        EventMgr.Instance.EventTrigger(E_EventType.startDragBox);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (GameStatus.Instance.offDrag) return;

        if (isAdLock) return;

        DragBox newDragBox = new DragBox();
        newDragBox.isUp = true;
        newDragBox.pos = box_Pos;
        newDragBox.type = transform;
        EventMgr.Instance.EventTrigger<DragBox>(E_EventType.dragBox, newDragBox);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameStatus.Instance.offDrag) return;

        if (isClick) return;
        Btn_OnClick();
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (GameStatus.Instance.offDrag) return;

        isClick = true;

        if (isAdLock) return;
        // 将鼠标位置转换为父级 RectTransform 的本地坐标
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            thisRect.parent as RectTransform, // 父级 RectTransform
            eventData.position,                // 鼠标屏幕坐标
            eventData.pressEventCamera,        // 摄像机（Canvas 的渲染模式决定）
            out Vector2 localPos
        ))
        {
            // 更新 UI 元素的位置
            thisRect.localPosition = localPos;
        }
        EventMgr.Instance.EventTrigger<DragBox>(E_EventType.dragBox, dragBox);
    }

    /// <summary>
    /// 点击触发事件
    /// </summary>
    protected virtual void Btn_OnClick()
    {
        if (!isAdLock) return;
        AdsMgr.Instance.ShowRewardedVideoAd(() =>
        {
            isAdLock = false;
            ad_lock.GetComponent<Image>().DOFade(0, 0.3f).OnComplete(() =>
            {
                ad_lock.SetActive(false);
            });
            ad_lock.transform.GetChild(0).GetComponent<Image>().DOFade(0, 0.3f);
        });
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
}