using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// ���л��ͷ����л�ʱ ����һ�ַ���
/// </summary>
public enum JsonType
{
    JsonUtlity, LitJson,
}

/// <summary>
/// Json���ݹ����� ����Json�����л��洢��Ӳ�� �� �����л���Ӳ�̶�ȡ���ڴ���
/// </summary>
public class JsonMgr
{
    private static JsonMgr instance = new JsonMgr();
    public static JsonMgr Instance => instance;

    private JsonMgr() { }

    /// <summary>
    /// �洢Json���� ���л�
    /// </summary>
    /// <param name="data">��ʲô����</param>
    /// <param name="fileName">�ļ�����</param>
    /// <param name="type"></param>
    public void  SaveData(object data,string fileName,JsonType type = JsonType.LitJson)
    {
        //�洢·��
        string path = Application.persistentDataPath + "/" + fileName + ".json";
        //���л� �õ�Json�ַ���
        string jsonStr = "";
        switch (type)
        {
            case JsonType.JsonUtlity:
                jsonStr = JsonUtility.ToJson(data);
                break;
            case JsonType.LitJson:
                jsonStr = JsonMapper.ToJson(data);
                break;
        }
        //�����кõ�Json�ַ��� �洢��ָ��·�����ļ���
        File.WriteAllText(path, jsonStr);
    }

    /// <summary>
    /// ��ȡָ���ļ��е�Json���� �����л�
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fileName"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public T LoadData<T>(string fileName, JsonType type = JsonType.LitJson) where T : new ()
    {
        //ȷ�����ĸ�·����ȡ
        //�������ж� Ĭ�������ļ������Ƿ���������Ҫ������ ����� �ʹ��л�ȡ
        string path = Application.streamingAssetsPath + "/" + fileName + ".json";
        //���ж��Ƿ��������ļ�
        //���������Ĭ���ļ� �ʹ� ��д�ļ�����ȥ��
        if (!File.Exists(path))
        {
            path = Application.persistentDataPath + "/" + fileName + ".json";
        }
        //�����д�ļ����ж���û�� �Ǿͷ���һ��Ĭ�϶���
        if (!File.Exists(path)) return new T();

        //���ļ���ȡ����
        string jsonStr = File.ReadAllText(path);
        //���ݶ���
        T data = default(T);
        //���з����л�
        switch (type) 
        { 
            case JsonType.JsonUtlity:
                data = JsonUtility.FromJson<T>(jsonStr);
                break;
            case JsonType.LitJson:
                data = JsonMapper.ToObject<T>(jsonStr);
                break;
        }

        //�Ѷ��󷵻س�ȥ
        return data;
    }
}
