using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �¼���ǩ�����µ��¼������һ���µı�ǩ
/// </summary>
public enum E_EventType
{
    _null,
    dragHero,//��ק��ɫ
    placeMatrix,//��ɫ�ɹ����ھ�����
    placeHeroBox,//������ɿ� ��ɫ�����
    dragBox,//��ק����
    startDragBox,//���� ��ʼ��ק����
    fieldHeroDrag,//�����еĽ�ɫ����ק
    enemyDeath,//��������
    setPlayGame,//�ı���Ϸ״̬����ʼ��Ϸ�����׼���׶�
    skipLevel,//������ǰ����
    playerBeAtk,//��ұ�����
    chaEnemyList,//���µ�������
    addHP,//ʳ�ð��̹���������Ѫ��
    upHero10006,//�����е�ʳ�ð��̺ϳ�����
    addBulletAll,//��¼��죬�ڹ���ʱͳһ����
    removeBulletAll,//��챻����
}
