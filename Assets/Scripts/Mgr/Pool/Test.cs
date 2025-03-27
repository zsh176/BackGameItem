using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        //从缓存池中拿东西出来用
        if (Input.GetMouseButtonDown(0))
        {
            //异步加载资源
            PoolMgr.Instance.GetObj("Test/Cube", new Vector3(Random.Range(0, 11), 0, Random.Range(0, 11)), (obj) =>
            {

            });
        }
        if (Input.GetMouseButtonDown(1))
        {
            //异步加载资源
            PoolMgr.Instance.GetObj("Test/Sphere", new Vector3(Random.Range(0, 11), 0, Random.Range(0, 11)), (obj) =>
            {

            });
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //异步加载资源
            ResMgr.Instance.LoadAsync<GameObject>("Test/Cube", (obj) =>
            {
                //资源加载完成后 想做的事情
                obj.transform.localScale = Vector3.zero * 2; 
            });
        }
    }
}
