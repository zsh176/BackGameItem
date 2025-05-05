using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData
{
    public EnemyData()
    {
        typeData = new Dictionary<EnemyType, EnemyDataInfo>()
        {   
            { EnemyType.Enemy_Ordinary01,
                new EnemyDataInfo
                {
                    moveSpeed = 50,
                    atkValue = 4,
                    atkCooling = 2,
                    maxHP = 25,
                }
            },
            { EnemyType.Enemy_Ordinary02,
                new EnemyDataInfo
                {
                    moveSpeed = 65,
                    atkValue = 5,
                    atkCooling = 3,
                    maxHP = 28,
                }
            },
            { EnemyType.Enemy_Ordinary03,
                new EnemyDataInfo
                {
                    moveSpeed = 70,
                    atkValue = 3,
                    atkCooling = 1.8f,
                    maxHP = 30,
                }
            },
            { EnemyType.Enemy_Ordinary04,
                new EnemyDataInfo
                {
                    moveSpeed = 57,
                    atkValue = 8,
                    atkCooling = 3.5f,
                    maxHP = 35,
                }
            },
            { EnemyType.Enemy_Ordinary05,
                new EnemyDataInfo
                {
                    moveSpeed = 85,
                    atkValue = 5,
                    atkCooling = 2.8f,
                    maxHP = 32,
                }
            },
            { EnemyType.Enemy_Boss01,
                new EnemyDataInfo
                {
                    moveSpeed = 40,
                    atkValue = 12,
                    atkCooling = 4f,
                    maxHP = 550,
                }
            },
            { EnemyType.Enemy_Boss02,
                new EnemyDataInfo
                {
                    moveSpeed = 70,
                    atkValue = 15,
                    atkCooling = 3.8f,
                    maxHP = 450,
                }
            },
        };
    }

    public Dictionary<EnemyType, EnemyDataInfo> typeData;
}

/// <summary>
/// 配置敌人信息
/// </summary>
public class EnemyDataInfo
{
    /// <summary>
    /// 移动速度
    /// </summary>
    public float moveSpeed;
    /// <summary>
    /// 攻击力
    /// </summary>
    public int atkValue;
    /// <summary>
    /// 攻击间隔
    /// </summary>
    public float atkCooling;
    /// <summary>
    /// 最大血量
    /// </summary>
    public int maxHP;
}
