using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


/// <summary>
/// Ã· æµØ¥∞
/// </summary>
public class PopUpTips : MonoBehaviour
{
    private RectTransform tipRectTs;
    private TextMeshProUGUI descTxt;

    private void Awake()
    {
        tipRectTs = GetComponent<RectTransform>();
        descTxt = transform.Find("Text").GetComponent<TextMeshProUGUI>();
    }

    public void ShowTips(string str, float time = 1.2f)
    {
        tipRectTs.anchoredPosition = Vector2.zero;

        tipRectTs.DOLocalMoveY(tipRectTs.anchoredPosition.y + 300, time).OnComplete(() =>
        {
            PoolMgr.Instance.PushObj(gameObject);
        }).SetUpdate(true).SetDelay(1f);

        var size = tipRectTs.sizeDelta;
        size.x = str.Length * descTxt.fontSize + 40;
        tipRectTs.sizeDelta = size;

        descTxt.text = str;
    }
}
