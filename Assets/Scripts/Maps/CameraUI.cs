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
        //激活节点
        Transform.SetParent(Global.Instance.transform);
        //初始化Var
        PreviewBar = Nodes["PreviewSupply"].GetComponent<Image>();
        SupplyBar = Nodes["Supply"].GetComponent<Image>();
        //初始满补给槽
        PreviewBar.fillAmount = 1f;
        SupplyBar.fillAmount = 1f;
    }
    //一旦开始寻路是后面的颜色变浅，前面的颜色不变。
    public void PreviewSupply(int minusSupply){
        int curSupply = MapManager.GetInstance().player.supply;
        int previewSupply = curSupply - minusSupply;
        Nodes["SupplyText"].GetComponent<Text>().text = curSupply+"-> "+previewSupply+"/"+GameData.MaxSupply;
        Debug.Log("pre = " + previewSupply);
        PreviewBar.fillAmount = (float)previewSupply/(float)GameData.MaxSupply;
        Debug.Log("percent:"+SupplyBar.fillAmount);
    }

}
