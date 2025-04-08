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
public class JsonMgr : InstanceMgr<JsonMgr>
{
    /// <summary>
    /// �洢·��
    /// </summary>
    private string DataPath => Application.persistentDataPath;



    /// <summary>
    /// �洢Json���� ���л�
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    /// <param name="data">�洢����</param>
    /// <param name="_path">·��ǰ׺</param>
    /// <param name="fileName">�洢���ļ���</param>
    /// <param name="type">���л���ʽ</param>
    public void SaveData<T>(object data, string _path = "", string fileName = "", JsonType type = JsonType.LitJson) where T : new()
    {
        //�Ƿ���·��ǰ׺
        if (_path != "")
            _path += "/";
        //Ĭ�����������ļ���
        fileName = typeof(T).Name;

        //�洢·��
        string path = $"{DataPath}/{_path}{fileName}.json";
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
    /// ��ȡָ���ļ��е�Json���� �������л�
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    /// <param name="_path">·��ǰ׺</param>
    /// <param name="fileName">�洢���ļ���</param>
    /// <param name="type">���л���ʽ</param>
    /// <returns></returns>
    public T LoadData<T>(string _path = "" , string fileName = "", JsonType type = JsonType.LitJson) where T : new ()
    {
        //�Ƿ����·��ǰ׺
        if (_path != "")
            _path += "/";
        //Ĭ�����������ļ���
        fileName = typeof(T).Name;

        //�����ж� streamingAssets�ļ������Ƿ������ݣ�������һ��Ϊ�ؿ�����̶������޸ĵ�����
        string path = $"{Application.streamingAssetsPath}/{_path}{fileName}.json";
        //���������Ĭ���ļ� �ʹ� ��д�ļ�����ȥ��
        if (!File.Exists(path))
        {
            path = $"{DataPath}/{_path}{fileName}.json";
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
        return data;
    }
}
