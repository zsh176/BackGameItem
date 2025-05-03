using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

//可寻址资源 信息
public class AddressablesInfo
{
    //记录 异步操作句柄
    public AsyncOperationHandle handle;
    /// <summary>
    /// 引用计数器
    /// </summary>
    public uint count;

    public AddressablesInfo(AsyncOperationHandle handle)
    {
        this.handle = handle;
        count += 1;
    }
}

public class AddressablesMgr : InstanceMgr<AddressablesMgr>
{
    //存储所有已加载的资源 避免重复加载相同资源
    public Dictionary<string, AddressablesInfo> resDic = new Dictionary<string, AddressablesInfo>();


    /// <summary>
    /// 异步加载资源的
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="name">资源名</param>
    /// <param name="callBack">加载完成回调</param>
    public void LoadAssetAsync<T>(string name, Action<T> callBack)
    {
        //构建唯一keyName 用于记录存在在字典中的对象
        string keyName = name + "_" + typeof(T).Name;
        AsyncOperationHandle<T> handle;

        //如果已经加载过该资源
        if (resDic.ContainsKey(keyName))
        {
            handle = resDic[keyName].handle.Convert<T>();
            //要使用资源了 引用计数+1
            resDic[keyName].count += 1;
            if (handle.IsDone)
            {
                //异步加载结束 获取已加载好的资源 直接相当于同步调用了
                callBack(handle.Result);
            }
            else
            {
                //还没有加载完成 添加回调
                handle.Completed += (obj) => {
                    if (obj.Status == AsyncOperationStatus.Succeeded)
                        callBack(obj.Result);
                };
            }
            return;
        }

        //没有加载过该资源 进行异步加载 并且记录
        handle = Addressables.LoadAssetAsync<T>(name);
        handle.Completed += (obj) => {
            if (obj.Status == AsyncOperationStatus.Succeeded)
                callBack(obj.Result);
            else
            {
                Debug.LogWarning(keyName + "资源加载失败");
                if (resDic.ContainsKey(keyName))
                    resDic.Remove(keyName);
            }
        };
        AddressablesInfo info = new AddressablesInfo(handle);
        resDic.Add(keyName, info);
    }

    /// <summary>
    /// 异步加载多个标签资源，可变长参数只能声明在最后一个参数，所以先传回调，再传标签
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="callBack">加载完成回调</param>
    /// <param name="keys">资源名 + 标签。可变长参数，可传多个参数(标签)</param>
    public void LoadAssetAsync<T>(Action<T> callBack, params string[] keys)
    {
        //构建唯一keyName 用于记录存在在字典中的对象
        List<string> list = new List<string>(keys);
        string keyName = "";
        foreach (string key in list)
            keyName += key + "_";
        keyName += typeof(T).Name;

        AsyncOperationHandle<IList<T>> handle;
        if (resDic.ContainsKey(keyName))
        {
            handle = resDic[keyName].handle.Convert<IList<T>>();
            //要使用资源了 那么引用计数+1
            resDic[keyName].count += 1;
            if (handle.IsDone)
            {
                foreach (T item in handle.Result)
                    callBack(item);
            }
            else
            {
                handle.Completed += (obj) =>
                {
                    //加载成功才调用外部传入的委托函数
                    if (obj.Status == AsyncOperationStatus.Succeeded)
                    {
                        foreach (T item in handle.Result)
                            callBack(item);
                    }
                };
            }
            return;
        }

        handle = Addressables.LoadAssetsAsync<T>(list, callBack, Addressables.MergeMode.Intersection);//查找资源方式
        handle.Completed += (obj) =>
        {
            if (obj.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError("资源加载失败：" + keyName);
                if (resDic.ContainsKey(keyName))
                    resDic.Remove(keyName);
            }
        };
        AddressablesInfo info = new AddressablesInfo(handle);
        resDic.Add(keyName, info);
    }


    /// <summary>
    /// 通过资源名 释放资源
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="name">资源名</param>
    public void Release<T>(string name)
    {
        string keyName = name + "_" + typeof(T).Name;
        if (resDic.ContainsKey(keyName))
        {
            //这个资源加载了几次 就要释放多少次才可以 真正移除资源
            resDic[keyName].count -= 1;
            if (resDic[keyName].count == 0)
            {
                AsyncOperationHandle<T> handle = resDic[keyName].handle.Convert<T>();
                Addressables.Release(handle);
                resDic.Remove(keyName);
            }
        }
    }
    /// <summary>
    /// 释放有多个标签资源的方法 
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="keys">资源名 + 标签。可变长参数，可传多个参数(标签)</param>
    public void Release<T>(params string[] keys)
    {
        List<string> list = new List<string>(keys);
        string keyName = "";
        foreach (string key in list)
            keyName += key + "_";
        keyName += typeof(T).Name;

        if (resDic.ContainsKey(keyName))
        {
            resDic[keyName].count -= 1;
            if (resDic[keyName].count == 0)
            {
                AsyncOperationHandle<IList<T>> handle = resDic[keyName].handle.Convert<IList<T>>();
                Addressables.Release(handle);
                resDic.Remove(keyName);
            }
        }
    }

    //清空所有资源 谨慎使用！！！
    public void Clear()
    {
        foreach (var item in resDic.Values)
        {
            Addressables.Release(item.handle);
        }
        resDic.Clear();
        AssetBundle.UnloadAllAssetBundles(true);
        MonoMgr.Instance.StartCoroutine(CleareRes());
    }
    IEnumerator CleareRes()
    {
        AsyncOperation async = Resources.UnloadUnusedAssets();
        yield return async;
        GC.Collect();
    }
}
