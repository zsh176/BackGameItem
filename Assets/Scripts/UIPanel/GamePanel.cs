using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : BasePanel
{
    private List<Transform> heroBoxs;//所有备用区角色信息
    private List<Transform> fieldHeros;//所有上阵中角色信息
    private List<Transform> enemyAllList;//存储场景中所有敌人
    private Transform heroBoxBase;//角色备用区
    private Transform fieldHero;//角色上阵中父物体
    private Transform enemyBoss;//敌人父物体
    private Transform bullet_Base;//子弹父物体

    private Transform addBoxPos;//生成格子的初始位置
    private Transform battleMap;//游戏场景父物体
    private Transform mapHeroBG;//防御塔位置
    private RectTransform phase;//准备阶段，卡池备用区
    private RectTransform btn_Time;//两倍速按钮
    private RectTransform fOV;//地图缩放按钮
    private RectTransform hpBase;//血条父物体
    private RectTransform sceneMapBG;//游戏场景地图背景

    private GameObject obj_SkipLevel;//跳过这波
    private GameObject obj_UpLevel;//升级

    private TextMeshProUGUI txt_level;//显示当前处于第几波
    private TextMeshProUGUI txt_ShowTime;//显示时间速度
    private TextMeshProUGUI txt_hp_value;//显示剩余血条
    private Slider fOV_Slider;//控制地图缩放滑动条
    private Image hp_ace;//血条进度

    private List<AwveData> allLevelData;//关卡数据

    private float animSmoothTime = 0.3f;//动画过度时间
    private float minMapScale = 0.38f;//地图最小缩放值
    private float maxMapScale = 0.9f;//地图最大缩放值
    private float fovMapScale = 0;//记录地图缩放比
    private float gameTimeSpeed = 1;//记录游戏中的时间流逝速度

    private int presentAllAwve;//当前关卡总波数
    private int presentAwve = 0;//当前处于第几波
    private int presentBatch = 0;//当前处于第几批次
    private int generateEnemyIndexID;//生成敌人计时器，用于关闭
    private bool isGameOver;//当前关卡是否结束

    private int playerLevel = 1;//玩家等级
    private int maxHP = 500;//最大血量
    private int nowHP;//当前血量

    #region 配置信息
    //根据标签方便获取英雄资源名
    Dictionary<HeroType, string[]> heroNameDict = new Dictionary<HeroType, string[]>()
    {
        { HeroType.Hero_10002, new [] { "Hero_10002" } },
        { HeroType.Hero_10004, new [] { "Hero_10004" }  },
        { HeroType.Hero_10005, new [] { "Hero_10005" }  },
        { HeroType.Hero_10006, new [] { "Hero_10006" }  },
        { HeroType.Hero_10014, new [] { "Hero_10014" } },
        { HeroType.Hero_10015, new [] { "Hero_10015" }  },
        { HeroType.Box_1, new [] { "Box_1one" }  },
        { HeroType.Box_2, new [] { "Box_2two_Horizontal", "Box_2two_Verical" }  },
        { HeroType.Box_3, new [] { "Box_3three_Horizontal", "Box_3three_Verical", "Box_3three_L" }  }
    };
    #endregion

    #region 配置角色随机概率
    // 创建概率字典（总概率100%） 权重(概率)可以修改，总权重可以超过100%
    Dictionary<HeroType, float> heroprobabilities = new Dictionary<HeroType, float>()
    {
        { HeroType.Hero_10002, 0.176f },
        { HeroType.Hero_10004, 0.176f },
        { HeroType.Hero_10005, 0.176f },
        { HeroType.Hero_10006, 0.176f },
        { HeroType.Hero_10014, 0.176f },
        { HeroType.Box_1, 0.07f },
        { HeroType.Box_2, 0.035f },
        { HeroType.Box_3, 0.015f }
    };
    Dictionary<int, float> levelprobabilities = new Dictionary<int, float>()
    {
        { 1,0.8f},
        { 2,0.15f},
        { 3,0},
        { 4,0},
    };
    List<HeroType> targetKeys = new List<HeroType>
    {
        HeroType.Hero_10002,
        HeroType.Hero_10004,
        HeroType.Hero_10005,
        HeroType.Hero_10006,
        HeroType.Hero_10014
    };
    #endregion

    //初始化
    public override void ShowMe()
    {
        heroBoxs = new List<Transform>();
        fieldHeros = new List<Transform>();
        enemyAllList = new List<Transform>();
        heroBoxBase = transform.Find("Phase/HeroBox");
        fieldHero = transform.Find("BattleMap/Matrix_MapBox/FieldHero");
        addBoxPos = transform.Find("Phase/AddBoxPos");
        battleMap = transform.Find("BattleMap");
        enemyBoss = transform.Find("BattleMap/Enemy_Base");
        mapHeroBG = transform.Find("BattleMap/mapHeroBG");
        bullet_Base = transform.Find("BattleMap/Bullet_Base");

        phase = transform.Find("Phase").GetComponent<RectTransform>();
        btn_Time = transform.Find("Gamethe/btn_Time").GetComponent<RectTransform>();
        fOV = transform.Find("Gamethe/FOV").GetComponent<RectTransform>();
        hpBase = transform.Find("hpBase").GetComponent<RectTransform>();
        sceneMapBG = transform.Find("BattleMap/SceneMap/BG").GetComponent<RectTransform>();

        obj_SkipLevel = transform.Find("Top/btn_SkipLevel").gameObject;
        obj_UpLevel = transform.Find("Top/btn_UpLevel").gameObject;

        txt_level = transform.Find("Top/level/txt_level").GetComponent<TextMeshProUGUI>();
        txt_ShowTime = transform.Find("Gamethe/btn_Time/txt_Time").GetComponent<TextMeshProUGUI>();
        txt_hp_value = transform.Find("hpBase/txt_hp_value").GetComponent<TextMeshProUGUI>();
        fOV_Slider = GetControl<Slider>("FOV_Slider");
        hp_ace = transform.Find("hpBase/hp_ace").GetComponent<Image>();

        allLevelData = FileSystemMgr.Instance.levelGameData.allLevelData;
        presentAllAwve = allLevelData.Count; 
        txt_level.text = $"第{presentAwve + 1}/{presentAllAwve}波";
        nowHP = maxHP;
        txt_hp_value.text = nowHP.ToString();
        hp_ace.fillAmount = 1;
        obj_SkipLevel.SetActive(false);
        RefreshHeroBox();

        EventMgr.Instance.AddEventListener<UpHeroOrBox>(E_EventType.placeHeroBox, PlaceHeroBox);
        EventMgr.Instance.AddEventListener<Transform>(E_EventType.enemyDeath, EnemyDeath);
        EventMgr.Instance.AddEventListener<BeAtkData>(E_EventType.playerBeAtk, PlayerBeAtk);
    }

    public override void HideMe()
    {
        EventMgr.Instance.RemoveEventListener<UpHeroOrBox>(E_EventType.placeHeroBox, PlaceHeroBox);
        EventMgr.Instance.RemoveEventListener<Transform>(E_EventType.enemyDeath, EnemyDeath);
        EventMgr.Instance.RemoveEventListener<BeAtkData>(E_EventType.playerBeAtk, PlayerBeAtk);
    }

    #region 绑定按钮等控件的事件
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
            case "btn_SkipLevel"://跳过这波
                OnClick_SkipLevel();
                break;
            case "btn_UpLevel"://升级
                if (++playerLevel > 10)
                    playerLevel = 10;
                PlayAnimMgr.Instance.ShowTips($"当前等级：{playerLevel}级");
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
            case "btn_Time"://倍速按钮
                OnClick_Time();
                break;
            case "btn_FOVAdd"://地图缩放 +
                OnClick_FOVAdd();
                break;
            case "btn_FOVDown"://地图缩放 -
                OnClick_FOVDown();
                break;
        }
    }
    protected override void OnValueChangedSlider(string sliderName, float value)
    {
        switch (sliderName)
        {
            case "FOV_Slider"://地图缩放
                OnValue_FOV(value);
                break;
        }
    }
    private void OnClick_Play()
    {
        GameStatus.Instance.offDrag = true;
        battleMap.DOScale(Vector3.one * Mathf.Lerp(minMapScale, maxMapScale, fovMapScale), animSmoothTime).OnComplete(() =>
        {   //开始游戏后，等动画都播放完毕，再把时间速度设置成上次使用速度
            Time.timeScale = gameTimeSpeed;
            PlayGanme();
            //清空卡池
            foreach (var item in heroBoxs)
            {
                Destroy(item.gameObject);
            }
            heroBoxs.Clear();
        });
        //基于锚点移动要用这个API
        phase.DOAnchorPosY(-410, animSmoothTime);
        btn_Time.DOAnchorPosX(47, animSmoothTime);
        fOV.DOAnchorPosX(35, animSmoothTime);
        hpBase.DOAnchorPosY(246.5f, animSmoothTime * 0.3f);
        EventMgr.Instance.EventTrigger<bool>(E_EventType.setPlayGame, true);
        obj_SkipLevel.SetActive(true);
        obj_UpLevel.SetActive(false);
    }
    private void PlayUIAnim()//进入准备阶段动画
    {
        GameStatus.Instance.offDrag = false;
        Time.timeScale = 1;
        battleMap.DOScale(Vector3.one, animSmoothTime).OnComplete(() =>
        {   //每次通过一波，自动刷新一次卡池
            RefreshHeroBox();
        });
        phase.DOAnchorPosY(0, animSmoothTime);
        btn_Time.DOAnchorPosX(-47, animSmoothTime);
        fOV.DOAnchorPosX(-35, animSmoothTime);
        hpBase.DOAnchorPosY(350.5f, animSmoothTime);
        EventMgr.Instance.EventTrigger<bool>(E_EventType.setPlayGame, false);
        obj_SkipLevel.SetActive(false);
        obj_UpLevel.SetActive(true);
    }
    private void OnClick_Back()
    {

    }
    private void OnClick_SkipLevel()
    {
        if (presentAwve == presentAllAwve - 1)
        {
            GameOver(true);
        }
        else
        {
            WinLevel();
        }
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
    private void OnClick_Time()
    {
        Time.timeScale = Time.timeScale == 1 ? 2 : 1;
        gameTimeSpeed = Time.timeScale;
        txt_ShowTime.text = $"加速×{gameTimeSpeed}";
    }
    private void OnClick_FOVAdd()
    {
        float value = fOV_Slider.value + 0.1f;
        fOV_Slider.DOValue(value, animSmoothTime);
        BtnOnClick_FOV(value);
    }
    private void OnClick_FOVDown()
    {
        float value = fOV_Slider.value - 0.1f;
        fOV_Slider.DOValue(value, animSmoothTime);
        BtnOnClick_FOV(value);
    }
    private void BtnOnClick_FOV(float sliderValue)
    {
        float targetScale = Mathf.Lerp(minMapScale, maxMapScale, sliderValue);
        battleMap.DOScale(Vector3.one * targetScale, animSmoothTime);
    }

    private void OnValue_FOV(float sliderValue)
    {
        // 将 0~1 映射成 0.38~0.9
        float targetScale = Mathf.Lerp(minMapScale, maxMapScale, sliderValue);
        battleMap.localScale = Vector3.one * targetScale;
        fovMapScale = sliderValue;
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

        List<BackHeroData> backHeroes = GetRandomHeroes(isAdv);

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
                    heroBase.bullet_Base = bullet_Base;
                    heroBase.sceneMapBG = sceneMapBG;
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
                //注释此处可以解决，快速点击时，物体直接跑到屏幕中间的问题
                uphoero.type.SetParent(transform, true);
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
                SortObjects(fieldHero);
                if (heroBoxs.Contains(uphoero.type))
                    heroBoxs.Remove(uphoero.type);
                break;
        }
    }

    /// <summary>
    /// 根据y轴排序物体
    /// </summary>
    private void SortObjects(Transform sorting)
    {
        var directChildren = new Transform[sorting.childCount];
        for (int i = 0; i < sorting.childCount; i++)
        {
            directChildren[i] = sorting.GetChild(i);
        }
        // 按 Y 轴升序排序（Y 小的在前）
        var sortedChildren = directChildren
            .OrderBy(child => child.localPosition.y)
            .ToList();
        // 逆序设置层级（Y 最小的最后设置，层级最高）
        for (int i = sortedChildren.Count - 1; i >= 0; i--)
            sortedChildren[i].SetAsLastSibling();
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
                sequence.Append(rt.DOLocalMove(targetPositions[i], animSmoothTime).SetEase(Ease.OutQuad));// 使用先快后慢的缓动曲线
                sequence.Join(rt.DORotate(new Vector3(0, 0, UnityEngine.Random.value > 0.5f ? 13 : -13), animSmoothTime));
            }
            else
            {
                rt.DOLocalMove(targetPositions[i], animSmoothTime)
                .SetEase(Ease.OutQuad);
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

    #region 随机获取角色相关
    /// <summary>
    /// 获取3个角色
    /// </summary>
    public List<BackHeroData> GetRandomHeroes(bool isAdv = false)
    {
        List<BackHeroData> results = new List<BackHeroData>();

        float randomPoint = Range(0f, 1);

        for (int i = 0; i < 3; i++)
        {
            if (isAdv)//必出一个高级角色
            {
                if (randomPoint < 0.334f)
                {
                    isAdv = false;
                    results.Add(new BackHeroData
                    {
                        type = targetKeys[Range(0, targetKeys.Count)],
                        level = Range(0f, 1) > 0.8f ? 4 : 3,
                    });
                    continue;
                }
                randomPoint -= 0.334f;
            }

            results.Add(new BackHeroData
            {
                type = GetHeroRandom(),
                level = GetLevelRandom()
            });
        }
        return results;
    }
    /// <summary>
    /// 随机获取一个角色
    /// </summary>
    private HeroType GetHeroRandom()
    {
        // 计算总权重
        float totalWeight = 0f;
        foreach (var pair in heroprobabilities)
            totalWeight += pair.Value;
        // 执行轮盘赌算法
        float randomPoint = Range(0f, totalWeight);
        foreach (var pair in heroprobabilities)
        {
            if (randomPoint < pair.Value)
            {
                return pair.Key;
            }
            randomPoint -= pair.Value;
        }
        // 保底返回第一个（理论上不会执行到这里）
        return HeroType.Hero_10002;
    }
    /// <summary>
    /// 随机获取等级
    /// </summary>
    private int GetLevelRandom()
    {
        Dictionary<int, float> dynamicWeights = new Dictionary<int, float>();
        //随着玩家等级提升，高等级卡牌概率增加
        foreach (var pair in levelprobabilities)
        {
            float weight = pair.Value;
            if (playerLevel > 1)
            {
                switch (pair.Key)
                {
                    case 2:
                        weight += (playerLevel - 1) * 0.125f;
                        break;
                    case 3:
                        weight += (playerLevel - 1) * 0.05f;
                        break;
                    case 4:
                        weight += (playerLevel - 1) * 0.016f;
                        break;
                }
            }
            dynamicWeights[pair.Key] = weight;
        }
        float totalWeight = 0;
        foreach (var w in dynamicWeights.Values)
            totalWeight += w;

        float randomPoint = Range(0f, totalWeight);
        foreach (var pair in dynamicWeights)
        {
            if (randomPoint < pair.Value)
                return pair.Key;
            randomPoint -= pair.Value;
        }
        return 1;
    }
    #endregion

    /******** 游戏流程相关 **********/

    #region 生成敌人相关
    /// <summary>
    /// 开始游戏
    /// </summary>
    private void PlayGanme()
    {
        isGameOver = false;
        presentBatch = 0;
        GenerateEnemy();
    }

    /// <summary>
    /// 生成敌人
    /// </summary>
    private void GenerateEnemy()
    {

        //用当前批次的敌人总数，除以该批次的敌人类型数，四舍五入得出，每个类型的敌人成几个
        int fewEnemy = Mathf.RoundToInt(allLevelData[presentAwve].batchDatas[presentBatch].fewEnemy / allLevelData[presentAwve].batchDatas[presentBatch].enemyName.Length);
       
        //Debug.Log($"第 {presentAwve + 1} 波的第 {presentBatch + 1} 批次，间隔 {allLevelData[presentAwve].batchDatas[presentBatch].Cooling} 秒后，再次生成敌人" +
        //  $"\n一共 {allLevelData[presentAwve].batchDatas[presentBatch].enemyName.Length} 个类型，每个类型生成 {fewEnemy} 个");

        for (int i = 0; i < allLevelData[presentAwve].batchDatas[presentBatch].enemyName.Length; i++)
        {
            for (int j = 0; j < fewEnemy; j++)
            {
                PoolMgr.Instance.GetObj(obj =>
                {
                    obj.transform.SetParent(enemyBoss, true);
                    EnemyBase enemy = obj.GetComponent<EnemyBase>();
                    enemy.Init(GeneratePositionAroundPoint(), mapHeroBG);

                    enemyAllList.Add(obj.transform);

                }, allLevelData[presentAwve].batchDatas[presentBatch].enemyName[i].ToString(), StaticFields.Enemy);
            }
        }
        if (allLevelData[presentAwve].batchDatas[presentBatch].isBoss && allLevelData[presentAwve].batchDatas[presentBatch].bossName != null)
        {   //boss批次
            for (int i = 0; i < allLevelData[presentAwve].batchDatas[presentBatch].bossName.Length; i++)
            {
                Debug.Log($"Boss关卡，生成：{allLevelData[presentAwve].batchDatas[presentBatch].bossName[i].ToString()}");
                PoolMgr.Instance.GetObj(obj =>
                {
                    obj.transform.SetParent(enemyBoss, true);
                    EnemyBase enemy = obj.GetComponent<EnemyBase>();
                    enemy.Init(GeneratePositionAroundPoint(), mapHeroBG);

                    enemyAllList.Add(obj.transform);

                }, allLevelData[presentAwve].batchDatas[presentBatch].bossName[i].ToString(), StaticFields.Enemy);
            }
        }
        EventMgr.Instance.EventTrigger<List<Transform>>(E_EventType.chaEnemyList, enemyAllList);
        SortObjects(enemyBoss);
        presentBatch++;
        if (presentBatch < allLevelData[presentAwve].batchDatas.Count)
        {
            generateEnemyIndexID = TimeMgr.Instance.AddTime(allLevelData[presentAwve].batchDatas[presentBatch].Cooling, () =>
            {
                //根据配置的间隔后，生成下一批
                GenerateEnemy();
            });
        }
    }
    /// <summary>
    /// 随机生成距离怪物位置
    /// </summary>
    public Vector2 GeneratePositionAroundPoint()
    {
        float randomAngle = Range(0f, 360f);
        float radians = randomAngle * Mathf.Deg2Rad;
        int offsetX = Mathf.RoundToInt(1500 * Mathf.Cos(radians));
        int offsetY = Mathf.RoundToInt(1500 * Mathf.Sin(radians));


        Vector2 point = new Vector2(mapHeroBG.position.x + offsetX, mapHeroBG.position.y + offsetY);

        if (point.x > (sceneMapBG.rect.width / 2))
            point.x = (sceneMapBG.rect.width / 2) + Range(0, 350);
        else if (point.x < -(sceneMapBG.rect.width / 2))
            point.x = -(sceneMapBG.rect.width / 2) - Range(0, 350);

        if (point.y > 0)
            point.y += Range(0, 350);
        else if (point.y < 0)
            point.y -= Range(0, 350);

        return point;
    }
    #endregion

    #region 判断是否通过小关，或过关
    /// <summary>
    /// 敌人死亡
    /// </summary>
    private void EnemyDeath(Transform enemy)
    {
        if (enemyAllList.Contains(enemy))
        {
            enemyAllList.Remove(enemy);
            EventMgr.Instance.EventTrigger<List<Transform>>(E_EventType.chaEnemyList, enemyAllList);
            if (enemyAllList.Count == 0 && presentBatch == allLevelData[presentAwve].batchDatas.Count && presentAwve == presentAllAwve - 1)
            {
                GameOver(true);
            }
            else if (enemyAllList.Count == 0 && presentBatch == allLevelData[presentAwve].batchDatas.Count)
            {
                WinLevel();
            }
        }
    }

    /// <summary>
    /// 玩家被攻击
    /// </summary>
    private void PlayerBeAtk(BeAtkData beAtk)
    {
        if (isGameOver) return;

        nowHP -= beAtk.harm;
        float hp = (float)nowHP / maxHP;
        hp_ace.fillAmount = hp;
        txt_hp_value.text = nowHP.ToString();

        if (nowHP <= 0)
        {
            isGameOver = true;
            //GameOver(false);
        }
    }

    /// <summary>
    /// 通过小关
    /// </summary>
    private void WinLevel()
    {
        isGameOver = true;
        TimeMgr.Instance.RemoveTime(generateEnemyIndexID);
        EventMgr.Instance.EventTrigger(E_EventType.skipLevel);
        enemyAllList.Clear();
        presentAwve++;
        PlayUIAnim();
        txt_level.text = $"第{presentAwve + 1}/{presentAllAwve}波";
        PlayAnimMgr.Instance.ShowTips($"通过第 {presentAwve} 波！！！");
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    private void GameOver(bool isWin)
    {
        isGameOver = true;
        if (isWin)
        {
            PlayAnimMgr.Instance.ShowTips($"-----游戏胜利-----");
        }
        else
        {
            PlayAnimMgr.Instance.ShowTips($"-----游戏失败-----");
        }
        
        presentAwve = 0;
        PlayUIAnim();

        TimeMgr.Instance.RemoveTime(generateEnemyIndexID);
        EventMgr.Instance.EventTrigger(E_EventType.skipLevel);
        enemyAllList.Clear();
        nowHP = maxHP;
        txt_hp_value.text = nowHP.ToString();
        hp_ace.fillAmount = 1;
        txt_level.text = $"第{presentAwve + 1}/{presentAllAwve}波";
    }

    
    #endregion

}

/// <summary>
/// 被攻击时传入的信息
/// </summary>
public struct BeAtkData
{
    /// <summary>
    /// 伤害值
    /// </summary>
    public int harm;
    /// <summary>
    /// 被攻击位置
    /// </summary>
    public Vector3 pos;
}

#region 随机返回的角色信息
/// <summary>
/// 随机返回的角色信息
/// </summary>
public class BackHeroData
{
    public HeroType type;
    public int level;
    public bool isAdLock;
}
#endregion