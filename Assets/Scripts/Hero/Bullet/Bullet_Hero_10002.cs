using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 王东东的子弹
/// </summary>
public class Bullet_Hero_10002 : BulletBase
{
    private GameObject ordinary;//普通子弹
    private GameObject skill;

    protected override void Awake()
    {
        base.Awake();

        moveSpeed = 57;
        ordinary = transform.Find("Ordinary").gameObject;
        skill = transform.Find("Skill").gameObject;
    }
    public override void Init(Vector3 initPos, Quaternion initRotation, int atkvalus, RectTransform initsceneMapBG, bool initIsStrike)
    {
        transform.position = initPos;
        transform.rotation = initRotation;
        atkValue = atkvalus;
        mapBGheight = (initsceneMapBG.rect.height / 2) + 200;
        mapBGwidth = (initsceneMapBG.rect.width / 2) + 200;
        ordinary.SetActive(!initIsStrike);
        skill.SetActive(initIsStrike);
        isOver = false;
        isStrike = initIsStrike;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isOver) return;
        if (collision.CompareTag(StaticFields.Enemy))
        {
            isOver = true;
            //Debug.Log($"攻击伤害为：{atkValue}");
            enemys.Add(collision.transform);
            enemys[0].GetComponent<EnemyBase>().EnemyBeAtk(atkValue, isStrike);
            enemys.Clear();

            PoolMgr.Instance.PushObj(gameObject);
        }
    }
}
