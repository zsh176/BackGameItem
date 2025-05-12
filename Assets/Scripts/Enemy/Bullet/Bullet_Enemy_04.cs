using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����04 �ı��� �ӵ�
/// </summary>
public class Bullet_Enemy_04 : MonoBehaviour
{
    protected float moveSpeed;//�ƶ��ٶ�
    protected int atkValue;//������
    private Transform icon;
    private SkeletonGraphic spineAnim;//��������
    private bool isAtk;//�Ƿ�ʼ������

    private void Awake()
    {
        icon = transform.Find("icon");
        spineAnim = transform.Find("spine_Anim").GetComponent<SkeletonGraphic>();
        moveSpeed = 27;
    }

    public void Init(int initatkValue,Vector3 initPos)
    {
        transform.SetParent(UIManager.Instance.GetPanel<GamePanel>().GetBullet_Base());
        transform.localScale = Vector3.one;
        
        atkValue = initatkValue;
        transform.position = initPos;
        isAtk = false;
        icon.gameObject.SetActive(true);
        spineAnim.gameObject.SetActive(false);
        //��ȡ������λ��
        Transform mapHero = UIManager.Instance.GetPanel<GamePanel>().GetmapHeroBG();
        Vector3 dir = (mapHero.position - transform.position).normalized;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, dir);
        //Y�ᳯ�������
        transform.localRotation = rotation;
    }
    private void Attack()
    {
        isAtk = true;
        icon.gameObject.SetActive(false);
        spineAnim.gameObject.SetActive(true);

        //�����¼�����ұ�����
        BeAtkData data = new BeAtkData()
        {
            harm = atkValue,
            pos = transform.position,
        };
        EventMgr.Instance.EventTrigger<BeAtkData>(E_EventType.playerBeAtk, data);

        spineAnim.AnimationState.SetAnimation(0, "boom", false).Complete += trackEntry =>
        {
            PoolMgr.Instance.PushObj(gameObject);
        };
    }
    void Update()
    {
        if (isAtk) return;
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.Self);
        //�ñ���һֱ��ת
        icon.Rotate(Vector3.forward * 240 * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag(StaticFields.MapHeroBGTag))
        {
            if (isAtk) return;
            Attack();
        }
    }

}
