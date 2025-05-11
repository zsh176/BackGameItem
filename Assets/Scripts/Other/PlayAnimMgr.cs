using DG.Tweening.Core.Easing;
using DragonBones;
using Spine.Unity;
using UnityEngine;

/// <summary>
/// ͳһ�����Ŷ���
/// </summary>
public class PlayAnimMgr : BaseManager<PlayAnimMgr>
{
    private UnityEngine.Transform animBase;
    void Start()
    {
        animBase = UIManager.Instance.GetLayer(E_UI_Layer.Top);
    }

    /// <summary>
    /// ���Ž�ɫ��������
    /// </summary>
    /// <param name="name">����Ԥ������</param>
    /// <param name="compname">������Ƭ��</param>
    /// <param name="pos">������ʾλ��</param>
    public void PlayUpLevelAnim(string name, string compname , Vector3 pos)
    {
        AddressablesMgr.Instance.LoadAssetAsync<GameObject>(obj =>
        {
            GameObject anim = Instantiate(obj);
            anim.transform.SetParent(animBase, false);
            anim.transform.position = pos;
            anim.transform.localScale = Vector3.one;
            UnityArmatureComponent armatureComp = anim.transform.Find("Armature").GetComponent<UnityArmatureComponent>();
            armatureComp.animation.Play(compname, 1);
            armatureComp.AddEventListener(EventObject.COMPLETE, (type, listener) =>
            {
                //print("����������ɣ����ٻ���뻺���");
                //����������ɣ����ٻ���뻺���
                Destroy(anim);
            });
        }, name, StaticFields.AnimTag);
    }

    /// <summary>
    /// ���Ŵ����ʧ����
    /// </summary>
    public void PlayBullePushAnim(Vector3 pos , UnityEngine.Transform parent)
    {
        PoolMgr.Instance.GetObj(obj =>
        {
            //���ø�����
            obj.transform.SetParent(parent, false);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = pos;
            //���궯������
            obj.transform.Find("spine_Anim").GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "duang", false).Complete += trackEntry =>
            {
                PoolMgr.Instance.PushObj(obj);
            };

        }, "Bullet_10005_PushAnim", StaticFields.AnimTag);
    }

    /// <summary>
    /// ��С���ӵ���ը����
    /// </summary>
    public void PlayBulleExplosionAnim(Vector3 pos, UnityEngine.Transform parent)
    {
        PoolMgr.Instance.GetObj(obj =>
        {
            //���ø�����
            obj.transform.SetParent(parent, false);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = pos;
            //���궯������
            obj.transform.Find("spine_Anim").GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "boom_yh", false).Complete += trackEntry =>
            {
                PoolMgr.Instance.PushObj(obj);
            };
        }, "Bullet_10004_ExplosionAnim", StaticFields.AnimTag);
    }


    /// <summary>
    /// ��ʾ����
    /// </summary>
    /// <param name="str"></param>
    /// <param name="time"></param>
    public void ShowTips(string str, float time = 1.2f)
    {
        PoolMgr.Instance.GetObj("PopUpTips", obj =>
        {
            obj.transform.SetParent(animBase, false);
            obj.GetComponent<PopUpTips>().ShowTips(str, time);
        });
    }
}
