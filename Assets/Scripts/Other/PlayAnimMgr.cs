using DG.Tweening.Core.Easing;
using DragonBones;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 统一管理播放动画
/// </summary>
public class PlayAnimMgr : BaseManager<PlayAnimMgr>
{
    private UnityEngine.Transform animBase;
    void Start()
    {
        animBase = UIManager.Instance.GetLayer(E_UI_Layer.Top);
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="name">动画预制体名</param>
    /// <param name="compname">动画切片名</param>
    /// <param name="pos">动画显示位置</param>
    public void PlayAnim(string name, string compname , Vector3 pos)
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
                //print("动画播放完成，销毁或放入缓存池");
                //动画播放完成，销毁或放入缓存池
                Destroy(anim);
            });
        }, name, StaticFields.AnimTag);
    }

    /// <summary>
    /// 提示弹窗
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
