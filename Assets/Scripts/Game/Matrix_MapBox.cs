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
/// ������ý�ɫ�ĵ�ͼ����
/// </summary>
public class Matrix_MapBox : MonoBehaviour
{
    private Transform mapBG;//��ͼ����
    private Transform boxBase;//��������
    private Transform curBase;//��ɫ�ɷ��ø��Ӿ�������
    private Transform mapBase;//��ͼ�ɷ��ø��Ӿ�������
    /// <summary>
    /// �洢���з��ø��ӵ���Ϣ
    /// </summary>
    private Transform[,] curBoxs;
    private Transform[,] mapBoxs;
    /// <summary>
    /// ����������ĸ��� key = ռ�ø�������value = ��ɫ��Ϣ
    /// </summary>
    private Dictionary<Transform,Transform> curHerosDict;

    private float mapMoveSpeed = 0.2f;//��ͼ�����ٶ�
    private Vector3 boxInitPos;//�����ʼλ��

    #region ���õ�ͼ���� �ص�ʱ��_���ż�����λ��
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
            //��ʼ����ɫ���ø�������
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
            item.GetComponent<Image>().DOFade(0, 0);//����͸��
            //��ʼ����ͼ���ø�������
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
    /// ��ʼ��Ϸ������͸��
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


    //��ʱ�洢���Ϸ���λ�õĸ���
    List<Transform> _nearestHeros = new List<Transform>();
    /// <summary>
    /// ��ק��ɫ �жϾ������
    /// </summary>
    private void IsHeroTrigger(DragHero hero)
    {
        if (hero.isUpHero) return;
        _nearestHeros.Clear();
        // �������и��ӵ���ɫ
        foreach (Transform box in curBoxs)
        {
            if (box.gameObject.activeSelf)
                box.GetComponent<Image>().color = Color.white;
        }
        // ���� pos �������Ӷ���
        foreach (Transform child in hero.pos)
        {
            Transform nearestBox = null;
            float minDistance = float.MaxValue;
            // �������и��ӣ����Ҿ���������Ǹ�
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
            // �������ĸ��Ӿ���������Χ�ڣ����Ǹø���
            if (nearestBox != null && minDistance < 4.5f && !_nearestHeros.Contains(nearestBox))
                _nearestHeros.Add(nearestBox);
        }
        foreach (Transform itmeBox in _nearestHeros)
            itmeBox.GetComponent<Image>().color = _nearestHeros.Count == hero.pos.childCount ? Color.green : Color.grey;

        //�ɿ���ק����
        if (hero.isUpBox)
        {
            UpHero(hero);
        }
    }
    /// <summary>
    /// �ɿ���ɫ
    /// </summary>
    /// <param name="hero"></param>
    private void UpHero(DragHero hero)
    {
        if (hero.type.isSyn)
        {
            //����ɿ��Ľ�ɫ�����ϳ���������ִ�к���
            foreach (var item in curHerosDict)
                item.Key.GetComponent<Image>().DOFade(0, mapMoveSpeed);
            return;
        }
        if (_nearestHeros.Count == hero.pos.childCount)
        {
            //���Է����ھ�����
            DragHero _draghreo = new DragHero();
            _draghreo.type = hero.type;
            //��ȡλ��ƫ��
            _draghreo.deviation = _nearestHeros[0].transform.position - hero.pos.GetChild(0).position;
            EventMgr.Instance.EventTrigger<DragHero>(E_EventType.placeMatrix, _draghreo);

            //��ȡ��λ�������еĽ�ɫ��Ϣ
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
                //�������ƶ�
                itemHero.DOLocalMove(pos, mapMoveSpeed)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        //���������Ľ�ɫ�Żر�����
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
            item.Key.GetComponent<Image>().DOFade(0, mapMoveSpeed);//������ĸ��� ����͸��
    }
    /// <summary>
    /// ��ɫ���󣬻򱻼������
    /// </summary>
    private void RemoveCurHerosDict(Transform heroTr)
    {
        List<Transform> removeHeros = new List<Transform>();
        //����һ������ʱ������ͬʱ������ɾ��
        foreach (var itemHero in curHerosDict)
        {
            if (itemHero.Value == heroTr && !removeHeros.Contains(itemHero.Key))
                removeHeros.Add(itemHero.Key);
        }
        //�ռ���Ҫɾ����key���������ͬһɾ��
        foreach (var item in removeHeros)
        {
            curHerosDict.Remove(item);
        }
    }

    List<Transform> _nearestBoxs = new List<Transform>();
    /// <summary>
    /// ��ק������
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
        //�ɿ�����
        if (dragBox.isUp)
            UpBox(dragBox);
    }
    /// <summary>
    /// �ɿ�����
    /// </summary>
    /// <param name="dragBox"></param>
    private void UpBox(DragBox dragBox)
    {
        //���뱸�����ص�
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
            if (posLegal)//�ж���Χ�Ƿ��м����Ӣ�۸���
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
    /// �ɿ����Ӻ����õ�ͼ�������ż�����λ��
    /// </summary>
    private void UpMapStatus()
    {
        foreach (Transform item in mapBase)
        {
            if (item != null)
                item.GetComponent<Image>().DOFade(0, mapMoveSpeed);
        }

        #region ���Ҽ�������--��ע��
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

        //��ȡ�ݺἤ������
        var allIndexY = string.Join("",
            Enumerable.Range(0, curBoxs.GetLength(1))
                .Where(j => j < 2 || j > 4)// �ų� 2 3 4��            
                .Where(j => Enumerable.Range(0, curBoxs.GetLength(0))  
                .Any(i => curBoxs[i, j].gameObject.activeSelf))// �ж�����������κμ���
                .Select(j => j.ToString()));//ƴ���ַ���

        var allIndexX = string.Join("",
            Enumerable.Range(0, curBoxs.GetLength(0))
                .Where(i => i < 1 || i > 3)
                .Where(i => Enumerable.Range(0, curBoxs.GetLength(1))
                .Any(j => curBoxs[i, j].gameObject.activeSelf))
                .Select(i => i.ToString()));

        //��ȡ��������
        var (posY, scaleY) = mapYDict.TryGetValue(allIndexY, out var valY) ? valY : mapYDict[""];
        var (posX, scaleX) = mapXDict.TryGetValue(allIndexX, out var valX) ? valX : mapXDict[""];
        boxBase.DOKill();
        mapBG.DOKill();
        boxBase.DOLocalMove(new Vector3(posX, posY), mapMoveSpeed).SetEase(Ease.OutQuad);
        mapBG.DOScale(new Vector3(scaleX, scaleY), mapMoveSpeed).SetEase(Ease.OutQuad);
    }
    /// <summary>
    /// ��ʼ��ק����
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
    /// ��ȡ����
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
        return new Vector2Int(-1, -1);  // δ�ҵ�
    }

    /// <summary>
    /// ��ȡ��Χ�Ƿ��м����Ӣ�۸��ӣ��жϸ�λ���Ƿ���Է��ø���
    /// </summary>
    private bool GetCurBoxsActive(Vector2Int index)
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0),  // ��
            new Vector2Int(-1, 0), // ��
            new Vector2Int(0, 1),  // ��
            new Vector2Int(0, -1)  // ��
        };

        foreach (var dir in directions)
        {
            // ����Ŀ������
            int x = index.x + dir.x;
            int y = index.y + dir.y;
            // �߽���
            if (x >= 0 && x < curBoxs.GetLength(0) && y >= 0 && y < curBoxs.GetLength(1))
            {
                if (curBoxs[x, y].gameObject.activeSelf)
                    return true;
            }
        }
        return false;
    }


}
