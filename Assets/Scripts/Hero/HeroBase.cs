using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;
using TMPro;

/// <summary>
/// ��ɫ����
/// </summary>
public abstract class HeroBase : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IDragHandler
{
    /// <summary>
    /// ��ɫ��ǩ
    /// </summary>
    public abstract HeroType Type { get; }

    protected RectTransform thisRt;//����
    private Transform box_Pos;//��ɫռ�ø�����Ϣ
    protected TextMeshProUGUI tet_Level;//��ʾ�ȼ�
    private DragHero dragHero;//��קʱ���ݵĲ���
    public bool isSyn;//����Լ�Ϊ���ϳɶ���


    protected bool isClick;//�Ƿ��� ��û�н�����ק


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
            if (level != 1)//������Ч
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
    /// ����
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
    /// �ɿ�
    /// </summary>
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        //��������¼�ʱ�ụ�����
        DragHero newDragHero = new DragHero();
        newDragHero.type = this;
        newDragHero.pos = box_Pos;
        newDragHero.isUpHero = true;
        EventMgr.Instance.EventTrigger<DragHero>(E_EventType.dragHero, newDragHero);
    }
    /// <summary>
    /// ���²�̧��
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (isClick) return;

        Btn_OnClick();

    }
    /// <summary>
    /// ��ק��
    /// </summary>
    public virtual void OnDrag(PointerEventData eventData)
    {
        isClick = true;
        //�����������ƶ���ӵ�����
        thisRt.anchoredPosition += eventData.delta;

        EventMgr.Instance.EventTrigger<DragHero>(E_EventType.dragHero, dragHero);
    }


    /// <summary>
    /// �ɿ���ק��ɫ �ж��Ƿ�ϳ�����
    /// </summary>
    protected void UpDragHero(DragHero _dragHero)
    {
        if (_dragHero.isUpHero && _dragHero.type.Type == Type)
        {
            RectTransform triggerRt = _dragHero.type.GetComponent<RectTransform>();
            float dist = Vector2.Distance(thisRt.position, triggerRt.position);
            
            // 1�����ж��Ƿ���Ժϳ�
            if (dist < 6.5f && _dragHero.type != this && _dragHero.type.level == level && !_dragHero.type.isSyn)
            {
                _dragHero.type.isSyn = true;
                _dragHero.type.transform.DOKill();
                _dragHero.type.transform.DOMove(transform.position, 0.25f).OnComplete(() =>
                {
                    //��������ص�
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
                    //������ϳɷ� ��ִ�� ���뱸ս�������ж����Ƿ���Ժϳɣ���ִ�зŻر�ս��
                    StartCoroutine(InvokeDragHero());
                };
                // 2���ٷ����¼����ж��Ƿ���Է����ھ�����
                EventMgr.Instance.EventTrigger<DragHero>(E_EventType.dragHero, newDragHero);
            }
        }
    }
    IEnumerator InvokeDragHero()
    {
        yield return null;
        if (!isSyn)//��������ֹ������������
        {
            // 3�������û�У��ͽ��Լ��Żر�ս��
            UpHeroOrBox upHero = new UpHeroOrBox();
            upHero.type = transform;
            upHero.e_touchState = E_TouchState.UpPlace;
            EventMgr.Instance.EventTrigger<UpHeroOrBox>(E_EventType.placeHeroBox, upHero);
        }
    }
    /// <summary>
    /// �ɹ������ھ����ϣ���ɫ����
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
    /// ��������¼�
    /// </summary>
    protected virtual void Btn_OnClick()
    {
        print("��������¼�");
    }

   

}


//Ӣ������ ö��
public enum HeroType
{
    Hero_10002, Hero_10004, Hero_10005, Hero_10006, Hero_100014, Hero_100015
}

/// <summary>
/// ���ڴ�����ק�н�ɫ����Ϣ ��Ϊ�¼�����ֻ�ܴ�һ������
/// </summary>
public class DragHero
{
    /// <summary>
    /// ��ȡ����ק�����ı�ǩ���ж��Ƿ����Լ�����
    /// </summary>
    public HeroBase type;
    /// <summary>
    /// ռ�ø�����Ϣ
    /// </summary>
    public Transform pos;
    /// <summary>
    /// �Ƿ��ɿ� �����ж��Ƿ�ϳ�
    /// </summary>
    public bool isUpHero;
    /// <summary>
    /// �Ƿ��ɿ� �����ж��Ƿ���þ���
    /// </summary>
    public bool isUpBox;
    /// <summary>
    /// ������ɵ� λ��ƫ����Ϣ
    /// </summary>
    public Vector3 deviation;
    /// <summary>
    /// �ɿ���ק�ص�
    /// </summary>
    public Action UpHeroOnCilck;
}

/// <summary>
/// ����״̬
/// </summary>
public enum E_TouchState{
    Down, UpPlace, UpField
}


/// <summary>
/// ���ڴ��ݰ��º��ɿ���ɫ�����ʱ�Ĳ���
/// </summary>
public class UpHeroOrBox
{
    public Transform type;
    /// <summary>
    /// �Ƿ�����
    /// </summary>
    public E_TouchState e_touchState;
}
