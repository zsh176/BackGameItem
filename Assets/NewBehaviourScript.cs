using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("�����¼�");
            EventCenter.Instance.EventTrigger(E_EventType.test,120);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("�Ƴ��¼�");
            EventCenter.Instance.RemoveEventListener<int>(E_EventType.test, test);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("����¼�");
            EventCenter.Instance.AddEventListener<int>(E_EventType.test, test);
        }
    }

    public void test(int i)
    {
        Debug.Log($"�¼����� ����Ĳ�����{i}");
    }
}
