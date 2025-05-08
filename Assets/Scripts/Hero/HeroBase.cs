using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;
using TMPro;
using Spine.Unity;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

/// <summary>
/// 角色基类
/// </summary>
public abstract class HeroBase : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IDragHandler
{
    /// <summary>
    /// 角色标签
    /// </summary>
    public abstract HeroType Type { get; }

    private Transform box_Pos;//角色占用格子信息
    private DragHero dragHero;//拖拽时传递的参数
    private Image img_Box;//等级底图框
    private GameObject buff_Anim;//处于buff状态动画
    private GameObject add_buff_Tips;//可以吃到buff的提示
    private Animator thisAnimator;//动画控制器
    private Quaternion downRotation;//按下时的旋转
    private bool isUpLevelSeq;//是否开启提示可以升级动画
    private bool isField;//自己是否已上阵

    protected RectTransform thisRect;//自身
    protected Transform thisRotation;//左右旋转点
    protected SkeletonGraphic spineAnim;//骨骼动画
    protected TextMeshProUGUI tet_Level;//显示等级
    protected HeroDataInfo heroDataInfo;//角色配置数据
    protected bool isClick;//是否点击 且没有进行拖拽

    
    [HideInInspector]
    public bool isSyn;//标记自己为被合成对象
    [HideInInspector]
    public bool isAdLock;//是否广告解锁
    [HideInInspector]
    public int level; //等级
    [HideInInspector]
    public Transform bullet_Base;//子弹父物体
    [HideInInspector]
    public RectTransform sceneMapBG;//游戏场景地图背景

    protected Transform atkPos;//子弹发射位置
    protected List<Transform> enemyAllList;//存储场景中所有敌人
    protected int atkValue;//基础攻击力
    protected int atkValueBuff;//加成后的攻击力
    protected float atkCooling;//攻击间隔
    protected int atkDistance;//攻击距离
    protected float timeAtkCooling;//记时攻击间隔
    protected string bulletName;//子弹名称
    private int LevelMultiplier => Mathf.RoundToInt(Mathf.Pow(2, level - 1));//等级系数伤害加成

    protected virtual void Start()
    {
        thisRect = GetComponent<RectTransform>();
        box_Pos = transform.Find("Box_Pos");
        spineAnim = transform.Find("Rotation/spine_Anim").GetComponent<SkeletonGraphic>();
        tet_Level = transform.Find("level/tet_level").GetComponent<TextMeshProUGUI>();
        buff_Anim = transform.Find("buff_Anim").gameObject;
        add_buff_Tips = transform.Find("Add_buff_Tips").gameObject;
        thisRotation = transform.Find("Rotation");
        atkPos = transform.Find("Rotation/AtkPos");


        dragHero = new DragHero();
        dragHero.type = this;
        dragHero.pos = box_Pos;

        heroDataInfo = FileSystemMgr.Instance.heroData.typeData[Type];
       
        tet_Level.text = level.ToString();
        buff_Anim.SetActive(false);
        add_buff_Tips.SetActive(false);

        if (Type != HeroType.Hero_10015)
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

        atkValue = heroDataInfo.atkValue;
        atkCooling = heroDataInfo.atkCooling;
        atkDistance = heroDataInfo.atkDistance;
        bulletName = $"Bullet_{Type}";

        atkValueBuff = atkValue * LevelMultiplier;

        EventMgr.Instance.AddEventListener<DragHero>(E_EventType.placeMatrix, PlaceMatrix);
        EventMgr.Instance.AddEventListener<DragHero>(E_EventType.dragHero, UpDragHero);
        EventMgr.Instance.AddEventListener<List<Transform>>(E_EventType.chaEnemyList, ChaEnemyList);
    }

