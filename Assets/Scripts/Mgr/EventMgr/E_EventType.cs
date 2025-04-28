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
    placeHeroBox,//将角色放置备用区
    dragBox,//拖拽格子
    upBox,//松开格子
}
