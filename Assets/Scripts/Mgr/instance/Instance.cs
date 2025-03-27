using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 未继承MonoBehaviour单利写法
/// </summary>
public class InstanceMgr<T> where T : new()
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
            }
            return instance;
        }
    }

}


/// <summary>
/// 继承MonoBehaviour单利写法
/// </summary>
public class BaseManager<T>  : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get 
        { 
            if (instance == null)
            {
                //创建一个空的游戏物体
                GameObject obj = new GameObject();
                //物体名设置成类名
                obj.name = typeof(T).ToString();
                //因为游戏物体是挂载在场景上的 所以要让它过场景时不被删除
                //因为单利模式对像 往往是存在整个生命周期的
                DontDestroyOnLoad(obj);
                //再给空物体挂载上脚本 赋值给instance
                instance = obj.AddComponent<T>();
            }
            return instance; 
        }
    }
}