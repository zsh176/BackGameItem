using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 缓存池模块
/// </summary>
public class PoolMgr : InstanceMgr<PoolMgr>
{
    //缓存池容器 (衣柜)
    public Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();
    //放在衣柜里暂时不用的东西的父物体
    private GameObject poolObj;


    /// <summary>
    /// 加载资源
    /// </summary>
    /// <param name="name">资源路径</param>
    /// <param name="pos">生成位置</param>
    /// <param name="callBack">生成后想做的事情</param>
    public void GetObj(string name, Vector3 pos, UnityAction<GameObject> callBack)
    {
        //有对应的键(抽屉) 并且抽屉里面有东西
        if (poolDic.ContainsKey(name) && poolDic[name].poolList.Count > 0)
        {
            callBack(poolDic[name].GetObj(pos));
        }
        else
        {
            //通过异步加载资源 创建对象给外部用
            ResMgr.Instance.LoadAsync<GameObject>(name, (o) =>
            {
                //资源加载完成后 想做的事情
                o.name = name;
                callBack(o);
            });
            //定义一个规则，生成物体的名字 就是池子的名字
        }
    }

    /// <summary>
    /// 回收资源
    /// </summary>
    /// <param name="name">定义一个规则，池子的名字 就是物体的名字</param>
    /// <param name="obj"></param>
    public void PushObj(string name, GameObject obj)
    {
        if (poolObj == null)
        {
            poolObj = new GameObject("Pool");
        }

        //里面有抽屉
        if (poolDic.ContainsKey(name))
        {
            //放进对应的抽屉
            poolDic[name].PushObj(obj);
        }
        //里面没有抽屉
        else
        {
            //创建一个抽屉 并且把东西放进去
            poolDic.Add(name, new PoolData(obj, poolObj));
        }
    }

    /// <summary>
    /// 清空缓存池的方法 主要用于切换场景时
    /// </summary>
    public void Clear() 
    { 
        //清空字典
        poolDic.Clear();
        poolObj = null;
    }
}

/// <summary>
/// 抽屉数据 池子中的一列容器
/// </summary>
public class PoolData
{
    //抽屉中 对象挂载的父节点
    public GameObject fatherObj;
    //对象的抽屉
    public List<GameObject> poolList;

    public PoolData(GameObject obj, GameObject poolObj)
    {
        //给我们的抽屉 创建一个父对象 并且把他作为pool的子对象
        fatherObj = new GameObject(obj.name);
        fatherObj.transform.parent = poolObj.transform;

        //创建一个抽屉 并且把东西放进去
        poolList = new List<GameObject>() { };
        PushObj(obj);
    }

    //往抽屉里放暂时不用的东西
    public void PushObj(GameObject obj)
    {
        //失活 让其隐藏
        obj.SetActive(false);
        //添加进列表
        poolList.Add(obj);
        //设置父对象
        obj.transform.parent = fatherObj.transform;
    }

    //从抽屉里拿东西
    public GameObject GetObj(Vector3 pos)
    {
        GameObject obj = null;
        //取出第一个
        obj = poolList[0];
        //拿出去用了 就要字典里删除
        poolList.RemoveAt(0);
        //生成位置
        obj.transform.position = pos;

        //激活 让其显示
        obj.SetActive(true);
        //断开父子关系
        obj.transform.parent = null;
        return obj;
    }
}
