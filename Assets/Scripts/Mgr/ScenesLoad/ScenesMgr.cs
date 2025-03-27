using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换模块
/// </summary>
public class ScenesMgr : InstanceMgr<ScenesMgr>
{
    /// <summary>
    /// 切换场景 同步
    /// </summary>
    public void LoadScene(string name, UnityAction fun)
    {
        //场景同步加载
        SceneManager.LoadScene(name);
        //加载完成过后 才会去执行fun
        fun();
    }

    /// <summary>
    /// 提供给外部的 异步加载的接口方法
    /// </summary>
    public void LoadSceneAsyn(string name,UnityAction fun)
    {
        MonoMgr.Instance.StartCoroutine(ReallyLoadSceneAsyn(name, fun));
    }

    /// <summary>
    /// 协程异步加载场景
    /// </summary>
    IEnumerator ReallyLoadSceneAsyn(string name,UnityAction fun)
    {
                            //切换场景
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        //可以得到场景加载的一个进度
        while (!ao.isDone)
        {
            //事件中心 向外分发 进度情况  外面想用就用
            //EventCenter.Instance.EventTrigger("进度条更新", ao.progress);
            //这里面去更新进度条
            yield return ao.progress;
        }
        //加载完成过后 才会去执行fun
        fun();
    }
}
