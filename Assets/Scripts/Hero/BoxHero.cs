using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ���Ƹ�����ק
/// </summary>
public class BoxHero : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IDragHandler
{

    private int level;//�����м�������
    private RectTransform thisRt;//����
    private Transform box_Pos;//ռ�ø�����Ϣ
    private GameObject ad_lock;//�������
    private bool isClick;//�Ƿ��� ��û�н�����ק
    private DragBox dragBox;

    [HideInInspector]
    public bool isAdLock;//�Ƿ�Ϊ������

    public HeroType type;//���ͱ�ǩ

    void Start()
    {
        thisRt = GetComponent<RectTransform>();
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
        
        thisRt.anchoredPosition += eventData.delta;
        EventMgr.Instance.EventTrigger<DragBox>(E_EventType.dragBox, dragBox);
    }

    /// <summary>
    /// ��������¼�
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
/// ��ק���Ӵ��ݵĲ���
/// </summary>
public class DragBox{
    /// <summary>
    /// ����λ��
    /// </summary>
    public Transform type;
    /// <summary>
    /// ռ�ø�����Ϣ
    /// </summary>
    public Transform pos;
    /// <summary>
    /// �Ƿ��ɿ�
    /// </summary>
    public bool isUp;
}