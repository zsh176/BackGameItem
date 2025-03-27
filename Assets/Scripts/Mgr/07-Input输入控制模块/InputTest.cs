using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Input�������ģ����Խű�
/// </summary>
public class InputTest : MonoBehaviour
{
    void Start()
    {
        //����������
        InputMgr.Instance.StartOrEndCheck(true);

        //����¼�����
        //EventCenter.Instance.AddEventListener<KeyCode>("ĳ������", CheckInputDown);
        //EventCenter.Instance.AddEventListener<KeyCode>("ĳ��̧��", CheckInputUp);
       
    }
    void OnDestroy()
    {
        //����������ʱ �ر�������
        InputMgr.Instance.StartOrEndCheck(false);
    }

    private void CheckInputDown(KeyCode key)
    {
        //KeyCode k = (KeyCode)key;
        switch (key)
        {
            case KeyCode.A:
                Debug.Log("A����");
                break;
            case KeyCode.D:
                Debug.Log("D����");
                break;
            case KeyCode.W:
                Debug.Log("W����");
                break;
            case KeyCode.S:
                Debug.Log("S����");
                break;
        }
    }
    private void CheckInputUp(KeyCode key)
    {
        //KeyCode k = (KeyCode)key;
        switch (key)
        {
            case KeyCode.A:
                Debug.Log("A̧��");
                break;
            case KeyCode.D:
                Debug.Log("Ḑ��");
                break;
            case KeyCode.W:
                Debug.Log("W̧��");
                break;
            case KeyCode.S:
                Debug.Log("Ş��");
                break;
        }
    }
}
