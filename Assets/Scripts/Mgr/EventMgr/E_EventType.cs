using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件标签，有新的事件需添加一个新的标签
/// </summary>
public enum E_EventType
{
    _null,
    dragHero,//拖拽角色
    placeMatrix,//角色成功放在矩阵上
    placeHeroBox,//点击或松开 角色或格子
    dragBox,//拖拽格子
    startDragBox,//按下 开始拖拽格子
    fieldHeroDrag,//上阵中的角色被拖拽
}
