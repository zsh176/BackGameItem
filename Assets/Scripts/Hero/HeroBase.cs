using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;
using TMPro;
using Spine.Unity;
using UnityEngine.UI;
using System.Diagnostics;

/// <summary>
/// ��ɫ����
/// </summary>
public abstract class HeroBase : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IDragHandler
{
    /// <summary>
    /// ��ɫ��ǩ
    /// </summary>
    public abstract HeroType Type { get; }

    private Transform box_Pos;//��ɫռ�ø�����Ϣ
    private DragHero dragHero;//��קʱ���ݵĲ���
    private Image img_Box;//�ȼ���ͼ��
    private GameObject buff_Anim;//����buff״̬����
    private GameObject add_buff_Tips;//���ԳԵ�buff����ʾ
    private Animator thisAnimator;//����������
    private Quaternion downRotation;//����ʱ����ת
    private bool isUpLevelSeq;//�Ƿ�����ʾ������������

    protected RectTransform thisRt;//����
    protected SkeletonGraphic spineAnim;//��������
    protected TextMeshProUGUI tet_Level;//��ʾ�ȼ�
    protected HeroDataInfo heroDataInfo;//��ɫ��������
    protected bool isClick;//�Ƿ��� ��û�н�����ק
    
    [HideInInspector]
    public bool isSyn;//����Լ�Ϊ���ϳɶ���
    [HideInInspector]
    public int level; //�ȼ�
    [HideInInspector]
    public bool isAdLock;//�Ƿ������


    protected virtual void Start()
    {
        thisRt = GetComponent<RectTransform>();
        box_Pos = transform.Find("Box_Pos");
        spineAnim = transform.Find("spine_Anim").GetComponent<SkeletonGraphic>();
        tet_Level = transform.Find("level/tet_level").GetComponent<TextMeshProUGUI>();
        buff_Anim = transform.Find("buff_Anim").gameObject;
        add_buff_Tips = transform.Find("Add_buff_Tips").gameObject;


        dragHero = new DragHero();
        dragHero.type = this;
        dragHero.pos = box_Pos;

        heroDataInfo = FileSystemMgr.Instance.heroData.typeData[Type];
       

        tet_Level.text = level.ToString();
        buff_Anim.SetActive(false);
        add_buff_Tips.SetActive(false);

        if (Type != HeroType.Hero_100015)
        {
            thisAnimator = GetComponent<Animator>();
            thisAnimator.enabled = false;
            img_Box = transform.Find("img_Box").GetComponent<Image>();
            spineAnim.Skeleton.SetSkin($"xhz{level}");
            AddressablesMgr.Instance.LoadAssetAsync<Sprite>(spr =>
            {
                img_Box.sprite = spr;
            }, heroDataInfo.levelSpiteName + level.ToString(), StaticFields.Sprite);
        }

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
        if (GameStatus.Instance.offDrag) return;

        isClick = false;

        UpHeroOrBox upHero = new UpHeroOrBox();
        upHero.type = transform;
        upHero.e_touchState = E_TouchState.Down;
        EventMgr.Instance.EventTrigger<UpHeroOrBox>(E_EventType.placeHeroBox, upHero);

        //��������¼�ʱ�ụ�����
        DragHero newDragHero = new DragHero();
        newDragHero.type = this;
        newDragHero.isDown = true;
        newDragHero.pos = box_Pos;
        EventMgr.Instance.EventTrigger<DragHero>(E_EventType.dragHero, newDragHero);
    }
    /// <summary>
    /// �ɿ�
    /// </summary>
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (GameStatus.Instance.offDrag) return;
       

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
        if (GameStatus.Instance.offDrag) return;

        if (isClick) return;

        Btn_OnClick();

    }
    /// <summary>
    /// ��ק��
    /// </summary>
    public virtual void OnDrag(PointerEventData eventData)
    {
        if (GameStatus.Instance.offDrag) return;

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
            if (isUpLevelSeq)
            {   //�رն���
                thisAnimator.enabled = false;
                transform.DOLocalRotate(downRotation.eulerAngles, 0.2f);
            }
            isUpLevelSeq = false;

            float dist = Vector2.Distance(thisRt.position, _dragHero.type.transform.position);
            // 1�����ж��Ƿ���Ժϳ�
            if (dist < 4.8f && _dragHero.type != this && _dragHero.type.level == level && level <= 3 && !_dragHero.type.isSyn)
            {
                _dragHero.type.isSyn = true;
                _dragHero.type.transform.DOKill();
                _dragHero.type.transform.DOMove(transform.position, 0.25f).OnComplete(() =>
                {   //��������ص�
                    Destroy(_dragHero.type.gameObject);
                    UpLevel();
                });
            }
            else if (_dragHero.type == this)
            {   // 2���ٷ����¼����ж��Ƿ���Է����ھ�����
                StartCoroutine(InvokeIsThis());
            }
        }
        else if (_dragHero.isDown && _dragHero.type != this && _dragHero.type.Type == Type && _dragHero.type.level == level && level <= 3 && Type != HeroType.Hero_100015)
        {   //����������������Ŷ���
            isUpLevelSeq = true;
            downRotation = transform.localRotation;
            thisAnimator.enabled = true;
            // �ӵ�0֡��ʼ����
            thisAnimator.Play("isUpLevel", -1, 0f);
        }
    }
    //Э�����ڽ�� ����ק���Ⱥϳɷ���ִ���ж�
    IEnumerator InvokeIsThis()
    {
        yield return null;
        DragHero newDragHero = new DragHero();
        newDragHero.type = this;
        newDragHero.pos = box_Pos;
        newDragHero.isUpBox = true;
        newDragHero.isUpHero = false;
        EventMgr.Instance.EventTrigger<DragHero>(E_EventType.dragHero, newDragHero);
    }
    /// <summary>
    /// ����
    /// </summary>
    protected virtual void UpLevel()
    {
        level++;
        tet_Level.text = level.ToString();
        spineAnim.Skeleton.SetSkin($"xhz{level}");
        PlayAnimMgr.Instance.PlayAnim("HeroUPAnim", "hec", transform.position);
        AddressablesMgr.Instance.LoadAssetAsync<Sprite>(spr =>
        {
            img_Box.sprite = spr;
        }, heroDataInfo.levelSpiteName + level, StaticFields.Sprite);
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
            transform.localRotation = Quaternion.identity;
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
    /// <summary>
    /// ������
    /// </summary>
    Hero_10002, 
    /// <summary>
    /// ��С��
    /// </summary>
    Hero_10004, 
    /// <summary>
    /// ������
    /// </summary>
    Hero_10005, 
    /// <summary>
    /// ʳ�ð���
    /// </summary>
    Hero_10006, 
    /// <summary>
    /// �����ӳ�
    /// </summary>
    Hero_100014,
    /// <summary>
    /// �ڵ����
    /// </summary>
    Hero_100015,
    Box_1, Box_2, Box_3
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
    /// ����˲��
    /// </summary>
    public bool isDown;
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
    /// <summary>
    /// ��ʼ��ק
    /// </summary>
    Down,
    /// <summary>
    /// ���뱸�ÿ�����
    /// </summary>
    UpPlace,
    /// <summary>
    /// UpField
    /// </summary>
    UpField
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
    /// <summary>
}
