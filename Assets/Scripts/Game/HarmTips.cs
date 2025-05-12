using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// ÉËº¦ÌáÊ¾
/// </summary>
public class HarmTips : MonoBehaviour
{
    private TextMeshProUGUI txt;//ÆÕÍ¨
    private Transform strikeicon;//±©»÷
    private TextMeshProUGUI txt_Strike;//±©»÷

    private void Awake()
    {
        txt = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        strikeicon = transform.Find("Strikeicon");
        txt_Strike = transform.Find("Strikeicon/Text").GetComponent<TextMeshProUGUI>();
    }

    public void ShowTips(int value, Vector3 initPos, bool isStrike = false)
    {
        transform.SetParent(UIManager.Instance.GetPanel<GamePanel>().GetBullet_Base());
        transform.localPosition = initPos;
        transform.localScale = Vector3.one * 2;
        if (isStrike)
        {
            strikeicon.gameObject.SetActive(true);
            txt_Strike.text = $"-{value}";
            txt.gameObject.SetActive(false);
        }
        else
        {
            strikeicon.gameObject.SetActive(false);
            txt.gameObject.SetActive(true);
            txt.text = $"-{value}";
        }
        transform.DOLocalMoveY(transform.localPosition.y + 140, 0.8f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            PoolMgr.Instance.PushObj(gameObject);
        });
    }

}
