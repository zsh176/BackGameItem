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
                }
            },
            { HeroType.Hero_10004,
                new HeroDataInfo
                {
                    levelSpiteName = "dz_2_",
                }
            },
            { HeroType.Hero_10005,
                new HeroDataInfo
                {
                    levelSpiteName = "dz_3_",
                }
            },
            { HeroType.Hero_10006,
                new HeroDataInfo
                {
                    levelSpiteName = "dz_2_",
                }
            },
            { HeroType.Hero_100014,
                new HeroDataInfo
                {
                    levelSpiteName = "dz_2_",
                }
            },
            { HeroType.Hero_100015,
                new HeroDataInfo
                {
                    levelSpiteName = "",
                }
            }
        };
    }
    public Dictionary<HeroType, HeroDataInfo> typeData;
}

public class HeroDataInfo
{
    /// <summary>
    /// µÈ¼¶µ×Í¼¿òÍ¼Æ¬
    /// </summary>
    public string levelSpiteName;
}