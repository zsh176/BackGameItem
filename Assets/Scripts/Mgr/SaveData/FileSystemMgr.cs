using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileSystemMgr : InstanceMgr<FileSystemMgr>
{
    public PlayerData playerData;
    public HeroData heroData;
    public LevelGameData levelGameData;
    public EnemyData enemyData;

    //构造函数 初始化
    public FileSystemMgr()
    {
        //数据只需读取一次 后续有修改都会存在单利中，在退出前保存数据即可
        playerData = JsonMgr.Instance.LoadData<PlayerData>();
        heroData = new HeroData();
        levelGameData = new LevelGameData();
        enemyData = new EnemyData();
    }

    /// <summary>
    /// 保存数据
    /// </summary>
    public void SavePlayerData()
    {
        JsonMgr.Instance.SaveData<PlayerData>(playerData);
    }
}
