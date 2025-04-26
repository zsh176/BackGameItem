using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 游戏启动显示第一个面板
/// </summary>
public class StartInitPanel : MonoBehaviour
{
    void Start()
    {
        UIManager.Instance.startinitPanel += () =>
        {
            UIManager.Instance.ShowPanel<GamePanel>(callBack:transform =>
            {
                print("显示GamePanel面板完成");
            });
        };
    }

}
