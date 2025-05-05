using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// �����ģ��
/// </summary>
public class PoolMgr : InstanceMgr<PoolMgr>
{
    public PoolMgr()
    {
        poolObj = new GameObject("Pool", typeof(RectTransform));
        poolObj.transform.SetParent(UIManager.Instance.GetLayer(E_UI_Layer.Bot), false);
        //instObj = new GameObject("Inst");
    }

    //���������
    public Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();
    //���ڻ������ʱ�ĸ�����
    private GameObject poolObj;
    //ʵ�����ڳ�����ʱ�ĸ�����
    //private GameObject instObj;

    /// <summary>
    /// ��ȡ�������Դ
    /// </summary>
    /// <param name="name">��Դ��</param>
    /// <param name="callBack">���������ʵ�����ڻص��в�Ҫ�ظ�����</param>
    public void GetObj(string name, UnityAction<GameObject> callBack)
    {
        //�ж�Ӧ�ļ�(����) ���ҳ��������ж���
        if (poolDic.ContainsKey(name) && poolDic[name].poolList.Count > 0)
        {
            callBack(poolDic[name].GetObj());
        }
        else
        {
            //ͨ���첽������Դ ����������ⲿ��
            AddressablesMgr.Instance.LoadAssetAsync<GameObject>(obj =>
            {
                //ʵ�������ٴ���ȥ
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
    /// ������Դ
    /// </summary>
    /// <param name="obj">��Ҫ���յ�����</param>
    public void PushObj(GameObject obj)
    {
        string name = obj.name;
        //�����г���
        if (poolDic.ContainsKey(name))
        {
            //�Ž���Ӧ�ĳ���
            poolDic[name].PushObj(obj);
        }
        //����û�г���
        else
        {
            //����һ������ ���ҰѶ����Ž�ȥ
            poolDic.Add(name, new PoolData(obj, poolObj));
        }
    }

    /// <summary>
    /// ��ջ���صķ��� ��Ҫ�����л�����ʱ
    /// </summary>
    public void Clear() 
    { 
        //����ֵ�
        poolDic.Clear();
        poolObj = null;
    }

}


/// <summary>
/// �������� �����е�һ������
/// </summary>
public class PoolData
{
    //������ ������صĸ��ڵ�
    public GameObject fatherObj;
    //public Transform fatherObj;
    //����ĳ���
    public List<GameObject> poolList;

    public PoolData(GameObject obj, GameObject poolObj)
    {
        //�����ǵĳ��� ����һ�������� ���Ұ�����Ϊpool���Ӷ���
        fatherObj = new GameObject(obj.name, typeof(RectTransform));

        fatherObj.transform.SetParent(poolObj.transform, false);

        //����һ������ ���ҰѶ����Ž�ȥ
        poolList = new List<GameObject>() { };
        PushObj(obj);
    }

    //�����������ʱ���õĶ���
    public void PushObj(GameObject obj)
    {
        //ʧ�� ��������
        obj.SetActive(false);
        //��ӽ��б�
        poolList.Add(obj);
        //���ø�����
        obj.transform.SetParent(fatherObj.transform, false);
    }

    //�ӳ������ö���
    public GameObject GetObj(Transform parent = null)
    {
        GameObject obj = null;
        //ȡ����һ��
        obj = poolList[0];
        //�ó�ȥ���� ��Ҫ�ֵ���ɾ��
        poolList.RemoveAt(0);
        //���� ������ʾ
        obj.SetActive(true);
        ////�Ͽ����ӹ�ϵ
        //obj.transform.parent = null;
        //������ʾʱ�ĸ�����
        //obj.transform.parent = parent;
        return obj;
    }
}


