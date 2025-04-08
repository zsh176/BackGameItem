using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    //如果读取数据是空的，就在此初始化
   public PlayerData()
    {
        name = "小胖云";
        coin = 100;
    }
    public string name;
    public int coin;
}
