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
        //�ӻ�������ö���������
        if (Input.GetMouseButtonDown(0))
        {
            //�첽������Դ
            PoolMgr.Instance.GetObj("Test/Cube", new Vector3(Random.Range(0, 11), 0, Random.Range(0, 11)), (obj) =>
            {

            });
        }
        if (Input.GetMouseButtonDown(1))
        {
            //�첽������Դ
            PoolMgr.Instance.GetObj("Test/Sphere", new Vector3(Random.Range(0, 11), 0, Random.Range(0, 11)), (obj) =>
            {

            });
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //�첽������Դ
            ResMgr.Instance.LoadAsync<GameObject>("Test/Cube", (obj) =>
            {
                //��Դ������ɺ� ����������
                obj.transform.localScale = Vector3.zero * 2; 
            });
        }
    }
}
