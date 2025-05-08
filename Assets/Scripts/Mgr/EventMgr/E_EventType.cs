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
    enemyDeath,//敌人死亡
    setPlayGame,//改变游戏状态，开始游戏或进入准备阶段
    skipLevel,//跳过当前波数
    playerBeAtk,//玩家被攻击
    chaEnemyList,//更新敌人数组
    addHP,//食堂阿姨攻击，更加血量
    upHero10006,//上阵中的食堂阿姨合成升级
    addBulletAll,//记录大鹅，在过关时统一回收
    removeBulletAll,//大鹅被回收
}
