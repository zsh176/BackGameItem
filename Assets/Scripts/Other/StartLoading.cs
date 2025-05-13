using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 游戏启动脚本，加载中第一个面板
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
                print("显示GamePanel面板完成");
            });
        };
    }

}
