using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 食堂阿姨
/// </summary>
public class Hero_10006 : HeroBase
{
    public override HeroType Type => HeroType.Hero_10006;

    /// <summary>
    /// 增加的最大血量
    /// </summary>
    [HideInInspector]
    public int[] addHPValue;

    protected override void Start()
    {
        base.Start();
        timeAtkCooling = atkCooling;
        addHPValue = new int[4] { 100, 150, 225, 337 };
    }

    protected override void IsOkAtk()
    {
        Attack(null);
    }
    protected override void Attack(Transform target)
    {
        EventMgr.Instance.EventTrigger<int>(E_EventType.addHP, atkValueBuff);
        spineAnim.AnimationState.SetAnimation(0, HeroAnimSpineTag.atk, false);
        spineAnim.AnimationState.AddAnimation(0, HeroAnimSpineTag.stand, true, 0f);

        timeAtkCooling = atkCooling;
    }
    protected override void UpLevel()
    {
        base.UpLevel();
        EventMgr.Instance.EventTrigger<Hero_10006>(E_EventType.upHero10006, this);
    }

}
