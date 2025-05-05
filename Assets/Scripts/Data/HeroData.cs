using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroData
{
    public HeroData()
    {
        typeData = new Dictionary<HeroType, HeroDataInfo>()
        {
            { HeroType.Hero_10002,
                new HeroDataInfo
                {
                    levelSpiteName = "dz_1_",
                    atkValue = 4,
                    atkCooling = 0.5f,
                    atkDistance = 950,
                }
            },
            { HeroType.Hero_10004,
                new HeroDataInfo
                {
                    levelSpiteName = "dz_2_",
                    atkValue = 10,
                    atkCooling = 1.55f,
                    atkDistance = 1200,
                }
            },
            { HeroType.Hero_10005,
                new HeroDataInfo
                {
                    levelSpiteName = "dz_3_",
                    atkValue = 12,
                    atkCooling = 2f,
                    atkDistance = 950,
                }
            },
            { HeroType.Hero_10006,
                new HeroDataInfo
                {
                    levelSpiteName = "dz_2_",
                    atkValue = 4,
                    atkCooling = 1.6f,
                    atkDistance = 0,
                }
            },
            { HeroType.Hero_10014,
                new HeroDataInfo
                {
                    levelSpiteName = "dz_2_",
                    atkValue = 6,
                    atkCooling = 2.5f,
                    atkDistance = 950,
                }
            },
            { HeroType.Hero_10015,
                new HeroDataInfo
                {
                    levelSpiteName = "",
                    atkValue = 18,
                    atkCooling = 2.3f,
                    atkDistance = 1050,
                }
            }
        };
    }
    public Dictionary<HeroType, HeroDataInfo> typeData;
}

/// <summary>
/// ���ý�ɫ��Ϣ
/// </summary>
public class HeroDataInfo
{
    /// <summary>
    /// �ȼ���ͼ��ͼƬ
    /// </summary>
    public string levelSpiteName;
    /// <summary>
    /// ������
    /// </summary>
    public int atkValue;
    /// <summary>
    /// �������
    /// </summary>
    public float atkCooling;
    /// <summary>
    /// ��������
    /// </summary>
    public int atkDistance;
}