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
    public List<Transform> heroBoxs;//���б�������ɫ��Ϣ
    private List<Transform> fieldHeros;//���������н�ɫ��Ϣ
    private Transform heroBoxBase;//��ɫ������
    private Transform fieldHero;//��ɫ������
    private float transitionDuration = 0.3f;//��������ʱ��

    //��ʼ��
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
    /// ��ק���ɿ� ��ɫ�������
    /// </summary>
    private void PlaceHeroBox(UpHeroOrBox uphoero)
    {
        switch (uphoero.e_touchState)
        {
            case E_TouchState.Down://��ʼ��ק
                uphoero.type.SetParent(transform, true);
                uphoero.type.SetAsLastSibling();
                uphoero.type.DORotate(new Vector3(0, 0, 0), transitionDuration);
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
    public void CalculateLayout(int addHeroIndex = -1,bool isRotation = false)
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
        // ������ÿ������λ�ã�ͳһ�ƶ�
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
                sequence.Append(rt.DOLocalMove(targetPositions[i], transitionDuration).SetEase(Ease.OutQuad));
                sequence.Join(rt.DORotate(new Vector3(0, 0, UnityEngine.Random.value > 0.5f ? 13 : -13), transitionDuration));
            }
            else
            {
                rt.DOLocalMove(targetPositions[i], transitionDuration)
                .SetEase(Ease.OutQuad); // ʹ���ȿ�����Ļ�������
            }
        }
    }
}
