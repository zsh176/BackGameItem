using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ��Ϸ������ʾ��һ�����
/// </summary>
public class StartInitPanel : MonoBehaviour
{
    void Start()
    {
        UIManager.Instance.startinitPanel += () =>
        {
            UIManager.Instance.ShowPanel<GamePanel>(callBack:transform =>
            {
                print("��ʾGamePanel������");
            });
        };
    }

}
