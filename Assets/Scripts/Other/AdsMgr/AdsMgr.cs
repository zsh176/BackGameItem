using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsMgr : InstanceMgr<AdsMgr>
{
    
    public void ShowRewardedVideoAd(Action click_success)
    {
        Debug.Log("������");
        click_success?.Invoke();
    }
}
