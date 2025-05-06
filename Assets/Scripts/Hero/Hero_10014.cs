using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero_10014 : HeroBase
{
    public override HeroType Type => HeroType.Hero_10014;


    private int bulletFew = 4;// �����ӵ�����
    private int bulletFewbuff;// �ӳɺ���ӵ�����
    private float anglePerBullet = 9f;// ÿ����һ���ӵ����ӵĽǶ�
    private float positionOffset = 0.2f;// λ��ƫ��ϵ��

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

        Debug.Log($"�ȼ� {level} ,�ӵ����� {bulletFewbuff}");

        int bulletCount = bulletFewbuff;
        Vector2 baseDirection = (target.position - transform.position).normalized;

        // ��̬���������νǶ� = �ӵ����� �� ÿ���Ƕ�
        float totalSpreadAngle = bulletCount * anglePerBullet;

        // ��ȡ���ηֲ��Ƕ�
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

                //----- ������� -----
                Vector2 rotatedDir = GetRotatedDirection(baseDirection, spreadAngles[index]);

                //----- λ��ƫ�� -----
                Vector2 perpendicularDir = new Vector2(-baseDirection.y, baseDirection.x);
                float offset = positionOffset * (index - (bulletCount - 1) / 2f);
                Vector3 spawnPos = transform.position + (Vector3)(perpendicularDir * offset);

                //----- ��ת���� -----
                float angle = Vector2.SignedAngle(Vector2.up, rotatedDir);
                Quaternion rotation = Quaternion.Euler(0, 0, angle);

                //----- ��ʼ���ӵ� -----
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
    /// ��̬���νǶȷֲ�
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
    /// ������ת
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