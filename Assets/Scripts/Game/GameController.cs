using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Spine.Unity.Editor.SkeletonBaker.BoneWeightContainer;

/// <summary>
/// 游戏流程控制器
/// </summary>
public class GameController : MonoBehaviour
{
  
    #region 配置随机概率
    // 创建概率字典（总概率100%） 权重(概率)可以修改，总权重可以超过100%
    Dictionary<HeroType, float> heroprobabilities = new Dictionary<HeroType, float>()
    {
        { HeroType.Hero_10002, 0.176f },
        { HeroType.Hero_10004, 0.176f },
        { HeroType.Hero_10005, 0.176f },
        { HeroType.Hero_10006, 0.176f },
        { HeroType.Hero_100014, 0.176f },
        { HeroType.Box_1, 0.07f },
        { HeroType.Box_2, 0.035f },
        { HeroType.Box_3, 0.015f }
    };
    Dictionary<int, float> levelprobabilities = new Dictionary<int, float>()
    {
        { 1,0.8f},
        { 2,0.15f},
        { 3,0},
        { 4,0},
    };
    List<HeroType> targetKeys = new List<HeroType>
    {
        HeroType.Hero_10002,
        HeroType.Hero_10004,
        HeroType.Hero_10005,
        HeroType.Hero_10006,
        HeroType.Hero_100014
    };
    #endregion

    private GamePanel gamePanel;
    [HideInInspector]
    public int playerLevel;//玩家等级


    private void Start()
    {
        gamePanel = GetComponent<GamePanel>();
        playerLevel = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerLevel++;
            Debug.Log("当前等级" + playerLevel);
        }
    }

    /// <summary>
    /// 获取3个角色
    /// </summary>
    public List<BackHeroData> GetRandomHeroes(bool isAdv = false)
    {
        List<BackHeroData> results = new List<BackHeroData>();

        float randomPoint = Range(0f, 1);

        for (int i = 0; i < 3; i++)
        {
            if (isAdv)//必出一个高级角色
            {   
                if (randomPoint < 0.334f)
                {
                    isAdv = false;
                    results.Add(new BackHeroData
                    {
                        type = targetKeys[Range(0, targetKeys.Count)],
                        level = Range(0f, 1) > 0.8f ? 4 : 3,
                    });
                    continue;
                }
                randomPoint -= 0.334f;
            }

            results.Add(new BackHeroData
            {
                type = GetHeroRandom(),
                level = GetLevelRandom()
            });
        }
        return results;
    }
    /// <summary>
    /// 随机获取一个角色
    /// </summary>
    private HeroType GetHeroRandom()
    {
        // 计算总权重
        float totalWeight = 0f;
        foreach (var pair in heroprobabilities)
            totalWeight += pair.Value;
        // 执行轮盘赌算法
        float randomPoint = Range(0f, totalWeight);
        foreach (var pair in heroprobabilities)
        {
            if (randomPoint < pair.Value)
            {
                return pair.Key;
            }
            randomPoint -= pair.Value;
        }
        // 保底返回第一个（理论上不会执行到这里）
        return HeroType.Hero_10002;
    }
    /// <summary>
    /// 随机获取等级
    /// </summary>
    private int GetLevelRandom()
    {
        Dictionary<int, float> dynamicWeights = new Dictionary<int, float>();
        //随着玩家等级提升，高等级卡牌概率增加
        foreach (var pair in levelprobabilities)
        {
            float weight = pair.Value;
            if (playerLevel > 1)
            {
                switch (pair.Key)
                {
                    case 2:
                        weight += (playerLevel - 1) * 0.125f;
                        break;
                    case 3:
                        weight += (playerLevel - 1) * 0.05f;
                        break;
                    case 4:
                        weight += (playerLevel - 1) * 0.016f;
                        break;
                }
            }
            dynamicWeights[pair.Key] = weight;
            //Debug.Log($"玩家等级{playerLevel}，等级{pair.Key}，概率{weight}");
        }
        float totalWeight = 0;
        foreach (var w in dynamicWeights.Values) 
            totalWeight += w;

        float randomPoint = Range(0f, totalWeight);
        foreach (var pair in dynamicWeights)
        {
            if (randomPoint < pair.Value)
                return pair.Key;
            randomPoint -= pair.Value;
        }
        return 1;
    }

    #region 封装一层随机数API
    private float Range(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }
    private int Range(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }
    #endregion
}

/// <summary>
/// 随机生成的角色数据
/// </summary>
public class BackHeroData
{
    public HeroType type;//角色标签
    public int level;//等级
    public bool isAdLock;//是否广告解锁
}
