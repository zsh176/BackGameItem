using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// �����л�ģ��
/// </summary>
public class ScenesMgr : InstanceMgr<ScenesMgr>
{
    /// <summary>
    /// �л����� ͬ��
    /// </summary>
    public void LoadScene(string name, UnityAction fun)
    {
        //����ͬ������
        SceneManager.LoadScene(name);
        //������ɹ��� �Ż�ȥִ��fun
        fun();
    }

    /// <summary>
    /// �ṩ���ⲿ�� �첽���صĽӿڷ���
    /// </summary>
    public void LoadSceneAsyn(string name,UnityAction fun)
    {
        MonoMgr.Instance.StartCoroutine(ReallyLoadSceneAsyn(name, fun));
    }

    /// <summary>
    /// Э���첽���س���
    /// </summary>
    IEnumerator ReallyLoadSceneAsyn(string name,UnityAction fun)
    {
                            //�л�����
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        //���Եõ��������ص�һ������
        while (!ao.isDone)
        {
            //�¼����� ����ַ� �������  �������þ���
            //EventCenter.Instance.EventTrigger("����������", ao.progress);
            //������ȥ���½�����
            yield return ao.progress;
        }
        //������ɹ��� �Ż�ȥִ��fun
        fun();
    }
}
