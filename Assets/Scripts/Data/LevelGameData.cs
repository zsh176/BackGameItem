using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 游戏关卡数据
/// </summary>
public class LevelGameData
{
    public LevelGameData()
    {
        allLevelData = new List<AwveData>
        {

        #region 第一波
            new AwveData()//第一波
            {
               batchDatas = new List<BatchData>
               {
                   new BatchData()//第一批
                   {
                       Cooling = 0,
                       fewEnemy = 20,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary01,
                       }
                   },
                    new BatchData()//第二批
                   {
                       Cooling = 7,
                       fewEnemy = 25,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary02,
                       }
                   },
                    new BatchData()//第三批
                   {
                       Cooling = 10,
                       fewEnemy = 30,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary01,
                           EnemyType.Enemy_Ordinary02,
                       }
                   }
               }
            },
	#endregion

        #region 第二波
            new AwveData()
            {
               batchDatas = new List<BatchData>
               {
                   new BatchData()
                   {
                       Cooling = 0,
                       fewEnemy = 20,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary03,
                       }
                   },
                    new BatchData()
                   {
                       Cooling = 7,
                       fewEnemy = 25,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary02,
                       }
                   },
                    new BatchData()
                   {
                       Cooling = 10,
                       fewEnemy = 30,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary01,
                           EnemyType.Enemy_Ordinary03,
                       }
                   }
               }
            },
	#endregion

        #region 第三波
            new AwveData()
            {
              
               batchDatas = new List<BatchData>
               {
                   new BatchData()
                   {
                       Cooling = 0,
                       fewEnemy = 20,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary02,
                       }
                   },
                    new BatchData()
                   {
                       Cooling = 7,
                       fewEnemy = 25,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary03,
                       }
                   },
                    new BatchData()
                   {
                       Cooling = 10,
                       fewEnemy = 30,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary01,
                           EnemyType.Enemy_Ordinary03,
                       },
                       isBoss = true,
                       bossName = new EnemyType[]
                       {
                           EnemyType.Enemy_Boss01
                       },
                   }
               }
            },
	#endregion
            
        #region 第四波
            new AwveData()
            {
               batchDatas = new List<BatchData>
               {
                   new BatchData()
                   {
                       Cooling = 0,
                       fewEnemy = 30,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary04,
                       }
                   },
                    new BatchData()
                   {
                       Cooling = 7,
                       fewEnemy = 40,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary03,
                       }
                   },
                    new BatchData()
                   {
                       Cooling = 10,
                       fewEnemy = 45,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary01,
                           EnemyType.Enemy_Ordinary04,
                       }
                   }
               }
            },
	#endregion

        #region 第五波
            new AwveData()
            {
               batchDatas = new List<BatchData>
               {
                   new BatchData()
                   {
                       Cooling = 0,
                       fewEnemy = 30,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary02,
                       }
                   },
                    new BatchData()
                   {
                       Cooling = 7,
                       fewEnemy = 40,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary03,
                            EnemyType.Enemy_Ordinary01,
                       }
                   },
                    new BatchData()
                   {
                       Cooling = 10,
                       fewEnemy = 45,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary01,
                           EnemyType.Enemy_Ordinary05,
                       }
                   }
               }
            },
	#endregion

        #region 第六波
            new AwveData()
            {
               batchDatas = new List<BatchData>
               {
                   new BatchData()
                   {
                       Cooling = 0,
                       fewEnemy = 30,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary05,
                       }
                   },
                    new BatchData()
                   {
                       Cooling = 7,
                       fewEnemy = 40,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary04,
                           EnemyType.Enemy_Ordinary02,
                       },
                       isBoss = true,
                       bossName = new EnemyType[]
                       {
                           EnemyType.Enemy_Boss01,
                       }
                   },
                    new BatchData()
                   {
                       Cooling = 10,
                       fewEnemy = 45,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary03,
                           EnemyType.Enemy_Ordinary04,
                           EnemyType.Enemy_Ordinary05,
                       },
                       isBoss = true,
                       bossName = new EnemyType[]
                       {
                           EnemyType.Enemy_Boss02,
                       }
                   }
               }
            },
	#endregion

        #region 第七波
            new AwveData()
            {
               batchDatas = new List<BatchData>
               {
                   new BatchData()
                   {
                       Cooling = 0,
                       fewEnemy = 35,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary01,
                           EnemyType.Enemy_Ordinary02,
                       }
                   },
                    new BatchData()
                   {
                       Cooling = 7,
                       fewEnemy = 45,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary03,
                           EnemyType.Enemy_Ordinary05,
                       }
                   },
                    new BatchData()
                   {
                       Cooling = 10,
                       fewEnemy = 50,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary04,
                           EnemyType.Enemy_Ordinary05,
                       }
                   }
               }
            },
	#endregion

        #region 第八波
            new AwveData()
            {
               batchDatas = new List<BatchData>
               {
                   new BatchData()
                   {
                       Cooling = 0,
                       fewEnemy = 35,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary05,
                       }
                   },
                    new BatchData()
                   {
                       Cooling = 7,
                       fewEnemy = 45,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary04,
                       }
                   },
                    new BatchData()
                   {
                       Cooling = 10,
                       fewEnemy = 50,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary02,
                           EnemyType.Enemy_Ordinary04,
                           EnemyType.Enemy_Ordinary05,
                       }
                   }
               }
            },
	#endregion

        #region 第九波
            new AwveData()
            {
               batchDatas = new List<BatchData>
               {
                   new BatchData()
                   {
                       Cooling = 0,
                       fewEnemy = 35,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary01,
                           EnemyType.Enemy_Ordinary02,
                       }
                   },
                    new BatchData()
                   {
                       Cooling = 7,
                       fewEnemy = 45,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary05,
                           EnemyType.Enemy_Ordinary02,
                       }
                   },
                    new BatchData()
                   {
                       Cooling = 10,
                       fewEnemy = 50,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary03,
                           EnemyType.Enemy_Ordinary02,
                           EnemyType.Enemy_Ordinary04,
                       }
                   }
               }
            },
	#endregion

        #region 第十波
            new AwveData()
            {
               batchDatas = new List<BatchData>
               {
                   new BatchData()
                   {
                       Cooling = 0,
                       fewEnemy = 35,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary02,
                           EnemyType.Enemy_Ordinary03,
                           EnemyType.Enemy_Ordinary04,
                       },
                       isBoss = true,
                       bossName = new EnemyType[]
                       {
                           EnemyType.Enemy_Boss01,
                       },
                   },
                    new BatchData()
                   {
                       Cooling = 8,
                       fewEnemy = 45,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary02,
                           EnemyType.Enemy_Ordinary01,
                           EnemyType.Enemy_Ordinary05,
                       },
                       isBoss = true,
                       bossName = new EnemyType[]
                       {
                           EnemyType.Enemy_Boss02,
                       },
                   },
                    new BatchData()
                   {
                       Cooling = 11,
                       fewEnemy = 50,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary02,
                           EnemyType.Enemy_Ordinary03,
                           EnemyType.Enemy_Ordinary04,
                           EnemyType.Enemy_Ordinary05,
                       },
                       isBoss = true,
                       bossName = new EnemyType[]
                       {
                           EnemyType.Enemy_Boss01,
                           EnemyType.Enemy_Boss02,
                       },
                   }
               }
            },
	#endregion

        };

    }

    public List<AwveData> allLevelData;
}

/// <summary>
/// 一个波次中包含的数据
/// </summary>
public class AwveData
{
    /// <summary>
    /// 一个批次中包含的数据
    /// </summary>
    public List<BatchData> batchDatas;
}

/// <summary>
/// 一批中包含的数据
/// </summary>
public class BatchData
{
    /// <summary>
    /// 每批之间的间隔
    /// </summary>
    public float Cooling;
    /// <summary>
    /// 这批生成几个敌人
    /// </summary>
    public int fewEnemy;
    /// <summary>
    /// 生成什么类型的敌人
    /// </summary>
    public EnemyType[] enemyName;
    /// <summary>
    /// 是否是Boss批次
    /// </summary>
    public bool isBoss;
    /// <summary>
    /// 生成几个boss
    /// </summary>
    public EnemyType[] bossName;
}
