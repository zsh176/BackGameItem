using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero_10014 : HeroBase
{
    public override HeroType Type => HeroType.Hero_10014;


    private int bulletFew = 4;// 基础子弹数量
    private int bulletFewbuff;// 加成后的子弹数量
    private float anglePerBullet = 9f;// 每增加一发子弹增加的角度
    private float positionOffset = 0.2f;// 位置偏移系数

    protected override void Start()
    {
        base.Start();
        bulletFewbuff = bulletFew + level;
    }
    protected override void UpLevel()
    {
        base.UpLevel();
        bulletFewbuff = bulletFew + level;
    }

    protected override void Attack(Transform target)
    {
        base.Attack(target);

        Debug.Log($"等级 {level} ,子弹数量 {bulletFewbuff}");

        int bulletCount = bulletFewbuff;
        Vector2 baseDirection = (target.position - transform.position).normalized;

        // 动态计算总扇形角度 = 子弹数量 × 每发角度
        float totalSpreadAngle = bulletCount * anglePerBullet;

        // 获取扇形分布角度
        float[] spreadAngles = GetSpreadAngles(
            bulletCount,
            totalSpreadAngle,
            startAngleOffset: 0
        );

        for (int i = 0; i < bulletCount; i++)
        {
            int index = i;

            PoolMgr.Instance.GetObj(obj =>
            {
                obj.transform.SetParent(bullet_Base, false);

                //----- 方向计算 -----
                Vector2 rotatedDir = GetRotatedDirection(baseDirection, spreadAngles[index]);

                //----- 位置偏移 -----
                Vector2 perpendicularDir = new Vector2(-baseDirection.y, baseDirection.x);
                float offset = positionOffset * (index - (bulletCount - 1) / 2f);
                Vector3 spawnPos = transform.position + (Vector3)(perpendicularDir * offset);

                //----- 旋转计算 -----
                float angle = Vector2.SignedAngle(Vector2.up, rotatedDir);
                Quaternion rotation = Quaternion.Euler(0, 0, angle);

                //----- 初始化子弹 -----
                obj.GetComponent<BulletBase>().Init(
                    spawnPos,
                    rotation,
                    atkValueBuff,
                    sceneMapBG
                );

            }, bulletName, StaticFields.Bullet);
        }
    }

    /// <summary>
    /// 动态扇形角度分布
    /// </summary>
    public static float[] GetSpreadAngles(int bulletCount, float totalSpreadAngle, float startAngleOffset = 0f)
    {
        if (bulletCount <= 0) return new float[0];
        if (bulletCount == 1) return new float[] { startAngleOffset };

        float[] angles = new float[bulletCount];
        float angleStep = totalSpreadAngle / (bulletCount - 1);
        float startAngle = -totalSpreadAngle / 2 + startAngleOffset;

        for (int i = 0; i < bulletCount; i++)
        {
            angles[i] = startAngle + angleStep * i;
        }

        return angles;
    }

    /// <summary>
    /// 方向旋转
    /// </summary>
    public static Vector2 GetRotatedDirection(Vector2 baseDirection, float rotateAngle)
    {
        float radian = rotateAngle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radian);
        float sin = Mathf.Sin(radian);
        return new Vector2(
            baseDirection.x * cos - baseDirection.y * sin,
            baseDirection.x * sin + baseDirection.y * cos
        );
    }
}