using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[PrefabPath("Prefabs/UI/MainUI")]
public class CameraUI : ObjectBinding
{
    public Image PreviewBar;
    public Image SupplyBar;

    public CameraUI(){
        //初始化满体力
        PreviewBar.fillAmount = 1f;
        SupplyBar.fillAmount = 1f;
    }

}
