using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

/// <summary>
/// ������ý�ɫ�ĵ�ͼ����
/// </summary>
public class Matrix_MapBox : MonoBehaviour
{
    private Transform curBoxsBase;//��ɫ�ɷ��ø��Ӿ�������
    private Transform mapBoxBase;//��ͼ�ɷ��ø��Ӿ�������

    /// <summary>
    /// �洢���з��ø��ӵ���Ϣ
    /// </summary>
    private Transform[,] curBoxs;
    private Transform[,] mapBoxs;
    void Start()
    {
        curBoxsBase = transform.Find("BattleMap/Matrix_MapBox/Current_Box").transform;
        mapBoxBase = transform.Find("BattleMap/Matrix_MapBox/_AddBox").transform;
        curBoxs = new Transform[5, 7];
        int x = 0;
        int y = 0;
        foreach (Transform item in curBoxsBase)
        {
            curBoxs[x, y] = item.GetComponent<Transform>();
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
        foreach (Transform item in mapBoxBase)
        {
            mapBoxs[x, y] = item.GetComponent<Transform>();
            //��ʼ����ͼ���ø�������
            if (y >= 2 && y <= 4 && x != 0 && x != 4)
                item.gameObject.SetActive(false);

            if (++x == 5)
            {
                x = 0;
                y++;
            }
        }

        EventMgr.Instance.AddEventListener<DragHero>(E_EventType.dragHero, IsHeroTrigger);
        EventMgr.Instance.AddEventListener<DragBox>(E_EventType.dragBox, IsBoxTrigger);
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
            Transform childRect = child.GetComponent<Transform>();
            Transform nearestBox = null;
            float minDistance = float.MaxValue;
            // �������и��ӣ����Ҿ���������Ǹ�
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
            // �������ĸ��Ӿ���������Χ�ڣ����Ǹø���
            if (nearestBox != null && minDistance < 4.5f && !_nearestHeros.Contains(nearestBox))
                _nearestHeros.Add(nearestBox);
        }

        foreach(Transform itmeBox in _nearestHeros)
            itmeBox.GetComponent<Image>().color = _nearestHeros.Count == hero.pos.childCount ? Color.green : Color.grey;

        //�ɿ���ק����
        if (hero.isUpBox)
        {
            if (_nearestHeros.Count == hero.pos.childCount)
            {
                //���Է����ھ�����
                DragHero _draghreo = new DragHero();
                _draghreo.type = hero.type;
                //��ȡλ��ƫ��
                _draghreo.deviation = _nearestHeros[0].transform.position - hero.pos.GetChild(0).position;
                EventMgr.Instance.EventTrigger<DragHero>(E_EventType.placeMatrix, _draghreo);
                foreach (var item in _nearestHeros)
                    item.GetComponent<Image>().color = Color.clear;//����͸��
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
    /// ��ק���Ӻ��ɿ����Ӵ���
    /// </summary>
    private void IsBoxTrigger(DragBox dragBox)
    {
        _nearestBoxs.Clear();
        foreach (Transform box in mapBoxs)
        {
            if (box.gameObject.activeSelf)
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
            if (nearestBox != null && minDistance < 4.5f && !_nearestBoxs.Contains(nearestBox))
                _nearestBoxs.Add(nearestBox);
        }
        foreach (Transform itmeBox in _nearestBoxs)
        {
            itmeBox.GetChild(0).gameObject.SetActive(true);
            if (_nearestBoxs.Count != dragBox.pos.childCount)
                itmeBox.GetChild(0).GetComponent<Image>().color = Color.grey;
        }
        //�����ɿ����Ӳ���
        if (dragBox.isUp)
        {
            if (_nearestBoxs.Count == dragBox.pos.childCount)
            {
                Vector3 movePos = _nearestBoxs[0].transform.position - dragBox.pos.GetChild(0).position;
                movePos = dragBox.type.position + movePos;
                dragBox.type.DOKill();
                dragBox.type.DOMove(movePos, 0.25f).SetEase(Ease.OutBounce).OnComplete(() =>
                {
                    Destroy(dragBox.type.gameObject);
                    foreach (Transform item in _nearestBoxs)
                    {
                        item.gameObject.SetActive(false);
                        Vector2Int indexV2 = FindIndex(mapBoxs, item);
                        curBoxs[indexV2.x, indexV2.y].gameObject.SetActive(true);
                    }
                });
            }
            else
            {
                UpHeroOrBox upHero = new UpHeroOrBox();
                upHero.type = dragBox.type;
                upHero.e_touchState = E_TouchState.UpPlace;
                EventMgr.Instance.EventTrigger<UpHeroOrBox>(E_EventType.placeHeroBox, upHero);
                foreach (var item in _nearestBoxs)
                    item.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// ��ȡ����
    /// </summary>
    public static Vector2Int FindIndex(Transform[,] array, Transform target)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                if (array[i, j] == target)
                {
                    return new Vector2Int(i, j);
                }
            }
        }
        return new Vector2Int(-1, -1);  // δ�ҵ�
    }
}
