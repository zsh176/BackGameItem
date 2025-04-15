using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �ɷ��ý�ɫ�ĵ�ͼ����
/// </summary>
public class Current_Box : MonoBehaviour
{
    /// <summary>
    /// �洢���з��ø��ӵ���Ϣ
    /// </summary>
    public RectTransform[,] curBoxs = new RectTransform[5, 7];
    void Start()
    {
        int x = 0;
        int y = 0;
        foreach (Transform item in transform)
        {
            curBoxs[x, y] = item.GetComponent<RectTransform>();
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

    //��ʱ�洢���Ϸ���λ�õĸ���
    List<RectTransform> _nearestBoxs = new List<RectTransform>();
    /// <summary>
    /// ��ק��ɫ �жϾ������
    /// </summary>
    private void IsHeroTrigger(DragHero hero)
    {
        _nearestBoxs.Clear();
        // �������и��ӵ���ɫ
        foreach (RectTransform box in curBoxs)
        {
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
                float dist = Vector2.Distance(box.position, childRect.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearestBox = box;
                }
            }
            // �������ĸ��Ӿ���������Χ�ڣ����Ǹø���
            if (nearestBox != null && minDistance < 0.65f && !_nearestBoxs.Contains(nearestBox))
            {
                nearestBox.GetComponent<Image>().color = Color.green;
                _nearestBoxs.Add(nearestBox);
            }
        }
       
        //�ɿ���ק����
        if (hero.isUp)
        {
            if (_nearestBoxs.Count != hero.pos.childCount) return;
            DragHero _draghreo = new DragHero();
            _draghreo.type = hero.type;
            //��ȡλ��ƫ��
            _draghreo.deviation = _nearestBoxs[0].transform.position - hero.pos.GetChild(0).position;
            EventMgr.Instance.EventTrigger<DragHero>(E_EventType.placeMatrix, _draghreo);
            foreach (var item in _nearestBoxs)
            {
                item.GetComponent<Image>().color = Color.clear;//����͸��
            }
        }
    }
}
