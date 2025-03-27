using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Input输入控制模块测试脚本
/// </summary>
public class InputTest : MonoBehaviour
{
    void Start()
    {
        //开启输入检测
        InputMgr.Instance.StartOrEndCheck(true);

        //添加事件监听
        //EventCenter.Instance.AddEventListener<KeyCode>("某键按下", CheckInputDown);
        //EventCenter.Instance.AddEventListener<KeyCode>("某键抬起", CheckInputUp);
       
    }
    void OnDestroy()
    {
        //当对象被销毁时 关闭输入检测
        InputMgr.Instance.StartOrEndCheck(false);
    }

    private void CheckInputDown(KeyCode key)
    {
        //KeyCode k = (KeyCode)key;
        switch (key)
        {
            case KeyCode.A:
                Debug.Log("A按下");
                break;
            case KeyCode.D:
                Debug.Log("D按下");
                break;
            case KeyCode.W:
                Debug.Log("W按下");
                break;
            case KeyCode.S:
                Debug.Log("S按下");
                break;
        }
    }
    private void CheckInputUp(KeyCode key)
    {
        //KeyCode k = (KeyCode)key;
        switch (key)
        {
            case KeyCode.A:
                Debug.Log("A抬起");
                break;
            case KeyCode.D:
                Debug.Log("D抬起");
                break;
            case KeyCode.W:
                Debug.Log("W抬起");
                break;
            case KeyCode.S:
                Debug.Log("S抬起");
                break;
        }
    }
}
