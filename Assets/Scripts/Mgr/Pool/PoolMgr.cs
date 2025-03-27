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
    //��������� (�¹�)
    public Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();
    //�����¹�����ʱ���õĶ����ĸ�����
    private GameObject poolObj;


    /// <summary>
    /// ������Դ
    /// </summary>
    /// <param name="name">��Դ·��</param>
    /// <param name="pos">����λ��</param>
    /// <param name="callBack">���ɺ�����������</param>
    public void GetObj(string name, Vector3 pos, UnityAction<GameObject> callBack)
    {
        //�ж�Ӧ�ļ�(����) ���ҳ��������ж���
        if (poolDic.ContainsKey(name) && poolDic[name].poolList.Count > 0)
        {
            callBack(poolDic[name].GetObj(pos));
        }
        else
        {
            //ͨ���첽������Դ ����������ⲿ��
            ResMgr.Instance.LoadAsync<GameObject>(name, (o) =>
            {
                //��Դ������ɺ� ����������
                o.name = name;
                callBack(o);
            });
            //����һ������������������� ���ǳ��ӵ�����
        }
    }

    /// <summary>
    /// ������Դ
    /// </summary>
    /// <param name="name">����һ�����򣬳��ӵ����� �������������</param>
    /// <param name="obj"></param>
    public void PushObj(string name, GameObject obj)
    {
        if (poolObj == null)
        {
            poolObj = new GameObject("Pool");
        }

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
    //����ĳ���
    public List<GameObject> poolList;

    public PoolData(GameObject obj, GameObject poolObj)
    {
        //�����ǵĳ��� ����һ�������� ���Ұ�����Ϊpool���Ӷ���
        fatherObj = new GameObject(obj.name);
        fatherObj.transform.parent = poolObj.transform;

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
        obj.transform.parent = fatherObj.transform;
    }

    //�ӳ������ö���
    public GameObject GetObj(Vector3 pos)
    {
        GameObject obj = null;
        //ȡ����һ��
        obj = poolList[0];
        //�ó�ȥ���� ��Ҫ�ֵ���ɾ��
        poolList.RemoveAt(0);
        //����λ��
        obj.transform.position = pos;

        //���� ������ʾ
        obj.SetActive(true);
        //�Ͽ����ӹ�ϵ
        obj.transform.parent = null;
        return obj;
    }
}
