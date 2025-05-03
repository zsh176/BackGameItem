using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

//��Ѱַ��Դ ��Ϣ
public class AddressablesInfo
{
    //��¼ �첽�������
    public AsyncOperationHandle handle;
    /// <summary>
    /// ���ü�����
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
    //�洢�����Ѽ��ص���Դ �����ظ�������ͬ��Դ
    public Dictionary<string, AddressablesInfo> resDic = new Dictionary<string, AddressablesInfo>();


    /// <summary>
    /// �첽������Դ��
    /// </summary>
    /// <typeparam name="T">��Դ����</typeparam>
    /// <param name="name">��Դ��</param>
    /// <param name="callBack">������ɻص�</param>
    public void LoadAssetAsync<T>(string name, Action<T> callBack)
    {
        //����ΨһkeyName ���ڼ�¼�������ֵ��еĶ���
        string keyName = name + "_" + typeof(T).Name;
        AsyncOperationHandle<T> handle;

        //����Ѿ����ع�����Դ
        if (resDic.ContainsKey(keyName))
        {
            handle = resDic[keyName].handle.Convert<T>();
            //Ҫʹ����Դ�� ���ü���+1
            resDic[keyName].count += 1;
            if (handle.IsDone)
            {
                //�첽���ؽ��� ��ȡ�Ѽ��غõ���Դ ֱ���൱��ͬ��������
                callBack(handle.Result);
            }
            else
            {
                //��û�м������ ��ӻص�
                handle.Completed += (obj) => {
                    if (obj.Status == AsyncOperationStatus.Succeeded)
                        callBack(obj.Result);
                };
            }
            return;
        }

        //û�м��ع�����Դ �����첽���� ���Ҽ�¼
        handle = Addressables.LoadAssetAsync<T>(name);
        handle.Completed += (obj) => {
            if (obj.Status == AsyncOperationStatus.Succeeded)
                callBack(obj.Result);
            else
            {
                Debug.LogWarning(keyName + "��Դ����ʧ��");
                if (resDic.ContainsKey(keyName))
                    resDic.Remove(keyName);
            }
        };
        AddressablesInfo info = new AddressablesInfo(handle);
        resDic.Add(keyName, info);
    }

    /// <summary>
    /// �첽���ض����ǩ��Դ���ɱ䳤����ֻ�����������һ�������������ȴ��ص����ٴ���ǩ
    /// </summary>
    /// <typeparam name="T">��Դ����</typeparam>
    /// <param name="callBack">������ɻص�</param>
    /// <param name="keys">��Դ�� + ��ǩ���ɱ䳤�������ɴ��������(��ǩ)</param>
    public void LoadAssetAsync<T>(Action<T> callBack, params string[] keys)
    {
        //����ΨһkeyName ���ڼ�¼�������ֵ��еĶ���
        List<string> list = new List<string>(keys);
        string keyName = "";
        foreach (string key in list)
            keyName += key + "_";
        keyName += typeof(T).Name;

        AsyncOperationHandle<IList<T>> handle;
        if (resDic.ContainsKey(keyName))
        {
            handle = resDic[keyName].handle.Convert<IList<T>>();
            //Ҫʹ����Դ�� ��ô���ü���+1
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
                    //���سɹ��ŵ����ⲿ�����ί�к���
                    if (obj.Status == AsyncOperationStatus.Succeeded)
                    {
                        foreach (T item in handle.Result)
                            callBack(item);
                    }
                };
            }
            return;
        }

        handle = Addressables.LoadAssetsAsync<T>(list, callBack, Addressables.MergeMode.Intersection);//������Դ��ʽ
        handle.Completed += (obj) =>
        {
            if (obj.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError("��Դ����ʧ�ܣ�" + keyName);
                if (resDic.ContainsKey(keyName))
                    resDic.Remove(keyName);
            }
        };
        AddressablesInfo info = new AddressablesInfo(handle);
        resDic.Add(keyName, info);
    }


    /// <summary>
    /// ͨ����Դ�� �ͷ���Դ
    /// </summary>
    /// <typeparam name="T">��Դ����</typeparam>
    /// <param name="name">��Դ��</param>
    public void Release<T>(string name)
    {
        string keyName = name + "_" + typeof(T).Name;
        if (resDic.ContainsKey(keyName))
        {
            //�����Դ�����˼��� ��Ҫ�ͷŶ��ٴβſ��� �����Ƴ���Դ
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
    /// �ͷ��ж����ǩ��Դ�ķ��� 
    /// </summary>
    /// <typeparam name="T">��Դ����</typeparam>
    /// <param name="keys">��Դ�� + ��ǩ���ɱ䳤�������ɴ��������(��ǩ)</param>
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

    //���������Դ ����ʹ�ã�����
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
