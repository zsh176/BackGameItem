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
    public void Init(Vector3 initPos, Quaternion initRotation, int atkvalus,RectTransform initsceneMapBG, bool isSkill = false)
    {
        transform.position = initPos;
        transform.rotation = initRotation;
        atkValue = atkvalus;
        mapBGheight = (initsceneMapBG.rect.height / 2) + 200;
        mapBGwidth = (initsceneMapBG.rect.width / 2) + 200;
        ordinary.SetActive(!isSkill);
        skill.SetActive(isSkill);
        isOver = false;
    }
}
