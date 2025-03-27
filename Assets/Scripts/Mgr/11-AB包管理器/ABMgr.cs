using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// AB�������� �����ⲿ������Դ
/// </summary>
public class ABMgr : BaseManager<ABMgr>
{
    //���� ���ڻ�ȡ��������Ϣ
    private AssetBundle mainAB = null;
    //��������ȡ�õ������ļ�
    AssetBundleManifest mainfest = null;

    //AB���ظ����ػᱨ�� �������ֵ�洢
    private Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();

    /// <summary>
    /// AB�����·�� �����޸�
    /// </summary>
    private string PathUrl => Application.streamingAssetsPath + "/";

    /// <summary>
    /// ������ ����ƽ̨���ض�Ӧƽ̨��Ϊ������
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
    /// ����AB��
    /// </summary>
    public void Load(string abName)
    {
        if (mainAB == null)
        {
            //��������
            mainAB = AssetBundle.LoadFromFile(PathUrl + mainABName);
            //���������еĹ̶��ļ�  �̶�д��
            mainfest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
        AssetBundle ab = null;
        //�ӹ̶��ļ��� �õ������������Ϣ
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
            //����Ŀ����Դ��
            ab = AssetBundle.LoadFromFile(PathUrl + abName);
            //�����ֵ�
            abDic.Add(abName, ab);
        }
    }

    /// <summary>
    ///  ͬ������ ����typeָ������
    /// </summary>
    public Object LoadRes(string abName, string resName, System.Type type)
    {
        // ����AB��
        Load(abName);
        //����AB���ڵ�һ����Դ
        Object obj = abDic[abName].LoadAsset(resName, type);
        return obj;
    }


    /// <summary>
    /// ͬ������ ���ݷ���ָ������
    /// </summary>
    public T LoadRes<T>(string abName, string resName) where T : Object
    {
        // ����AB��
        Load(abName);
        //����AB���ڵ�һ����Դ
        T obj = abDic[abName].LoadAsset<T>(resName);
        return obj;
    }


    /// <summary>
    /// �첽���� ����typeָ������
    /// </summary>
    public void LoadAssetAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack)
    {
        StartCoroutine(ReallyLoadAssetAsync(abName, resName, type, callBack));
    }
    private IEnumerator ReallyLoadAssetAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack)
    {
        // ����AB��
        Load(abName);
        AssetBundleRequest abr = abDic[abName].LoadAssetAsync(resName, type);
        yield return abr;
        callBack(abr.asset);
    }


    /// <summary>
    /// �첽���� ���ݷ���ָ������
    /// </summary>
    /// <returns></returns>
    public void LoadAssetAsync<T>(string abName, string resName, UnityAction<T> callBack) where T : Object
    {
        StartCoroutine(ReallyLoadAssetAsync<T>(abName, resName, callBack));
    }
    private IEnumerator ReallyLoadAssetAsync<T>(string abName, string resName, UnityAction<T> callBack) where T : Object
    {
        // ����AB��
        Load(abName);
        AssetBundleRequest abr = abDic[abName].LoadAssetAsync<T>(resName);
        yield return abr;
        callBack(abr.asset as T);
    }


    /// <summary>
    /// ж�ص���AB��
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
    /// ж�����а� ��Ҫ�����л�����
    /// </summary>
    public void ClearAB()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        abDic.Clear();
        mainAB = null;
        mainfest = null;
    }
}
