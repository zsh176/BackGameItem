using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileSystemMgr : InstanceMgr<FileSystemMgr>
{
    public PlayerData playerData;
    public HeroData heroData;
    public LevelGameData levelGameData;
    public EnemyData enemyData;

    //���캯�� ��ʼ��
    public FileSystemMgr()
    {
        //����ֻ���ȡһ�� �������޸Ķ�����ڵ����У����˳�ǰ�������ݼ���
        playerData = JsonMgr.Instance.LoadData<PlayerData>();
        heroData = new HeroData();
        levelGameData = new LevelGameData();
        enemyData = new EnemyData();
    }

    /// <summary>
    /// ��������
    /// </summary>
    public void SavePlayerData()
    {
        JsonMgr.Instance.SaveData<PlayerData>(playerData);
    }
}
