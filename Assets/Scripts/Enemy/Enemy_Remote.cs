using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Զ�̹�������
/// </summary>
public class Enemy_Remote : EnemyBase
{
    private GameObject remote_Atk;//Զ�̹�����ⷶΧ
    protected override void Awake()
    {
        base.Awake();
        remote_Atk = transform.Find("Remote_Atk").gameObject;
    }

    protected override void PushObj()
    {
        remote_Atk.SetActive(true);
        base.PushObj();
    }
    protected override void StopMoveStartAtk()
    {
        base.StopMoveStartAtk();
        remote_Atk.SetActive(false);
    }

   
}
