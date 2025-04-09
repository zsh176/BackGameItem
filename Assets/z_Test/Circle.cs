using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    private void OnEnable()
    {
        Invoke("Hide", 3);
    }

    void Hide()
    {
        PoolMgr.Instance.PushObj(gameObject);
    }
}
