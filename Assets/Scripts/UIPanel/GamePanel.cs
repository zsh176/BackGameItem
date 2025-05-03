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
    private List<Transform> heroBoxs;//���б�������ɫ��Ϣ
    private List<Transform> fieldHeros;//���������н�ɫ��Ϣ
    private Transform heroBoxBase;//��ɫ������
    private Transform fieldHero;//��ɫ������
    private Transform addBoxPos;//���ɸ��ӵĳ�ʼλ��
    private float animSmoothTime = 0.3f;//��������ʱ��
    private GameController gameController;//��Ϸ���̿�����


    private Transform battleMap;//��Ϸ����
    private RectTransform phase;//׼���׶Σ����ر�����
    private RectTransform btn_Time;//�����ٰ�ť
    private RectTransform fOV;//��ͼ���Ű�ť

    #region ����Ӣ����Դ��
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

    //��ʼ��
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
            case "btn_Play"://��ս����ʼ��Ϸ
                OnClick_Play();
                break;
            case "btn_Back"://�˳���Ϸ
                OnClick_Back();
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
            
        }
    }

    #region ��ť�¼�
    private void OnClick_Play()
    {
        battleMap.DOScale(new Vector3(0.45f, 0.45f), animSmoothTime);
        //����ê���ƶ�Ҫ�����API
        phase.DOAnchorPosY(-410, animSmoothTime);
        btn_Time.DOAnchorPosX(47, animSmoothTime);
        fOV.DOAnchorPosX(35, animSmoothTime);
    }
    private void OnClick_Back()
    {
        //������������С�ؿ��������ã�����ʱ����
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

        List<BackHeroData> backHeroes = gameController.GetRandomHeroes(isAdv);

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
                //uphoero.type.SetParent(transform, true);
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
                if (heroBoxs.Contains(uphoero.type))
                    heroBoxs.Remove(uphoero.type);
                break;
        }
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
                sequence.Append(rt.DOLocalMove(targetPositions[i], animSmoothTime).SetEase(Ease.OutQuad));
                sequence.Join(rt.DORotate(new Vector3(0, 0, UnityEngine.Random.value > 0.5f ? 13 : -13), animSmoothTime));
            }
            else
            {
                rt.DOLocalMove(targetPositions[i], animSmoothTime)
                .SetEase(Ease.OutQuad); // ʹ���ȿ�����Ļ�������
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
}
