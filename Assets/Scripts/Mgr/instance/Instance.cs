using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// δ�̳�MonoBehaviour����д��
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
/// �̳�MonoBehaviour����д��
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
                //����һ���յ���Ϸ����
                GameObject obj = new GameObject();
                //���������ó�����
                obj.name = typeof(T).ToString();
                //��Ϊ��Ϸ�����ǹ����ڳ����ϵ� ����Ҫ����������ʱ����ɾ��
                //��Ϊ����ģʽ���� �����Ǵ��������������ڵ�
                DontDestroyOnLoad(obj);
                //�ٸ�����������Ͻű� ��ֵ��instance
                instance = obj.AddComponent<T>();
            }
            return instance; 
        }
    }
}