    private void OnDisable()
    {
        EventMgr.Instance.RemoveEventListener<DragHero>(E_EventType.placeMatrix, PlaceMatrix);
        EventMgr.Instance.RemoveEventListener<DragHero>(E_EventType.dragHero, UpDragHero);
        EventMgr.Instance.RemoveEventListener<List<Transform>>(E_EventType.chaEnemyList, ChaEnemyList);
    }

    #region 拖拽相关API
    /// <summary>
    /// 按下
    /// </summary>
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (GameStatus.Instance.offDrag) return;
        isClick = false;
        UpHeroOrBox upHero = new UpHeroOrBox();
        upHero.type = transform;
        upHero.heroBase = this;
        upHero.e_touchState = E_TouchState.Down;
        EventMgr.Instance.EventTrigger<UpHeroOrBox>(E_EventType.placeHeroBox, upHero);
        //解决处理事件时会互相干扰
        DragHero newDragHero = new DragHero();
        newDragHero.type = this;
        newDragHero.isDown = true;
        newDragHero.pos = box_Pos;
        EventMgr.Instance.EventTrigger<DragHero>(E_EventType.dragHero, newDragHero);
    }
    /// <summary>
    /// 松开
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
    /// 按下并抬起
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (GameStatus.Instance.offDrag) return;
        if (isClick) return;
        Btn_OnClick();
    }
    /// <summary>
    /// 拖拽中
    /// </summary>
    public virtual void OnDrag(PointerEventData eventData)
    {
        if (GameStatus.Instance.offDrag) return;
        isClick = true;
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
        EventMgr.Instance.EventTrigger<DragHero>(E_EventType.dragHero, dragHero);
    }
    /// <summary>
    /// 松开拖拽角色 判断是否合成升级
    /// </summary>
    protected void UpDragHero(DragHero _dragHero)
    {
        if (_dragHero.isUpHero && _dragHero.type.Type == Type)
        {
            if (isUpLevelSeq)
            {   //关闭动画
                thisAnimator.enabled = false;
                transform.DOKill();
                if (isField)
                    transform.DOLocalRotate(Vector3.zero, 0.2f);
                else
                    transform.DOLocalRotate(downRotation.eulerAngles, 0.2f);
            }
            isUpLevelSeq = false;

            float dist = Vector2.Distance(thisRect.position, _dragHero.type.transform.position);
            // 1、先判断是否可以合成
            if (dist < 4.5f && _dragHero.type != this && _dragHero.type.level == level && level <= 3 && !_dragHero.type.isSyn)
            {
                _dragHero.type.isSyn = true;
                _dragHero.type.transform.DOKill();
                _dragHero.type.transform.DOMove(transform.position, 0.25f).OnComplete(() =>
                {   //动画播完回调
                    Destroy(_dragHero.type.gameObject);
                    UpLevel();
                });
            }
            else if (_dragHero.type == this)
            {   // 2、再发送事件，判断是否可以放置在矩阵上
                StartCoroutine(InvokeIsThis());
            }
        }
        else if (_dragHero.isDown && _dragHero.type != this && _dragHero.type.Type == Type && _dragHero.type.level == level && level <= 3 && Type != HeroType.Hero_10015)
        {   //自身符合升级，播放动画
            isUpLevelSeq = true;
            downRotation = transform.localRotation;
            thisAnimator.enabled = true;
            // 从第0帧开始播放
            thisAnimator.Play("isUpLevel", -1, 0f);
        }
    }
    //协程用于解决 被拖拽方比合成方先执行判断
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
    /// 升级
    /// </summary>
    protected virtual void UpLevel()
    {
        level++;
        tet_Level.text = level.ToString();
        atkValueBuff = atkValue * LevelMultiplier;
        spineAnim.Skeleton.SetSkin($"xhz{level}");
        PlayAnimMgr.Instance.PlayUpLevelAnim("HeroUPAnim", "hec", transform.position);
        AddressablesMgr.Instance.LoadAssetAsync<Sprite>(spr =>
        {
            img_Box.sprite = spr;
        }, heroDataInfo.levelSpiteName + level, StaticFields.Sprite);
    }
    /// <summary>
    /// 成功放置在矩阵上，角色上阵
    /// </summary>
    protected void PlaceMatrix(DragHero _dragHero)
    {
        if (_dragHero.type == this)
        {
            isField = true;
            UpHeroOrBox upHero = new UpHeroOrBox();
            upHero.type = transform;
            upHero.heroBase = this;
            upHero.e_touchState = E_TouchState.UpField;
            EventMgr.Instance.EventTrigger<UpHeroOrBox>(E_EventType.placeHeroBox, upHero);
            Vector3 movePos = transform.position + _dragHero.deviation;
            transform.localRotation = Quaternion.identity;
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
    #endregion

    /// <summary>
    /// 更新敌人数组
    /// </summary>
    private void ChaEnemyList(List<Transform> allEnemy)
    {
        enemyAllList = allEnemy;
    }

    //临时判断变量
    private Transform nearestEnemy;
    private float dist;
    private float minDistance;
    /// <summary>
    /// 是否可以攻击
    /// </summary>
    protected virtual void IsOkAtk()
    {
        if (enemyAllList != null && enemyAllList.Count > 0)
        {
            minDistance = float.MaxValue;
            // 遍历所有敌人，查找距离最近的那个
            foreach (Transform enemy in enemyAllList)
            {
                dist = Vector2.Distance(enemy.localPosition, transform.localPosition);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearestEnemy = enemy;
                }
            }
            if (minDistance < atkDistance)
            {
                Attack(nearestEnemy);
            }
        }
    }
    /// <summary>
    /// 发起攻击
    /// </summary>
    protected virtual void Attack(Transform target)
    {
        if (timeAtkCooling > 0) return;
        if (target.position.x < transform.position.x)
            thisRotation.localScale = new Vector2(-1, 1);
        else
            thisRotation.localScale = Vector2.one;

        // 先播放攻击动画，结束后自动播放待机动画
        spineAnim.AnimationState.SetAnimation(0, HeroAnimSpineTag.atk, false);
        spineAnim.AnimationState.AddAnimation(0, HeroAnimSpineTag.stand, true, 0f);

        timeAtkCooling = atkCooling;
    }

    protected virtual void Update()
    {
        if (!GameStatus.Instance.offDrag) return;

        if (enemyAllList == null) return;

        if (timeAtkCooling > 0)
            timeAtkCooling -= Time.deltaTime;
        else
            IsOkAtk();
    }

}


