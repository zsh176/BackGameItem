using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.UI;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Assertions.Must;

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
    protected bool isClick;//�Ƿ��� ��û�н�����ק
    private Transform box_Pos;//��ɫռ�ø�����Ϣ

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
    /// ����
    /// </summary>
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        isClick = false;
    }
    /// <summary>
    /// �ɿ�
    /// </summary>
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        dragHero.isUp = true;
        EventMgr.Instance.EventTrigger<DragHero>(E_EventType.dragHero, dragHero);

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
        dragHero.isUp = false;
        EventMgr.Instance.EventTrigger<DragHero>(E_EventType.dragHero, dragHero);
    }


    /// <summary>
    /// �ɿ���ק��ɫ �ж��Ƿ�ϳ�����
    /// </summary>
    protected void TriggerDragHero(DragHero _dragHero)
    {
        if (_dragHero.isUp && _dragHero.type.Type == Type && _dragHero.type != this)
        {
            RectTransform triggerRt = _dragHero.type.GetComponent<RectTransform>();
            float dist = Vector2.Distance(thisRt.position, triggerRt.position);
            //�ɿ�ʱ����С��0.3 ��Ϊ�ϳ�
            if (dist < 0.3)
            {
                //����ק��ʧ��
                _dragHero.type.gameObject.SetActive(false);
                print(gameObject.name + "����");
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

    /// <summary>
    /// �����ھ�����
    /// </summary>
    protected void PlaceMatrix(DragHero _dragHero)
    {
        if (_dragHero.type == this)
        {
            transform.position += _dragHero.deviation;
        }
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
    /// �Ƿ��ɿ�
    /// </summary>
    public bool isUp;
    /// <summary>
    /// ������ɵ� λ��ƫ����Ϣ
    /// </summary>
    public Vector3 deviation;
}
