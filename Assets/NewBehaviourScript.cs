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
            Debug.Log("触发事件");
            EventCenter.Instance.EventTrigger(E_EventType.test,120);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("移除事件");
            EventCenter.Instance.RemoveEventListener<int>(E_EventType.test, test);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("添加事件");
            EventCenter.Instance.AddEventListener<int>(E_EventType.test, test);
        }
    }

    public void test(int i)
    {
        Debug.Log($"事件触发 传入的参数是{i}");
    }
}
