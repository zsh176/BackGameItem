using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Input�������ģ��
/// </summary>
public class InputMgr : InstanceMgr<InputMgr>
{
    private bool isStart = false;

    /// <summary>
    /// ���캯���� ���Updata����
    /// </summary>
    public InputMgr()
    {
        MonoMgr.Instance.AddUpdateListener(MyUpdate);
    }

    /// <summary>
    /// �Ƿ�����ر� �ҵ�������
    /// </summary>
    public void StartOrEndCheck(bool isOpen)
    {
        isStart = isOpen;
    }

    /// <summary>
    /// ������ⰴ��̧���� �ַ��¼���
    /// </summary>
    /// <param name="key"></param>
    private void CheckKeyCode(KeyCode key)
    {
        //�¼�����ģ�� �ַ�����̧���¼�
        //if (Input.GetKeyDown(key))
            //EventCenter.Instance.EventTrigger("�ƶ�������", key);
        //�¼�����ģ�� �ַ�����̧���¼�
        //if (Input.GetKeyUp(key))
            //EventCenter.Instance.EventTrigger("ĳ��̧��", key);
    }
     
    private void MyUpdate()
    {
        //û�п��������� �Ͳ�ȥ��� ֱ��return
        if (!isStart) return;

        CheckKeyCode(KeyCode.W);
        CheckKeyCode(KeyCode.S);
        CheckKeyCode(KeyCode.A);
        CheckKeyCode(KeyCode.D);

    }
}
