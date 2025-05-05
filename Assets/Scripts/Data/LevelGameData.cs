using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// ��Ϸ�ؿ�����
/// </summary>
public class LevelGameData
{
    public LevelGameData()
    {
        allLevelData = new List<AwveData>
        {

        #region ��һ��
            new AwveData()//��һ��
            {
               batchDatas = new List<BatchData>
               {
                   new BatchData()//��һ��
                   {
                       Cooling = 0,
                       fewEnemy = 20,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary01,
                       }
                   },
                    new BatchData()//�ڶ���
                   {
                       Cooling = 7,
                       fewEnemy = 25,
                       enemyName = new EnemyType[]
                       {
                           EnemyType.Enemy_Ordinary02,
                       }
                   },
                    new BatchData()//������
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

        #region �ڶ���
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

        #region ������
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
            
        #region ���Ĳ�
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

        #region ���岨
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

        #region ������
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

        #region ���߲�
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

        #region �ڰ˲�
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

        #region �ھŲ�
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

        #region ��ʮ��
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
/// һ�������а���������
/// </summary>
public class AwveData
{
    /// <summary>
    /// һ�������а���������
    /// </summary>
    public List<BatchData> batchDatas;
}

/// <summary>
/// һ���а���������
/// </summary>
public class BatchData
{
    /// <summary>
    /// ÿ��֮��ļ��
    /// </summary>
    public float Cooling;
    /// <summary>
    /// �������ɼ�������
    /// </summary>
    public int fewEnemy;
    /// <summary>
    /// ����ʲô���͵ĵ���
    /// </summary>
    public EnemyType[] enemyName;
    /// <summary>
    /// �Ƿ���Boss����
    /// </summary>
    public bool isBoss;
    /// <summary>
    /// ���ɼ���boss
    /// </summary>
    public EnemyType[] bossName;
}
