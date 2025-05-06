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
    private List<Transform> heroBoxs;//���б�������ɫ��Ϣ
    private List<Transform> fieldHeros;//���������н�ɫ��Ϣ
    private List<Transform> enemyAllList;//�洢���������е���
    private Transform heroBoxBase;//��ɫ������
    private Transform fieldHero;//��ɫ�����и�����
    private Transform enemyBoss;//���˸�����
    private Transform bullet_Base;//�ӵ�������

    private Transform addBoxPos;//���ɸ��ӵĳ�ʼλ��
    private Transform battleMap;//��Ϸ����������
    private Transform mapHeroBG;//������λ��
    private RectTransform phase;//׼���׶Σ����ر�����
    private RectTransform btn_Time;//�����ٰ�ť
    private RectTransform fOV;//��ͼ���Ű�ť
    private RectTransform hpBase;//Ѫ��������
    private RectTransform sceneMapBG;//��Ϸ������ͼ����

    private GameObject obj_SkipLevel;//�����Ⲩ
    private GameObject obj_UpLevel;//����

    private TextMeshProUGUI txt_level;//��ʾ��ǰ���ڵڼ���
    private TextMeshProUGUI txt_ShowTime;//��ʾʱ���ٶ�
    private TextMeshProUGUI txt_hp_value;//��ʾʣ��Ѫ��
    private Slider fOV_Slider;//���Ƶ�ͼ���Ż�����
    private Image hp_ace;//Ѫ������

    private List<AwveData> allLevelData;//�ؿ�����

    private float animSmoothTime = 0.3f;//��������ʱ��
    private float minMapScale = 0.38f;//��ͼ��С����ֵ
    private float maxMapScale = 0.9f;//��ͼ�������ֵ
    private float fovMapScale = 0;//��¼��ͼ���ű�
    private float gameTimeSpeed = 1;//��¼��Ϸ�е�ʱ�������ٶ�

    private int presentAllAwve;//��ǰ�ؿ��ܲ���
    private int presentAwve = 0;//��ǰ���ڵڼ���
    private int presentBatch = 0;//��ǰ���ڵڼ�����
    private int generateEnemyIndexID;//���ɵ��˼�ʱ�������ڹر�
    private bool isGameOver;//��ǰ�ؿ��Ƿ����

    private int playerLevel = 1;//��ҵȼ�
    private int maxHP = 500;//���Ѫ��
    private int nowHP;//��ǰѪ��

    #region ������Ϣ
    //���ݱ�ǩ�����ȡӢ����Դ��
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

    #region ���ý�ɫ�������
    // ���������ֵ䣨�ܸ���100%�� Ȩ��(����)�����޸ģ���Ȩ�ؿ��Գ���100%
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

    //��ʼ��
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
        txt_level.text = $"��{presentAwve + 1}/{presentAllAwve}��";
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

    #region �󶨰�ť�ȿؼ����¼�
    protected override void OnClick(string btnName)
    {
        switch (btnName)
        {
            case "btn_Play"://��ս����ʼ��Ϸ
                OnClick_Play();
                break;
            case "btn_Back"://�˳���Ϸ
                OnClick_Back();
                break;
            case "btn_SkipLevel"://�����Ⲩ
                OnClick_SkipLevel();
                break;
            case "btn_UpLevel"://����
                if (++playerLevel > 10)
                    playerLevel = 10;
                PlayAnimMgr.Instance.ShowTips($"��ǰ�ȼ���{playerLevel}��");
                break;
            case "btn_Refresh"://��ͨˢ��
                OnClick_Refresh();
                break;
            case "btn_AdBox"://���Ӹ���
                OnClick_AdBox();
                break;
            case "btn_Adadv"://�߼�ˢ�£��س��߼���ɫ
                OnClick_Adadv();
                break;
            case "btn_Time"://���ٰ�ť
                OnClick_Time();
                break;
            case "btn_FOVAdd"://��ͼ���� +
                OnClick_FOVAdd();
                break;
            case "btn_FOVDown"://��ͼ���� -
                OnClick_FOVDown();
                break;
        }
    }
    protected override void OnValueChangedSlider(string sliderName, float value)
    {
        switch (sliderName)
        {
            case "FOV_Slider"://��ͼ����
                OnValue_FOV(value);
                break;
        }
    }
    private void OnClick_Play()
    {
        GameStatus.Instance.offDrag = true;
        battleMap.DOScale(Vector3.one * Mathf.Lerp(minMapScale, maxMapScale, fovMapScale), animSmoothTime).OnComplete(() =>
        {   //��ʼ��Ϸ�󣬵ȶ�����������ϣ��ٰ�ʱ���ٶ����ó��ϴ�ʹ���ٶ�
            Time.timeScale = gameTimeSpeed;
            PlayGanme();
            //��տ���
            foreach (var item in heroBoxs)
            {
                Destroy(item.gameObject);
            }
            heroBoxs.Clear();
        });
        //����ê���ƶ�Ҫ�����API
        phase.DOAnchorPosY(-410, animSmoothTime);
        btn_Time.DOAnchorPosX(47, animSmoothTime);
        fOV.DOAnchorPosX(35, animSmoothTime);
        hpBase.DOAnchorPosY(246.5f, animSmoothTime * 0.3f);
        EventMgr.Instance.EventTrigger<bool>(E_EventType.setPlayGame, true);
        obj_SkipLevel.SetActive(true);
        obj_UpLevel.SetActive(false);
    }
    private void PlayUIAnim()//����׼���׶ζ���
    {
        GameStatus.Instance.offDrag = false;
        Time.timeScale = 1;
        battleMap.DOScale(Vector3.one, animSmoothTime).OnComplete(() =>
        {   //ÿ��ͨ��һ�����Զ�ˢ��һ�ο���
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
            //��ָ��������������
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
        txt_ShowTime.text = $"���١�{gameTimeSpeed}";
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
        // �� 0~1 ӳ��� 0.38~0.9
        float targetScale = Mathf.Lerp(minMapScale, maxMapScale, sliderValue);
        battleMap.localScale = Vector3.one * targetScale;
        fovMapScale = sliderValue;
    }
    #endregion

    #region ˢ�¿���
    /// <summary>
    /// ˢ�¿���
    /// </summary>
    /// <param name="isAdv">�Ƿ�س��߼���ɫ</param>
    public void RefreshHeroBox(bool isAdv = false)
    {
        foreach (var item in heroBoxs)
        {
            Destroy(item.gameObject);
        }
        heroBoxs.Clear();

        List<BackHeroData> backHeroes = GetRandomHeroes(isAdv);

        //ȫ��������ص�
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
    /// ���ݱ�ǩ��ȡ��Դ��
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

    #region ��ק���ɿ�����������̬��������
    /// <summary>
    /// ��ק���ɿ� ��ɫ����Ӵ���
    /// </summary>
    private void PlaceHeroBox(UpHeroOrBox uphoero)
    {
        switch (uphoero.e_touchState)
        {
            case E_TouchState.Down://��ʼ��ק
                if (fieldHeros.Contains(uphoero.type))
                {
                    //���������н�ɫ����ק
                    fieldHeros.Remove(uphoero.type);
                    EventMgr.Instance.EventTrigger<Transform>(E_EventType.fieldHeroDrag, uphoero.type);
                }
                //ע�ʹ˴����Խ�������ٵ��ʱ������ֱ���ܵ���Ļ�м������
                uphoero.type.SetParent(transform, true);
                uphoero.type.SetAsLastSibling();
                uphoero.type.DOLocalRotate(new Vector3(0, 0, 0), animSmoothTime);
                if (heroBoxs.Contains(uphoero.type))
                    heroBoxs.Remove(uphoero.type);
                CalculateLayout();
                break;
            case E_TouchState.UpPlace://���뱸�ÿ�����
                uphoero.type.SetParent(heroBoxBase, true);
                int index = FindClosest(uphoero.type.position);
                //��ָ��������������
                if (index < heroBoxBase.childCount)
                    uphoero.type.SetSiblingIndex(index);
                if (index >= heroBoxs.Count)
                    heroBoxs.Add(uphoero.type);
                else
                    heroBoxs.Insert(index, uphoero.type);
                CalculateLayout(index);
                break;
            case E_TouchState.UpField://������
                uphoero.type.SetParent(fieldHero, true);
                fieldHeros.Add(uphoero.type);
                SortObjects(fieldHero);
                if (heroBoxs.Contains(uphoero.type))
                    heroBoxs.Remove(uphoero.type);
                break;
        }
    }

    /// <summary>
    /// ����y����������
    /// </summary>
    private void SortObjects(Transform sorting)
    {
        var directChildren = new Transform[sorting.childCount];
        for (int i = 0; i < sorting.childCount; i++)
        {
            directChildren[i] = sorting.GetChild(i);
        }
        // �� Y ����������Y С����ǰ��
        var sortedChildren = directChildren
            .OrderBy(child => child.localPosition.y)
            .ToList();
        // �������ò㼶��Y ��С��������ã��㼶��ߣ�
        for (int i = sortedChildren.Count - 1; i >= 0; i--)
            sortedChildren[i].SetAsLastSibling();
    }
    /// <summary>
    /// ��ò���λ������
    /// </summary>
    private int FindClosest(Vector3 dragCardPos)
    {
        if (heroBoxs.Count == 0) return 0;
        int index = 0;
        float minDistance = float.MaxValue;

        for (int i = 0; i < heroBoxs.Count; i++)
        {
            // ���㵱ǰ�������Լ��� X ��ľ���
            float distance = Mathf.Abs(heroBoxs[i].position.x - dragCardPos.x);
            if (distance < minDistance)
            {
                minDistance = distance;
                index = i;
            }
        }
        //���ִ���߲��뻹���ұ߲���
        if (heroBoxs[index].position.x > dragCardPos.x)
            return index;
        else
            return index + 1;
    }

    /// <summary>
    /// ��������̬��������
    /// </summary>
    public void CalculateLayout(int addHeroIndex = -1, bool isRotation = false)
    {
        // ��ȡ�����������б�
        var children = heroBoxs;
        if (children.Count < 1) return;

        // ��ȡ����������
        float parentWidth = heroBoxBase.GetComponent<RectTransform>().rect.width;
        // �������п��Ƶ��ܿ��
        float totalWidth = children.Sum(rt => rt.GetComponent<RectTransform>().rect.width);
        // �洢���п��Ƶ�Ŀ��λ��
        List<Vector3> targetPositions = new List<Vector3>();

        //�����ܿ�Ȳ�����������
        if (totalWidth <= parentWidth)
        {
            //����㹻�����߼��ϼ��
            float spacing = (parentWidth - totalWidth) / (children.Count + 1);
            // ��һ�ſ�����ʼλ�ã���������߽����� + ��һ�����
            float currentX = -parentWidth / 2 + spacing;

            foreach (var child in children)
            {
                RectTransform rt = child.GetComponent<RectTransform>();
                // Ŀ��λ�� = ��ǰ��� + ���ư��
                targetPositions.Add(new Vector3(currentX + rt.rect.width / 2, 0, 0));
                // �ۼ���ʼλ�ã�����ÿ����Ƭλ��
                // ��¼��㣺��ǰλ�� + ����ȫ�� + ���
                currentX += rt.rect.width + spacing;
            }
        }
        //�����ܿ�ȳ���������
        else
        {
            //��Ȳ��������߲�Ҫ���
            float overlap = (parentWidth - totalWidth) / (children.Count - 1);
            // ��һ�ſ�����ʼλ�ã���������߽�
            float currentX = -parentWidth / 2;
            foreach (var child in children)
            {
                RectTransform rt = child.GetComponent<RectTransform>();
                targetPositions.Add(new Vector3(currentX + rt.rect.width / 2, 0, 0));
                currentX += rt.rect.width + overlap;
            }
        }
        // ������ÿ�ſ���λ�ã�ͳһ�ƶ�
        for (int i = 0; i < children.Count; i++)
        {
            RectTransform rt = children[i].GetComponent<RectTransform>();
            // ����հ�
            int currentIndex = i;
            rt.DOKill();
            if (isRotation || (addHeroIndex != -1 && addHeroIndex == currentIndex))
            {
                //Sequence������ͬʱ�������������ŵ�
                Sequence sequence = DOTween.Sequence();
                sequence.Append(rt.DOLocalMove(targetPositions[i], animSmoothTime).SetEase(Ease.OutQuad));// ʹ���ȿ�����Ļ�������
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

    #region ��װһ�������API
    private float Range(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }
    private int Range(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }
    #endregion

    #region �����ȡ��ɫ���
    /// <summary>
    /// ��ȡ3����ɫ
    /// </summary>
    public List<BackHeroData> GetRandomHeroes(bool isAdv = false)
    {
        List<BackHeroData> results = new List<BackHeroData>();

        float randomPoint = Range(0f, 1);

        for (int i = 0; i < 3; i++)
        {
            if (isAdv)//�س�һ���߼���ɫ
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
    /// �����ȡһ����ɫ
    /// </summary>
    private HeroType GetHeroRandom()
    {
        // ������Ȩ��
        float totalWeight = 0f;
        foreach (var pair in heroprobabilities)
            totalWeight += pair.Value;
        // ִ�����̶��㷨
        float randomPoint = Range(0f, totalWeight);
        foreach (var pair in heroprobabilities)
        {
            if (randomPoint < pair.Value)
            {
                return pair.Key;
            }
            randomPoint -= pair.Value;
        }
        // ���׷��ص�һ���������ϲ���ִ�е����
        return HeroType.Hero_10002;
    }
    /// <summary>
    /// �����ȡ�ȼ�
    /// </summary>
    private int GetLevelRandom()
    {
        Dictionary<int, float> dynamicWeights = new Dictionary<int, float>();
        //������ҵȼ��������ߵȼ����Ƹ�������
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

    /******** ��Ϸ������� **********/

    #region ���ɵ������
    /// <summary>
    /// ��ʼ��Ϸ
    /// </summary>
    private void PlayGanme()
    {
        isGameOver = false;
        presentBatch = 0;
        GenerateEnemy();
    }

    /// <summary>
    /// ���ɵ���
    /// </summary>
    private void GenerateEnemy()
    {

        //�õ�ǰ���εĵ������������Ը����εĵ�������������������ó���ÿ�����͵ĵ��˳ɼ���
        int fewEnemy = Mathf.RoundToInt(allLevelData[presentAwve].batchDatas[presentBatch].fewEnemy / allLevelData[presentAwve].batchDatas[presentBatch].enemyName.Length);
       
        //Debug.Log($"�� {presentAwve + 1} ���ĵ� {presentBatch + 1} ���Σ���� {allLevelData[presentAwve].batchDatas[presentBatch].Cooling} ����ٴ����ɵ���" +
        //  $"\nһ�� {allLevelData[presentAwve].batchDatas[presentBatch].enemyName.Length} �����ͣ�ÿ���������� {fewEnemy} ��");

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
        {   //boss����
            for (int i = 0; i < allLevelData[presentAwve].batchDatas[presentBatch].bossName.Length; i++)
            {
                Debug.Log($"Boss�ؿ������ɣ�{allLevelData[presentAwve].batchDatas[presentBatch].bossName[i].ToString()}");
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
                //�������õļ����������һ��
                GenerateEnemy();
            });
        }
    }
    /// <summary>
    /// ������ɾ������λ��
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

    #region �ж��Ƿ�ͨ��С�أ������
    /// <summary>
    /// ��������
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
    /// ��ұ�����
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
    /// ͨ��С��
    /// </summary>
    private void WinLevel()
    {
        isGameOver = true;
        TimeMgr.Instance.RemoveTime(generateEnemyIndexID);
        EventMgr.Instance.EventTrigger(E_EventType.skipLevel);
        enemyAllList.Clear();
        presentAwve++;
        PlayUIAnim();
        txt_level.text = $"��{presentAwve + 1}/{presentAllAwve}��";
        PlayAnimMgr.Instance.ShowTips($"ͨ���� {presentAwve} ��������");
    }

    /// <summary>
    /// ��Ϸ����
    /// </summary>
    private void GameOver(bool isWin)
    {
        isGameOver = true;
        if (isWin)
        {
            PlayAnimMgr.Instance.ShowTips($"-----��Ϸʤ��-----");
        }
        else
        {
            PlayAnimMgr.Instance.ShowTips($"-----��Ϸʧ��-----");
        }
        
        presentAwve = 0;
        PlayUIAnim();

        TimeMgr.Instance.RemoveTime(generateEnemyIndexID);
        EventMgr.Instance.EventTrigger(E_EventType.skipLevel);
        enemyAllList.Clear();
        nowHP = maxHP;
        txt_hp_value.text = nowHP.ToString();
        hp_ace.fillAmount = 1;
        txt_level.text = $"��{presentAwve + 1}/{presentAllAwve}��";
    }

    
    #endregion

}

/// <summary>
/// ������ʱ�������Ϣ
/// </summary>
public struct BeAtkData
{
    /// <summary>
    /// �˺�ֵ
    /// </summary>
    public int harm;
    /// <summary>
    /// ������λ��
    /// </summary>
    public Vector3 pos;
}

#region ������صĽ�ɫ��Ϣ
/// <summary>
/// ������صĽ�ɫ��Ϣ
/// </summary>
public class BackHeroData
{
    public HeroType type;
    public int level;
    public bool isAdLock;
}
#endregion