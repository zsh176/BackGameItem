using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ��Ϸ�����ű��������е�һ�����
/// </summary>
public class StartLoading : MonoBehaviour
{
    void Start()
    {
        UIManager.Instance.startinitPanel += () =>
        {
            UIManager.Instance.ShowPanel<GamePanel>(callBack:transform =>
            {
                gameObject.SetActive(false);
                print("��ʾGamePanel������");
            });
        };
    }

}
