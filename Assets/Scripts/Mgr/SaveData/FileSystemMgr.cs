using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileSystemMgr : InstanceMgr<FileSystemMgr>
{
    public PlayerData playerData;
    public HeroData heroData;

    //���캯�� ��ʼ��
    public FileSystemMgr()
    {
        //����ֻ���ȡһ�� �������޸Ķ�����ڵ����У����˳�ǰ�������ݼ���
        playerData = JsonMgr.Instance.LoadData<PlayerData>();
        heroData = JsonMgr.Instance.LoadData<HeroData>();
    }

    /// <summary>
    /// ��������
    /// </summary>
    public void SavePlayerData()
    {
        JsonMgr.Instance.SaveData<PlayerData>(playerData);
    }
}
