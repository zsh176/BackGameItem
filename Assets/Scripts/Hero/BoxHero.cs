using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ���Ƹ�����ק
/// </summary>
public class BoxHero : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IDragHandler
{
    public int level;//�����м�������

    private RectTransform thisRt;//����
    private Transform box_Pos;//ռ�ø�����Ϣ
    private bool isClick;//�Ƿ��� ��û�н�����ק
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

    //������ק�����Ľ��
    private void BackUpBox(DragBox dragBox)
    {
        if(dragBox.type == transform)
        {
            if (dragBox.isWin)
            {
                //�ɹ�����
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
    /// ��������¼�
    /// </summary>
    protected virtual void Btn_OnClick()
    {
        print("��������¼�");
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
    /// <summary>
    /// ������ɵ� λ��ƫ����Ϣ
    /// </summary>
    public Vector3 deviation;
    /// <summary>
    /// �Ƿ�ɹ�����
    /// </summary>
    public bool isWin;
}
