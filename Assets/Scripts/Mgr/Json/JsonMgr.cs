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
public class JsonMgr
{
    private static JsonMgr instance = new JsonMgr();
    public static JsonMgr Instance => instance;

    private JsonMgr() { }

    /// <summary>
    /// 存储Json数据 序列化
    /// </summary>
    /// <param name="data">存什么类型</param>
    /// <param name="fileName">文件夹名</param>
    /// <param name="type"></param>
    public void  SaveData(object data,string fileName,JsonType type = JsonType.LitJson)
    {
        //存储路径
        string path = Application.persistentDataPath + "/" + fileName + ".json";
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
    /// 读取指定文件中的Json数据 反序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fileName"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public T LoadData<T>(string fileName, JsonType type = JsonType.LitJson) where T : new ()
    {
        //确定从哪个路径读取
        //首先先判断 默认数据文件夹中是否有我们想要的数据 如果有 就从中获取
        string path = Application.streamingAssetsPath + "/" + fileName + ".json";
        //先判断是否存在这个文件
        //如果不存在默认文件 就从 读写文件夹中去找
        if (!File.Exists(path))
        {
            path = Application.persistentDataPath + "/" + fileName + ".json";
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

        //把对象返回出去
        return data;
    }
}