#region 枚举，数据类相关

/// <summary>
/// 动画名
/// </summary>
public class HeroAnimSpineTag
{
    /// <summary>
    /// 待机
    /// </summary>
    public const string stand = "stand";
    /// <summary>
    /// 攻击
    /// </summary>
    public const string atk = "hit";
    /// <summary>
    /// 技能
    /// </summary>
    public const string skill = "skill";
}

//英雄类型 枚举
public enum HeroType
{
    /// <summary>
    /// 王东东
    /// </summary>
    Hero_10002, 
    /// <summary>
    /// 王小虎
    /// </summary>
    Hero_10004, 
    /// <summary>
    /// 养鹅叔
    /// </summary>
    Hero_10005, 
    /// <summary>
    /// 食堂阿姨
    /// </summary>
    Hero_10006, 
    /// <summary>
    /// 保安队长
    /// </summary>
    Hero_10014,
    /// <summary>
    /// 黑道大哥
    /// </summary>
    Hero_10015,
    Box_1, Box_2, Box_3
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
    /// 按下瞬间
    /// </summary>
    public bool isDown;
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
    /// <summary>
    /// 开始拖拽
    /// </summary>
    Down,
    /// <summary>
    /// 放入备用卡池区
    /// </summary>
    UpPlace,
    /// <summary>
    /// 角色上阵
    /// </summary>
    UpField
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
    
    public HeroBase heroBase;
}
#endregion
