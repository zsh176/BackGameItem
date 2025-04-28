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
    private RectTransform[,] curBoxs;
    private RectTransform[,] mapBoxs;
    void Start()
    {
        curBoxsBase = transform.Find("BattleMap/Matrix_MapBox/Current_Box").transform;
        mapBoxBase = transform.Find("BattleMap/Matrix_MapBox/_AddBox").transform;
        curBoxs = new RectTransform[5, 7];
        int x = 0;
        int y = 0;
        foreach (Transform item in curBoxsBase)
        {
            curBoxs[x, y] = item.GetComponent<RectTransform>();
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
        mapBoxs = new RectTransform[5, 7];
        x = 0;
        y = 0;
        foreach (Transform item in mapBoxBase)
        {
            mapBoxs[x, y] = item.GetComponent<RectTransform>();
            //��ʼ����ͼ���ø�������
            //if (!(y <= 1) || !(y >= 5))
            //    item.gameObject.SetActive(false);
            //else if (!(x == 0) || !(x == 4))
            //    item.gameObject.SetActive(false);

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
    List<RectTransform> _nearestHeros = new List<RectTransform>();
    /// <summary>
    /// ��ק��ɫ �жϾ������
    /// </summary>
    private void IsHeroTrigger(DragHero hero)
    {
        if (hero.isUpHero) return;
        _nearestHeros.Clear();
        // �������и��ӵ���ɫ
        foreach (RectTransform box in curBoxs)
        {
            if (box.gameObject.activeSelf)
                box.GetComponent<Image>().color = Color.white;
        }
        // ���� pos �������Ӷ���
        foreach (Transform child in hero.pos)
        {
            RectTransform childRect = child.GetComponent<RectTransform>();
            RectTransform nearestBox = null;
            float minDistance = float.MaxValue;
            // �������и��ӣ����Ҿ���������Ǹ�
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
            // �������ĸ��Ӿ���������Χ�ڣ����Ǹø���
            if (nearestBox != null && minDistance < 6.5f && !_nearestHeros.Contains(nearestBox))
                _nearestHeros.Add(nearestBox);
        }

        foreach(RectTransform itmeBox in _nearestHeros)
        {
            itmeBox.GetComponent<Image>().color = _nearestHeros.Count == hero.pos.childCount ? Color.green : Color.grey;
        }

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
                {
                    item.GetComponent<Image>().color = Color.clear;//����͸��
                }
            }
            else
            {
                hero.UpHeroOnCilck?.Invoke();
                foreach (var item in _nearestHeros)
                {
                    item.GetComponent<Image>().color = Color.white;
                }
            }

        }
    }

    //��ʱ�洢���Ϸ���λ�õĸ���
    List<RectTransform> _nearestBoxs = new List<RectTransform>();
    /// <summary>
    /// ��ק���Ӻ��ɿ����Ӵ���
    /// </summary>
    private void IsBoxTrigger(DragBox dragBox)
    {
        _nearestBoxs.Clear();
        // �������и��ӵ���ɫ
        foreach (RectTransform box in mapBoxs)
        {
            if (box.gameObject.activeSelf)
                box.GetComponent<Image>().color = Color.white;
        }

        foreach (Transform child in dragBox.pos)
        {
            RectTransform childRect = child.GetComponent<RectTransform>();
            RectTransform nearestBox = null;
            float minDistance = float.MaxValue;
            foreach (RectTransform box in mapBoxs)
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
            if (nearestBox != null && minDistance < 6.5f && !_nearestBoxs.Contains(nearestBox))
                _nearestBoxs.Add(nearestBox);
        }

        foreach (RectTransform itmeBox in _nearestBoxs)
            itmeBox.GetComponent<Image>().color = _nearestBoxs.Count == dragBox.pos.childCount ? Color.green : Color.grey;

        if (dragBox.isUp)
        {
            DragBox _draghreo = new DragBox();
            _draghreo.type = dragBox.type;
            if (_nearestBoxs.Count == dragBox.pos.childCount)
            {
                //���Է���
                _draghreo.isWin= true;
                _draghreo.deviation = _nearestBoxs[0].transform.position - dragBox.pos.GetChild(0).position;
                foreach (var item in _nearestBoxs)
                    item.gameObject.SetActive(false);
            }
            else
            {
                _draghreo.isWin = false;
                foreach (var item in _nearestBoxs)
                    item.GetComponent<Image>().color = Color.white;
            }
            EventMgr.Instance.EventTrigger<DragBox>(E_EventType.upBox, _draghreo);
        }
    }
}
