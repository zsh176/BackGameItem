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
    public PoolMgr()
    {
        poolObj = new GameObject("Pool", typeof(RectTransform));
        poolObj.transform.SetParent(UIManager.Instance.GetLayer(E_UI_Layer.Bot), false);
        //instObj = new GameObject("Inst");
    }

    //缓存池容器
    public Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();
    //放在缓存池里时的父物体
    private GameObject poolObj;
    //实例化在场景上时的父物体
    //private GameObject instObj;

    /// <summary>
    /// 获取缓存池资源
    /// </summary>
    /// <param name="name">资源名</param>
    /// <param name="callBack">传入参数是实例，在回调中不要重复生成</param>
    public void GetObj(string name, UnityAction<GameObject> callBack)
    {
        //有对应的键(抽屉) 并且抽屉里面有东西
        if (poolDic.ContainsKey(name) && poolDic[name].poolList.Count > 0)
        {
            callBack(poolDic[name].GetObj());
        }
        else
        {
            //通过异步加载资源 创建对象给外部用
            AddressablesMgr.Instance.LoadAssetAsync<GameObject>(obj =>
            {
                //实例化完再传出去
                GameObject inst = GameObject.Instantiate(obj);
                inst.name = name;
                //inst.transform.parent = instObj.transform;
                callBack(inst);
            }, name);
        }
    }
    public void GetObj(UnityAction<GameObject> callBack, params string[] keys)
    {
        if (poolDic.ContainsKey(keys[0]) && poolDic[keys[0]].poolList.Count > 0)
        {
            callBack(poolDic[keys[0]].GetObj());
        }
        else
        {
            AddressablesMgr.Instance.LoadAssetAsync<GameObject>(obj =>
            {
                GameObject inst = GameObject.Instantiate(obj);
                inst.name = keys[0];
                //inst.transform.parent = instObj.transform;
                callBack(inst);
            }, keys);
        }
    }

    /// <summary>
    /// 回收资源
    /// </summary>
    /// <param name="obj">需要回收的物体</param>
    public void PushObj(GameObject obj)
    {
        string name = obj.name;
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
    //public Transform fatherObj;
    //对象的抽屉
    public List<GameObject> poolList;

    public PoolData(GameObject obj, GameObject poolObj)
    {
        //给我们的抽屉 创建一个父对象 并且把他作为pool的子对象
        fatherObj = new GameObject(obj.name, typeof(RectTransform));

        fatherObj.transform.SetParent(poolObj.transform, false);

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
        obj.transform.SetParent(fatherObj.transform, false);
    }

    //从抽屉里拿东西
    public GameObject GetObj(Transform parent = null)
    {
        GameObject obj = null;
        //取出第一个
        obj = poolList[0];
        //拿出去用了 就要字典里删除
        poolList.RemoveAt(0);
        //激活 让其显示
        obj.SetActive(true);
        ////断开父子关系
        //obj.transform.parent = null;
        //设置显示时的父物体
        //obj.transform.parent = parent;
        return obj;
    }
}


