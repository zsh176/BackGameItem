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
    public List<Transform> heroBoxs;//所有备用区角色信息
    private List<Transform> fieldHeros;//所有上阵中角色信息
    private Transform heroBoxBase;//角色备用区
    private Transform fieldHero;//角色上阵中
    private float transitionDuration = 0.3f;//动画过度时间

    //初始化
    public override void ShowMe()
    {
        heroBoxs = new List<Transform>();
        fieldHeros = new List<Transform>(); 
        heroBoxBase = transform.Find("Phase/HeroBox").transform;
        fieldHero = transform.Find("BattleMap/Matrix_MapBox/FieldHero").transform;

        foreach (Transform item in heroBoxBase)
        {
            heroBoxs.Add(item);
        }

        CalculateLayout(isRotation: true);

        EventMgr.Instance.AddEventListener<UpHeroOrBox>(E_EventType.placeHeroBox, PlaceHeroBox);
    }

    /// <summary>
    /// 拖拽，松开 角色或对象处理
    /// </summary>
    private void PlaceHeroBox(UpHeroOrBox uphoero)
    {
        switch (uphoero.e_touchState)
        {
            case E_TouchState.Down://开始拖拽
                uphoero.type.SetParent(transform, true);
                uphoero.type.SetAsLastSibling();
                uphoero.type.DORotate(new Vector3(0, 0, 0), transitionDuration);
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
    public void CalculateLayout(int addHeroIndex = -1,bool isRotation = false)
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
        // 计算完每个卡牌位置，统一移动
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
                sequence.Append(rt.DOLocalMove(targetPositions[i], transitionDuration).SetEase(Ease.OutQuad));
                sequence.Join(rt.DORotate(new Vector3(0, 0, UnityEngine.Random.value > 0.5f ? 13 : -13), transitionDuration));
            }
            else
            {
                rt.DOLocalMove(targetPositions[i], transitionDuration)
                .SetEase(Ease.OutQuad); // 使用先快后慢的缓动曲线
            }
        }
    }
}
