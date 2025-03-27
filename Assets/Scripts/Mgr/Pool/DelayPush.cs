using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//挂载在对象身上
public class DelayPush : MonoBehaviour
{
    public float isTime = 1;//多久后再把它放入对象池
    //对象每次激活的时候都会调用函数
    void OnEnable()
    {
        Invoke("Push", isTime);
    }

    //往池子里面放东西
    public void Push()
    {
        PoolMgr.Instance.PushObj(gameObject.name,gameObject);
    }
}
