using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 管理放置角色的地图矩阵
/// </summary>
public class Matrix_MapBox : MonoBehaviour
{

    private Transform curBoxsBase;//可放置格子矩阵父物体
    /// <summary>
    /// 存储所有放置格子的信息
    /// </summary>
    public RectTransform[,] curBoxs;
    void Start()
    {
        curBoxsBase = transform.Find("BattleMap/Matrix_MapBox/Current_Box").transform;
        curBoxs = new RectTransform[5, 7];
        int x = 0;
        int y = 0;
        foreach (Transform item in curBoxsBase)
        {
            curBoxs[x, y] = item.GetComponent<RectTransform>();

            //初始化可用格子区域
            if (y <= 1 || y >= 5)
                item.gameObject.SetActive(false);
            else if (x == 0 || x == 4)
                item.gameObject.SetActive(false);

            if (++x == 5)
            {
                x = 0;
                y++;
            }
        }

        EventMgr.Instance.AddEventListener<DragHero>(E_EventType.dragHero, IsHeroTrigger);
    }

    void Update()
    {
      
    }

    //临时存储符合放置位置的格子
    List<RectTransform> _nearestBoxs = new List<RectTransform>();
    /// <summary>
    /// 拖拽角色 判断矩阵格子
    /// </summary>
    private void IsHeroTrigger(DragHero hero)
    {
        if (hero.isUpHero) return;
        _nearestBoxs.Clear();
        // 重置所有格子的颜色
        foreach (RectTransform box in curBoxs)
        {
            if (box.gameObject.activeSelf)
                box.GetComponent<Image>().color = Color.white;
        }
        // 遍历 pos 的所有子对象
        foreach (Transform child in hero.pos)
        {
            RectTransform childRect = child.GetComponent<RectTransform>();
            RectTransform nearestBox = null;
            float minDistance = float.MaxValue;
            // 遍历所有格子，查找距离最近的那个
            foreach (RectTransform box in curBoxs)
            {
                if (box.gameObject.activeSelf)
                {
                    float dist = Vector2.Distance(box.position, childRect.position);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        nearestBox = box;
                    }
                }
            }
            // 如果最近的格子距离在允许范围内，则标记该格子
            if (nearestBox != null && minDistance < 6.5f && !_nearestBoxs.Contains(nearestBox))
                _nearestBoxs.Add(nearestBox);
        }

        foreach(RectTransform itmeBox in _nearestBoxs)
        {
            itmeBox.GetComponent<Image>().color = _nearestBoxs.Count == hero.pos.childCount ? Color.green : Color.grey;
        }

        //松开拖拽处理
        if (hero.isUpBox)
        {
            if (_nearestBoxs.Count == hero.pos.childCount)
            {
                //可以放置在矩阵上
                DragHero _draghreo = new DragHero();
                _draghreo.type = hero.type;
                //获取位置偏移
                _draghreo.deviation = _nearestBoxs[0].transform.position - hero.pos.GetChild(0).position;
                EventMgr.Instance.EventTrigger<DragHero>(E_EventType.placeMatrix, _draghreo);
                foreach (var item in _nearestBoxs)
                {
                    item.GetComponent<Image>().color = Color.clear;//设置透明
                }
            }
            else
            {
                hero.UpHeroOnCilck?.Invoke();
                foreach (var item in _nearestBoxs)
                {
                    item.GetComponent<Image>().color = Color.white;
                }
            }

        }
    }
}
