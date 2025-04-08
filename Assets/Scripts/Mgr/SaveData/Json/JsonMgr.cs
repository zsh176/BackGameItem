using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 序列化和反序列化时 用哪一种方案
/// </summary>
public enum JsonType
{
    JsonUtlity, LitJson,
}

/// <summary>
/// Json数据管理类 用于Json的序列化存储到硬盘 和 反序列化从硬盘读取到内存中
/// </summary>
public class JsonMgr : InstanceMgr<JsonMgr>
{
    /// <summary>
    /// 存储路径
    /// </summary>
    private string DataPath => Application.persistentDataPath;



    /// <summary>
    /// 存储Json数据 序列化
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="data">存储数据</param>
    /// <param name="_path">路径前缀</param>
    /// <param name="fileName">存储的文件名</param>
    /// <param name="type">序列化方式</param>
    public void SaveData<T>(object data, string _path = "", string fileName = "", JsonType type = JsonType.LitJson) where T : new()
    {
        //是否有路径前缀
        if (_path != "")
            _path += "/";
        //默认类名就是文件名
        fileName = typeof(T).Name;

        //存储路径
        string path = $"{DataPath}/{_path}{fileName}.json";
        //序列化 得到Json字符串
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
        //把序列好的Json字符串 存储到指定路径的文件中
        File.WriteAllText(path, jsonStr);
    }

    /// <summary>
    /// 读取指定文件中的Json数据 并反序列化
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="_path">路径前缀</param>
    /// <param name="fileName">存储的文件名</param>
    /// <param name="type">序列化方式</param>
    /// <returns></returns>
    public T LoadData<T>(string _path = "" , string fileName = "", JsonType type = JsonType.LitJson) where T : new ()
    {
        //是否添加路径前缀
        if (_path != "")
            _path += "/";
        //默认类名就是文件名
        fileName = typeof(T).Name;

        //首先判断 streamingAssets文件夹中是否有数据，此数据一般为关卡怪物固定不能修改的属性
        string path = $"{Application.streamingAssetsPath}/{_path}{fileName}.json";
        //如果不存在默认文件 就从 读写文件夹中去找
        if (!File.Exists(path))
        {
            path = $"{DataPath}/{_path}{fileName}.json";
        }
        //如果读写文件夹中都还没有 那就返回一个默认对象
        if (!File.Exists(path)) return new T();

        //把文件读取出来
        string jsonStr = File.ReadAllText(path);
        //数据对象
        T data = default(T);
        //进行反序列化
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
