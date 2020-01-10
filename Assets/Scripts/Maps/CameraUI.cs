using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[PrefabPath("Prefabs/UI/MainUI")]
public class HUD : ObjectBinding
{
    public Image PreviewBar;
    public Image SupplyBar;

    public HUD(){
        //激活节点
        Transform.SetParent(Global.Instance.transform);
        //初始化Var
        PreviewBar = Find("PreviewSupply").GetComponent<Image>();
        SupplyBar = Find("Supply").GetComponent<Image>();
        //初始满补给槽
        PreviewBar.fillAmount = 1f;
        SupplyBar.fillAmount = 1f;
    }
    //一旦开始寻路是后面的颜色变浅，前面的颜色不变。
    public void PreviewSupply(int minusSupply){
        int curSupply = MapManager.Instance.playerInMap.supply;
        int previewSupply = curSupply - minusSupply;
        Find("SupplyText").GetComponent<Text>().text = curSupply+"-> "+previewSupply+"/"+GameData.MaxSupply;
        Debug.Log("pre = " + previewSupply);
        PreviewBar.fillAmount = (float)previewSupply/(float)GameData.MaxSupply;
        Debug.Log("percent:"+SupplyBar.fillAmount);
    }

}
