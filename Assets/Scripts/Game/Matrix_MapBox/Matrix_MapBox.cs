using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.Profiling;
using UnityEngine.UI;

/// <summary>
/// 管理放置角色的地图矩阵
/// </summary>
public class Matrix_MapBox : MonoBehaviour
{
    private Transform mapBG;//地图背景
    private Transform curBoxsBase;//角色可放置格子矩阵父物体
    private Transform mapBoxBase;//地图可放置格子矩阵父物体

    /// <summary>
    /// 存储所有放置格子的信息
    /// </summary>
    private Transform[,] curBoxs;
    private Transform[,] mapBoxs;

    private float mapImgSpeed = 0.2f;//地图渐变速度
    void Start()
    {
        mapBG = transform.Find("BattleMap/mapBG").transform;
        curBoxsBase = transform.Find("BattleMap/Matrix_MapBox/Current_Box").transform;
        mapBoxBase = transform.Find("BattleMap/Matrix_MapBox/_AddBox").transform;

        curBoxs = new Transform[5, 7];
        int x = 0;
        int y = 0;
        foreach (Transform item in curBoxsBase)
        {
            curBoxs[x, y] = item.GetComponent<Transform>();
            //初始化角色可用格子区域
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
        mapBoxs = new Transform[5, 7];
        x = 0;
        y = 0;
        foreach (Transform item in mapBoxBase)
        {
            item.GetComponent<Image>().DOFade(0, 0);//设置透明
            //初始化地图可用格子区域
            if (y >= 2 && y <= 4 && x != 0 && x != 4)
            {
                Destroy(item.gameObject);
                mapBoxs[x, y] = null;
            }
            else
                mapBoxs[x, y] = item.GetComponent<Transform>();

            if (++x == 5)
            {
                x = 0;
                y++;
            }
        }

        EventMgr.Instance.AddEventListener<DragHero>(E_EventType.dragHero, IsHeroTrigger);
        EventMgr.Instance.AddEventListener<DragBox>(E_EventType.dragBox, IsBoxTrigger);
        EventMgr.Instance.AddEventListener(E_EventType.boxStatus, BoxStatus);
    }
    private void OnDestroy()
    {
        EventMgr.Instance.RemoveEventListener<DragHero>(E_EventType.dragHero, IsHeroTrigger);
        EventMgr.Instance.RemoveEventListener<DragBox>(E_EventType.dragBox, IsBoxTrigger);
        EventMgr.Instance.RemoveEventListener(E_EventType.boxStatus, BoxStatus);
    }


    //临时存储符合放置位置的格子
    List<Transform> _nearestHeros = new List<Transform>();
    /// <summary>
    /// 拖拽角色 判断矩阵格子
    /// </summary>
    private void IsHeroTrigger(DragHero hero)
    {
        if (hero.isUpHero) return;
        _nearestHeros.Clear();
        // 重置所有格子的颜色
        foreach (Transform box in curBoxs)
        {
            if (box.gameObject.activeSelf)
                box.GetComponent<Image>().color = Color.white;
        }
        // 遍历 pos 的所有子对象
        foreach (Transform child in hero.pos)
        {
            Transform childRect = child.GetComponent<Transform>();
            Transform nearestBox = null;
            float minDistance = float.MaxValue;
            // 遍历所有格子，查找距离最近的那个
            foreach (Transform box in curBoxs)
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
            if (nearestBox != null && minDistance < 4.5f && !_nearestHeros.Contains(nearestBox))
                _nearestHeros.Add(nearestBox);
        }

        foreach(Transform itmeBox in _nearestHeros)
            itmeBox.GetComponent<Image>().color = _nearestHeros.Count == hero.pos.childCount ? Color.green : Color.grey;

        //松开拖拽处理
        if (hero.isUpBox)
        {
            if (_nearestHeros.Count == hero.pos.childCount)
            {
                //可以放置在矩阵上
                DragHero _draghreo = new DragHero();
                _draghreo.type = hero.type;
                //获取位置偏移
                _draghreo.deviation = _nearestHeros[0].transform.position - hero.pos.GetChild(0).position;
                EventMgr.Instance.EventTrigger<DragHero>(E_EventType.placeMatrix, _draghreo);
                foreach (var item in _nearestHeros)
                    item.GetComponent<Image>().color = Color.clear;//设置透明
            }
            else
            {
                hero.type.transform.DOKill();
                UpHeroOrBox upHero = new UpHeroOrBox();
                upHero.type = hero.type.transform;
                upHero.e_touchState = E_TouchState.UpPlace;
                EventMgr.Instance.EventTrigger<UpHeroOrBox>(E_EventType.placeHeroBox, upHero);
                foreach (var item in _nearestHeros)
                    item.GetComponent<Image>().color = Color.white;
            }

        }
    }

    List<Transform> _nearestBoxs = new List<Transform>();
    /// <summary>
    /// 拖拽格子和松开格子处理
    /// </summary>
    private void IsBoxTrigger(DragBox dragBox)
    {
        _nearestBoxs.Clear();
        foreach (Transform box in mapBoxs)
        {
            if (box != null)
            {
                box.GetChild(0).gameObject.SetActive(false);
                box.GetChild(0).GetComponent<Image>().color = Color.white;
            }
        }

        foreach (Transform child in dragBox.pos)
        {
            Transform childRect = child.GetComponent<Transform>();
            Transform nearestBox = null;
            float minDistance = float.MaxValue;
            foreach (Transform box in mapBoxs)
            {
                if (box != null)
                {
                    float dist = Vector2.Distance(box.position, childRect.position);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        nearestBox = box;
                    }
                }
            }
            if (nearestBox != null && minDistance < 4.5f && !_nearestBoxs.Contains(nearestBox))
                _nearestBoxs.Add(nearestBox);
        }
        foreach (Transform itmeBox in _nearestBoxs)
        {
            itmeBox.GetChild(0).gameObject.SetActive(true);
            if (_nearestBoxs.Count != dragBox.pos.childCount)
                itmeBox.GetChild(0).GetComponent<Image>().color = Color.grey;
        }
        //松开格子
        if (dragBox.isUp)
            UpBox(dragBox);
    }
    /// <summary>
    /// 松开格子
    /// </summary>
    /// <param name="dragBox"></param>
    private void UpBox(DragBox dragBox)
    {
        //放入备用区回调
        Action upPlace = () =>
        {
            UpHeroOrBox upHero = new UpHeroOrBox();
            upHero.type = dragBox.type;
            upHero.e_touchState = E_TouchState.UpPlace;
            EventMgr.Instance.EventTrigger<UpHeroOrBox>(E_EventType.placeHeroBox, upHero);
            foreach (var item in _nearestBoxs)
                item.GetChild(0).gameObject.SetActive(false);
            UpMapStatus();
        };

        if (_nearestBoxs.Count == dragBox.pos.childCount)
        {
            bool posLegal = false;
            foreach (Transform item in _nearestBoxs)
            {
                posLegal = GetCurBoxsActive(GetFindIndex(mapBoxs, item));
                if (posLegal)
                    break;
            }
            if (posLegal)//判断周围是否有激活的英雄格子
            {
                Vector3 movePos = _nearestBoxs[0].transform.position - dragBox.pos.GetChild(0).position;
                movePos = dragBox.type.position + movePos;
                dragBox.type.DOKill();
                dragBox.type.DOMove(movePos, 0.25f).SetEase(Ease.OutBounce).OnComplete(() =>
                {
                    Destroy(dragBox.type.gameObject);
                    foreach (Transform item in _nearestBoxs)
                    {
                        Vector2Int indexV2 = GetFindIndex(mapBoxs, item);
                        Destroy(item.gameObject);
                        mapBoxs[indexV2.x, indexV2.y] = null;
                        curBoxs[indexV2.x, indexV2.y].gameObject.SetActive(true);
                    }
                    UpMapStatus();
                });
            }
            else
                upPlace();
        }
        else
            upPlace();
    }
    /// <summary>
    /// 松开格子后设置地图背景缩放及矩阵位置
    /// </summary>
    private void UpMapStatus()
    {
        foreach (Transform item in mapBoxBase)
        {
            if (item != null)
                item.GetComponent<Image>().DOFade(0, mapImgSpeed);
        }
    }
    /// <summary>
    /// 开始拖拽格子
    /// </summary>
    private void BoxStatus()
    {
        foreach (Transform item in mapBoxBase)
        {
            if (item != null)
                item.GetComponent<Image>().DOFade(1, mapImgSpeed);
        }
    }
    

    /// <summary>
    /// 获取坐标
    /// </summary>
    private Vector2Int GetFindIndex(Transform[,] array, Transform target)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                if (array[i, j] != null && array[i, j] == target)
                {
                    return new Vector2Int(i, j);
                }
            }
        }
        return new Vector2Int(-1, -1);  // 未找到
    }

    /// <summary>
    /// 获取周围是否有激活的英雄格子，判断该位置是否可以放置格子
    /// </summary>
    private bool GetCurBoxsActive(Vector2Int index)
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0),  // 上
            new Vector2Int(-1, 0), // 下
            new Vector2Int(0, 1),  // 右
            new Vector2Int(0, -1)  // 左
        };

        foreach (var dir in directions)
        {
            // 计算目标坐标
            int x = index.x + dir.x;
            int y = index.y + dir.y;
            // 边界检查
            if (x >= 0 && x < curBoxs.GetLength(0) && y >= 0 && y < curBoxs.GetLength(1))
            {
                if(curBoxs[x, y].gameObject.activeSelf)
                    return true;
            }
        }
        return false;
    }


}
