using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
    private Transform boxBase;//矩阵父物体
    private Transform curBase;//角色可放置格子矩阵父物体
    private Transform mapBase;//地图可放置格子矩阵父物体
    /// <summary>
    /// 存储所有放置格子的信息
    /// </summary>
    private Transform[,] curBoxs;
    private Transform[,] mapBoxs;
    /// <summary>
    /// 所有已上阵的格子 key = 占用格子自身，value = 角色信息
    /// </summary>
    private Dictionary<Transform,Transform> curHerosDict;

    private float mapMoveSpeed = 0.2f;//地图渐变速度
    private Vector3 boxInitPos;//矩阵初始位置

    #region 配置地图背景 回弹时的_缩放及矩阵位置
    private readonly Dictionary<string, (float posY, float scaleY)> mapYDict = new Dictionary<string, (float, float)>
    {
        [""] = (74f, 0.45f),
        ["1"] = (27f, 0.59f),
        ["5"] = (132f, 0.59f),
        ["01"] = (-15f, 0.73f),
        ["15"] = (87f, 0.73f),
        ["56"] = (189f, 0.73f),
        ["015"] = (42f, 0.87f),
        ["156"] = (144f, 0.87f),
        ["0156"] = (97f, 1.00f),
    };
    private readonly Dictionary<string, (float posX, float scaleX)> mapXDict = new Dictionary<string, (float, float)>
    {
        [""] = (0f, 0.6f),
        ["0"] = (49.5f, 0.8f),
        ["4"] = (-52.5f, 0.8f),
        ["04"] = (0f, 1.0f),
    };
    #endregion


    void Start()
    {
        mapBG = transform.Find("BattleMap/mapHeroBG").transform;
        boxBase = transform.Find("BattleMap/Matrix_MapBox").transform;
        curBase = transform.Find("BattleMap/Matrix_MapBox/Current_Box").transform;
        mapBase = transform.Find("BattleMap/Matrix_MapBox/_AddBox").transform;

        boxInitPos = boxBase.localPosition;
        boxBase.localPosition = new Vector3(0, 74);
        mapBG.localScale = new Vector3(0.6f, 0.45f);

        curHerosDict = new Dictionary<Transform, Transform>();
        curBoxs = new Transform[5, 7];
        int x = 0;
        int y = 0;
        foreach (Transform item in curBase)
        {
            curBoxs[x, y] = item;
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
        foreach (Transform item in mapBase)
        {
            item.GetComponent<Image>().DOFade(0, 0);//设置透明
            //初始化地图可用格子区域
            if (y >= 2 && y <= 4 && x != 0 && x != 4)
            {
                Destroy(item.gameObject);
                mapBoxs[x, y] = null;
            }
            else
                mapBoxs[x, y] = item;

            if (++x == 5)
            {
                x = 0;
                y++;
            }
        }

        EventMgr.Instance.AddEventListener<DragHero>(E_EventType.dragHero, IsHeroTrigger);
        EventMgr.Instance.AddEventListener<DragBox>(E_EventType.dragBox, IsBoxTrigger);
        EventMgr.Instance.AddEventListener(E_EventType.startDragBox, StartDragBox);
        EventMgr.Instance.AddEventListener<Transform>(E_EventType.fieldHeroDrag, RemoveCurHerosDict);
        EventMgr.Instance.AddEventListener<bool>(E_EventType.setPlayGame, SetCur);
    }
    private void OnDestroy()
    {
        EventMgr.Instance.RemoveEventListener<DragHero>(E_EventType.dragHero, IsHeroTrigger);
        EventMgr.Instance.RemoveEventListener<DragBox>(E_EventType.dragBox, IsBoxTrigger);
        EventMgr.Instance.RemoveEventListener(E_EventType.startDragBox, StartDragBox);
        EventMgr.Instance.RemoveEventListener<Transform>(E_EventType.fieldHeroDrag, RemoveCurHerosDict);
        EventMgr.Instance.RemoveEventListener<bool>(E_EventType.setPlayGame, SetCur);
    }

    /// <summary>
    /// 开始游戏，设置透明
    /// </summary>
    private void SetCur(bool isPlay)
    {
        if (isPlay)
        {
            foreach (Transform item in curBoxs)
            {
                item.GetComponent<Image>().DOFade(0, mapMoveSpeed);
            }
        }
        else
        {
            foreach (Transform item in curBoxs)
            {
                if (!curHerosDict.ContainsKey(item))
                    item.GetComponent<Image>().DOFade(1, mapMoveSpeed);
            }
        }
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
            Transform nearestBox = null;
            float minDistance = float.MaxValue;
            // 遍历所有格子，查找距离最近的那个
            foreach (Transform box in curBoxs)
            {
                if (box.gameObject.activeSelf)
                {
                    float dist = Vector2.Distance(box.position, child.position);
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
        foreach (Transform itmeBox in _nearestHeros)
            itmeBox.GetComponent<Image>().color = _nearestHeros.Count == hero.pos.childCount ? Color.green : Color.grey;

        //松开拖拽处理
        if (hero.isUpBox)
        {
            UpHero(hero);
        }
    }
    /// <summary>
    /// 松开角色
    /// </summary>
    /// <param name="hero"></param>
    private void UpHero(DragHero hero)
    {
        if (hero.type.isSyn)
        {
            //如果松开的角色，被合成升级，则不执行后续
            foreach (var item in curHerosDict)
                item.Key.GetComponent<Image>().DOFade(0, mapMoveSpeed);
            return;
        }
        if (_nearestHeros.Count == hero.pos.childCount)
        {
            //可以放置在矩阵上
            DragHero _draghreo = new DragHero();
            _draghreo.type = hero.type;
            //获取位置偏移
            _draghreo.deviation = _nearestHeros[0].transform.position - hero.pos.GetChild(0).position;
            EventMgr.Instance.EventTrigger<DragHero>(E_EventType.placeMatrix, _draghreo);

            //获取该位置上已有的角色信息
            List<Transform> tempHeros = new List<Transform>();
            foreach (var item in _nearestHeros)
            {
                if (curHerosDict.ContainsKey(item))
                {
                    if (!tempHeros.Contains(curHerosDict[item]))
                        tempHeros.Add(curHerosDict[item]);
                    curHerosDict.Remove(item);
                }
                curHerosDict.Add(item, hero.type.transform);
            }

            foreach (Transform itemHero in tempHeros)
            {
                RemoveCurHerosDict(itemHero);
                float posX = (hero.type.GetComponent<RectTransform>().rect.width + itemHero.GetComponent<RectTransform>().rect.width) / 2;
                if (itemHero.position.x < hero.type.transform.position.x)
                    posX = -posX;
                Vector3 pos = new Vector3(itemHero.localPosition.x + posX, itemHero.localPosition.y + 2);
                itemHero.DOKill();
                //先左右移动
                itemHero.DOLocalMove(pos, mapMoveSpeed)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        //将挤下来的角色放回备用区
                        UpHeroOrBox upHero = new UpHeroOrBox();
                        upHero.type = itemHero;
                        upHero.e_touchState = E_TouchState.UpPlace;
                        EventMgr.Instance.EventTrigger<UpHeroOrBox>(E_EventType.placeHeroBox, upHero);
                    });
            }
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
        foreach (var item in curHerosDict)
            item.Key.GetComponent<Image>().DOFade(0, mapMoveSpeed);//已上阵的格子 设置透明
    }
    /// <summary>
    /// 角色下阵，或被挤下阵等
    /// </summary>
    private void RemoveCurHerosDict(Transform heroTr)
    {
        List<Transform> removeHeros = new List<Transform>();
        //遍历一个集合时，不能同时进行增删改
        foreach (var itemHero in curHerosDict)
        {
            if (itemHero.Value == heroTr && !removeHeros.Contains(itemHero.Key))
                removeHeros.Add(itemHero.Key);
        }
        //收集需要删除的key，查找完后同一删除
        foreach (var item in removeHeros)
        {
            curHerosDict.Remove(item);
        }
    }

    List<Transform> _nearestBoxs = new List<Transform>();
    /// <summary>
    /// 拖拽格子中
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
            Transform nearestBox = null;
            float minDistance = float.MaxValue;
            foreach (Transform box in mapBoxs)
            {
                if (box != null)
                {
                    float dist = Vector2.Distance(box.position, child.position);
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
        foreach (Transform item in mapBase)
        {
            if (item != null)
                item.GetComponent<Image>().DOFade(0, mapMoveSpeed);
        }

        #region 查找激活索引--已注释
        //List<string> indexY = new List<string>();
        //for (int j = 0; j < curBoxs.GetLength(1); j++)
        //{
        //    if (j >= 2 && j <= 4) continue;
        //    for (int i = 0; i < curBoxs.GetLength(0); i++)
        //    {
        //        if (curBoxs[i, j].gameObject.activeSelf)
        //        {
        //            indexY.Add(j.ToString());
        //            break;
        //        }
        //    }
        //}
        //string allindexY = "";
        //foreach (string itemHero in indexY)
        //{
        //    allindexY += itemHero;
        //}
        //List<string> indexX = new List<string>();
        //for (int i = 0; i < curBoxs.GetLength(0); i++)
        //{
        //    if (i >= 1 && i <= 3) continue;
        //    for (int j = 0; j < curBoxs.GetLength(1); j++)
        //    {
        //        if (curBoxs[i, j].gameObject.activeSelf)
        //        {
        //            indexX.Add(i.ToString());
        //            break;
        //        }
        //    }
        //}
        //string allindexX = "";
        //foreach (string itemHero in indexX)
        //{
        //    allindexX += itemHero;
        //}
        #endregion

        //获取纵横激活索引
        var allIndexY = string.Join("",
            Enumerable.Range(0, curBoxs.GetLength(1))
                .Where(j => j < 2 || j > 4)// 排除 2 3 4列            
                .Where(j => Enumerable.Range(0, curBoxs.GetLength(0))  
                .Any(i => curBoxs[i, j].gameObject.activeSelf))// 判断如果该列有任何激活
                .Select(j => j.ToString()));//拼接字符串

        var allIndexX = string.Join("",
            Enumerable.Range(0, curBoxs.GetLength(0))
                .Where(i => i < 1 || i > 3)
                .Where(i => Enumerable.Range(0, curBoxs.GetLength(1))
                .Any(j => curBoxs[i, j].gameObject.activeSelf))
                .Select(i => i.ToString()));

        //获取配置数据
        var (posY, scaleY) = mapYDict.TryGetValue(allIndexY, out var valY) ? valY : mapYDict[""];
        var (posX, scaleX) = mapXDict.TryGetValue(allIndexX, out var valX) ? valX : mapXDict[""];
        boxBase.DOKill();
        mapBG.DOKill();
        boxBase.DOLocalMove(new Vector3(posX, posY), mapMoveSpeed).SetEase(Ease.OutQuad);
        mapBG.DOScale(new Vector3(scaleX, scaleY), mapMoveSpeed).SetEase(Ease.OutQuad);
    }
    /// <summary>
    /// 开始拖拽格子
    /// </summary>
    private void StartDragBox()
    {
        foreach (Transform item in mapBase)
        {
            if (item != null)
                item.GetComponent<Image>().DOFade(1, mapMoveSpeed).SetEase(Ease.InCubic);
        }
        boxBase.DOKill();
        mapBG.DOKill();
        boxBase.DOLocalMove(boxInitPos, mapMoveSpeed).SetEase(Ease.OutQuad);
        mapBG.DOScale(new Vector3(1, 1), mapMoveSpeed).SetEase(Ease.OutQuad);
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
                if (curBoxs[x, y].gameObject.activeSelf)
                    return true;
            }
        }
        return false;
    }


}
