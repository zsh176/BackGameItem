using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// AB包管理器 方便外部加载资源
/// </summary>
public class ABMgr : BaseManager<ABMgr>
{
    //主包 用于获取依赖项信息
    private AssetBundle mainAB = null;
    //依赖包获取用的配置文件
    AssetBundleManifest mainfest = null;

    //AB包重复加载会报错 所以用字典存储
    private Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();

    /// <summary>
    /// AB包存放路径 方便修改
    /// </summary>
    private string PathUrl => Application.streamingAssetsPath + "/";

    /// <summary>
    /// 主包名 根据平台返回对应平台作为主包名
    /// </summary>
    private string mainABName
    {
        get
        {
#if UNITY_IOS
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#else
            return "PC";
#endif
        }
    }

    /// <summary>
    /// 加载AB包
    /// </summary>
    public void Load(string abName)
    {
        if (mainAB == null)
        {
            //加载主包
            mainAB = AssetBundle.LoadFromFile(PathUrl + mainABName);
            //加载主包中的固定文件  固定写法
            mainfest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
        AssetBundle ab = null;
        //从固定文件中 得到依赖包相关信息
        string[] strs = mainfest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            if (!abDic.ContainsKey(strs[i]))
            {
                ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                abDic.Add(strs[i], ab);
            }
        }
        if (!abDic.ContainsKey(abName))
        {
            //加载目标资源包
            ab = AssetBundle.LoadFromFile(PathUrl + abName);
            //存入字典
            abDic.Add(abName, ab);
        }
    }

    /// <summary>
    ///  同步加载 根据type指定类型
    /// </summary>
    public Object LoadRes(string abName, string resName, System.Type type)
    {
        // 加载AB包
        Load(abName);
        //加载AB包内的一个资源
        Object obj = abDic[abName].LoadAsset(resName, type);
        return obj;
    }


    /// <summary>
    /// 同步加载 根据泛型指定类型
    /// </summary>
    public T LoadRes<T>(string abName, string resName) where T : Object
    {
        // 加载AB包
        Load(abName);
        //加载AB包内的一个资源
        T obj = abDic[abName].LoadAsset<T>(resName);
        return obj;
    }


    /// <summary>
    /// 异步加载 根据type指定类型
    /// </summary>
    public void LoadAssetAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack)
    {
        StartCoroutine(ReallyLoadAssetAsync(abName, resName, type, callBack));
    }
    private IEnumerator ReallyLoadAssetAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack)
    {
        // 加载AB包
        Load(abName);
        AssetBundleRequest abr = abDic[abName].LoadAssetAsync(resName, type);
        yield return abr;
        callBack(abr.asset);
    }


    /// <summary>
    /// 异步加载 根据泛型指定类型
    /// </summary>
    /// <returns></returns>
    public void LoadAssetAsync<T>(string abName, string resName, UnityAction<T> callBack) where T : Object
    {
        StartCoroutine(ReallyLoadAssetAsync<T>(abName, resName, callBack));
    }
    private IEnumerator ReallyLoadAssetAsync<T>(string abName, string resName, UnityAction<T> callBack) where T : Object
    {
        // 加载AB包
        Load(abName);
        AssetBundleRequest abr = abDic[abName].LoadAssetAsync<T>(resName);
        yield return abr;
        callBack(abr.asset as T);
    }


    /// <summary>
    /// 卸载单个AB包
    /// </summary>
    public void UnLoad(string abName)
    {
        if (abDic.ContainsKey(abName))
        {
            abDic[abName].Unload(false);
            abDic.Remove(abName);
        }
    }

    /// <summary>
    /// 卸载所有包 主要用于切换场景
    /// </summary>
    public void ClearAB()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        abDic.Clear();
        mainAB = null;
        mainfest = null;
    }
}
