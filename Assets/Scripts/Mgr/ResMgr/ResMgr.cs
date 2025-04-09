using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ��Դ����ģ��
/// </summary>
public class ResMgr : BaseManager<ResMgr>
{

    /// <summary>
    /// ͬ��������Դ  ���������һ��GameObject���͵� ʵ������ �ٷ��س�ȥ �ⲿ ֱ��ʹ�ü���
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public T Load<T>(string name) where T : Object
    {
        return Resources.Load<T>(name);
    }


    //�첽������Դ
    public void LoadAsync<T>(string name, UnityAction<T> callback) where T : Object
    {
        //�����첽���ص�Э��
        MonoMgr.Instance.StartCoroutine(ReallyLoadAsync(name, callback));
    }

    //������Эͬ������  ���� �����첽���ض�Ӧ����Դ
    private IEnumerator ReallyLoadAsync<T>(string name, UnityAction<T> callback) where T : Object
    {
        ResourceRequest r = Resources.LoadAsync<T>(name);
        yield return r;

        callback(r.asset as T);

    }


}
