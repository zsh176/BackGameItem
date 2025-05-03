using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : BasePanel
{
    private List<Transform> heroBoxs;//所有备用区角色信息
    private List<Transform> fieldHeros;//所有上阵中角色信息
    private Transform heroBoxBase;//角色备用区
    private Transform fieldHero;//角色上阵中
    private Transform addBoxPos;//生成格子的初始位置
    private float animSmoothTime = 0.3f;//动画过度时间
    private GameController gameController;//游戏流程控制器


    private Transform battleMap;//游戏场景
    private RectTransform phase;//准备阶段，卡池备用区
    private RectTransform btn_Time;//两倍速按钮
    private RectTransform fOV;//地图缩放按钮

    #region 配置英雄资源名
    Dictionary<HeroType, string[]> heroNameDict = new Dictionary<HeroType, string[]>()
    {
        { HeroType.Hero_10002, new [] { "Hero_10002" } },
        { HeroType.Hero_10004, new [] { "Hero_10004" }  },
        { HeroType.Hero_10005, new [] { "Hero_10005" }  },
        { HeroType.Hero_10006, new [] { "Hero_10006" }  },
        { HeroType.Hero_100014, new [] { "Hero_10014" } },
        { HeroType.Hero_100015, new [] { "Hero_10015" }  },
        { HeroType.Box_1, new [] { "Box_1one" }  },
        { HeroType.Box_2, new [] { "Box_2two_Horizontal", "Box_2two_Verical" }  },
        { HeroType.Box_3, new [] { "Box_3three_Horizontal", "Box_3three_Verical", "Box_3three_L" }  }
    };
    #endregion

    //初始化
    public override void ShowMe()
    {
        heroBoxs = new List<Transform>();
        fieldHeros = new List<Transform>();
        heroBoxBase = transform.Find("Phase/HeroBox");
        fieldHero = transform.Find("BattleMap/Matrix_MapBox/FieldHero");
        addBoxPos = transform.Find("Phase/AddBoxPos");
        gameController = GetComponent<GameController>();

        battleMap = transform.Find("BattleMap");
        phase = transform.Find("Phase").GetComponent<RectTransform>();
        btn_Time = transform.Find("Gamethe/btn_Time").GetComponent<RectTransform>();
        fOV = transform.Find("Gamethe/FOV").GetComponent<RectTransform>();

        RefreshHeroBox();

        EventMgr.Instance.AddEventListener<UpHeroOrBox>(E_EventType.placeHeroBox, PlaceHeroBox);
    }

    public override void HideMe()
    {
        EventMgr.Instance.RemoveEventListener<UpHeroOrBox>(E_EventType.placeHeroBox, PlaceHeroBox);
    }


    protected override void OnClick(string btnName)
    {
        switch (btnName)
        {
            case "btn_Play"://开战，开始游戏
                OnClick_Play();
                break;
            case "btn_Back"://退出游戏
                OnClick_Back();
                break;
            case "btn_Refresh"://普通刷新
                OnClick_Refresh();
                break;
            case "btn_AdBox"://增加格子
                OnClick_AdBox();
                break;
            case "btn_Adadv"://高级刷新，必出高级角色
                OnClick_Adadv();
                break;
            
        }
    }

    #region 按钮事件
    private void OnClick_Play()
    {
        battleMap.DOScale(new Vector3(0.45f, 0.45f), animSmoothTime);
        //基于锚点移动要用这个API
        phase.DOAnchorPosY(-410, animSmoothTime);
        btn_Time.DOAnchorPosX(47, animSmoothTime);
        fOV.DOAnchorPosX(35, animSmoothTime);
    }
    private void OnClick_Back()
    {
        //以下内容是在小关卡结束调用，先暂时这里
        battleMap.DOScale(Vector3.one, animSmoothTime);
        phase.DOAnchorPosY(0, animSmoothTime);
        btn_Time.DOAnchorPosX(-47, animSmoothTime);
        fOV.DOAnchorPosX(-35, animSmoothTime);
    }
    private void OnClick_Refresh()
    {
        RefreshHeroBox();
    }
    private void OnClick_AdBox()
    {
        HeroType heroType = Range(0f, 1) > 0.9f ? HeroType.Box_3 : HeroType.Box_2;
        string name = GetHeroName(heroType);

        AddressablesMgr.Instance.LoadAssetAsync<GameObject>(obj =>
        {
            GameObject insObj = Instantiate(obj);
            insObj.transform.SetParent(heroBoxBase, true);
            insObj.transform.localScale = Vector3.one;
            insObj.transform.position = addBoxPos.position;
            insObj.GetComponent<BoxHero>().isAdLock = false;

            insObj.transform.SetParent(heroBoxBase, true);
            int index = FindClosest(insObj.transform.position);
            //在指定索引处，插入
            if (index < heroBoxBase.childCount)
                insObj.transform.SetSiblingIndex(index);

            if (index >= heroBoxs.Count)
                heroBoxs.Add(insObj.transform);
            else
                heroBoxs.Insert(index, insObj.transform);
            CalculateLayout(index);
        }, name, StaticFields.Hero);

    }
    private void OnClick_Adadv()
    {
        AdsMgr.Instance.ShowRewardedVideoAd(() =>
        {
            RefreshHeroBox(true);
        });
    }
   
    #endregion

    #region 刷新卡池
    /// <summary>
    /// 刷新卡池
    /// </summary>
    /// <param name="isAdv">是否必出高级角色</param>
    public void RefreshHeroBox(bool isAdv = false)
    {
        foreach (var item in heroBoxs)
        {
            Destroy(item.gameObject);
        }
        heroBoxs.Clear();

        List<BackHeroData> backHeroes = gameController.GetRandomHeroes(isAdv);

        //全部加载完回调
        Action comp = () =>
        {
            CalculateLayout(isRotation: true);
        };
        int iaLoadFaw = 0;
        foreach (var item in backHeroes)
        {
            string name = GetHeroName(item.type);

            AddressablesMgr.Instance.LoadAssetAsync<GameObject>(obj =>
            {
                GameObject insObj = Instantiate(obj);
                insObj.transform.SetParent(heroBoxBase, true);
                insObj.transform.localScale = Vector3.one;
                insObj.transform.position = heroBoxBase.position;
                HeroBase heroBase = insObj.GetComponent<HeroBase>();
                if (heroBase != null)
                {
                    heroBase.level = item.level;
                    heroBase.isAdLock = item.isAdLock;
                }
                else
                {
                    BoxHero boxHero = insObj.GetComponent<BoxHero>();
                    boxHero.isAdLock = item.isAdLock;
                }
                heroBoxs.Add(insObj.transform);
                iaLoadFaw++;
                if (iaLoadFaw == backHeroes.Count)
                    comp();
            }, name, StaticFields.Hero);
        }
    }
    /// <summary>
    /// 根据标签获取资源名
    /// </summary>
    private string GetHeroName(HeroType type)
    {
        string[] names = heroNameDict[type];
        string name = names[0];
        if (type == HeroType.Box_2 || type == HeroType.Box_3)
            name = names[Range(0, names.Length)];
        return name;
    }
    #endregion

    #region 拖拽，松开，备用区动态布局排列
    /// <summary>
    /// 拖拽，松开 角色或格子处理
    /// </summary>
    private void PlaceHeroBox(UpHeroOrBox uphoero)
    {
        switch (uphoero.e_touchState)
        {
            case E_TouchState.Down://开始拖拽
                if (fieldHeros.Contains(uphoero.type))
                {
                    //处于上阵中角色被拖拽
                    fieldHeros.Remove(uphoero.type);
                    EventMgr.Instance.EventTrigger<Transform>(E_EventType.fieldHeroDrag, uphoero.type);
                }
                //uphoero.type.SetParent(transform, true);
                uphoero.type.SetAsLastSibling();
                uphoero.type.DOLocalRotate(new Vector3(0, 0, 0), animSmoothTime);
                if (heroBoxs.Contains(uphoero.type))
                    heroBoxs.Remove(uphoero.type);
                CalculateLayout();
                break;
            case E_TouchState.UpPlace://放入备用卡池区
                uphoero.type.SetParent(heroBoxBase, true);
                int index = FindClosest(uphoero.type.position);
                //在指定索引处，插入
                if (index < heroBoxBase.childCount)
                    uphoero.type.SetSiblingIndex(index);
                if (index >= heroBoxs.Count)
                    heroBoxs.Add(uphoero.type);
                else
                    heroBoxs.Insert(index, uphoero.type);
                CalculateLayout(index);
                break;
            case E_TouchState.UpField://上阵处理
                uphoero.type.SetParent(fieldHero, true);
                fieldHeros.Add(uphoero.type);
                if (heroBoxs.Contains(uphoero.type))
                    heroBoxs.Remove(uphoero.type);
                break;
        }
    }
    /// <summary>
    /// 获得插入位置索引
    /// </summary>
    private int FindClosest(Vector3 dragCardPos)
    {
        if (heroBoxs.Count == 0) return 0;
        int index = 0;
        float minDistance = float.MaxValue;

        for (int i = 0; i < heroBoxs.Count; i++)
        {
            // 计算当前物体与自己的 X 轴的距离
            float distance = Mathf.Abs(heroBoxs[i].position.x - dragCardPos.x);
            if (distance < minDistance)
            {
                minDistance = distance;
                index = i;
            }
        }
        //区分从左边插入还是右边插入
        if (heroBoxs[index].position.x > dragCardPos.x)
            return index;
        else
            return index + 1;
    }

    /// <summary>
    /// 备用区动态布局排列
    /// </summary>
    public void CalculateLayout(int addHeroIndex = -1, bool isRotation = false)
    {
        // 获取所有子物体列表
        var children = heroBoxs;
        if (children.Count < 1) return;

        // 获取父容物体宽度
        float parentWidth = heroBoxBase.GetComponent<RectTransform>().rect.width;
        // 计算所有卡牌的总宽度
        float totalWidth = children.Sum(rt => rt.GetComponent<RectTransform>().rect.width);
        // 存储所有卡牌的目标位置
        List<Vector3> targetPositions = new List<Vector3>();

        //卡牌总宽度不超过父容器
        if (totalWidth <= parentWidth)
        {
            //宽度足够，两边加上间隔
            float spacing = (parentWidth - totalWidth) / (children.Count + 1);
            // 第一张卡牌起始位置，父容器左边界坐标 + 第一个间距
            float currentX = -parentWidth / 2 + spacing;

            foreach (var child in children)
            {
                RectTransform rt = child.GetComponent<RectTransform>();
                // 目标位置 = 当前起点 + 卡牌半宽
                targetPositions.Add(new Vector3(currentX + rt.rect.width / 2, 0, 0));
                // 累计起始位置，计算每个卡片位置
                // 记录起点：当前位置 + 卡牌全宽 + 间距
                currentX += rt.rect.width + spacing;
            }
        }
        //卡牌总宽度超过父容器
        else
        {
            //宽度不够，两边不要间隔
            float overlap = (parentWidth - totalWidth) / (children.Count - 1);
            // 第一张卡牌起始位置，父容器左边界
            float currentX = -parentWidth / 2;
            foreach (var child in children)
            {
                RectTransform rt = child.GetComponent<RectTransform>();
                targetPositions.Add(new Vector3(currentX + rt.rect.width / 2, 0, 0));
                currentX += rt.rect.width + overlap;
            }
        }
        // 计算完每张卡牌位置，统一移动
        for (int i = 0; i < children.Count; i++)
        {
            RectTransform rt = children[i].GetComponent<RectTransform>();
            // 解决闭包
            int currentIndex = i;
            rt.DOKill();
            if (isRotation || (addHeroIndex != -1 && addHeroIndex == currentIndex))
            {
                //Sequence，可以同时管理多个动画播放等
                Sequence sequence = DOTween.Sequence();
                sequence.Append(rt.DOLocalMove(targetPositions[i], animSmoothTime).SetEase(Ease.OutQuad));
                sequence.Join(rt.DORotate(new Vector3(0, 0, UnityEngine.Random.value > 0.5f ? 13 : -13), animSmoothTime));
            }
            else
            {
                rt.DOLocalMove(targetPositions[i], animSmoothTime)
                .SetEase(Ease.OutQuad); // 使用先快后慢的缓动曲线
            }
        }
    }
    #endregion

    #region 封装一层随机数API
    private float Range(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }
    private int Range(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }
    #endregion
}
