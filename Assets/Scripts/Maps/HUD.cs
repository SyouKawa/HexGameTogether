using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[PrefabPath("Prefabs/UI/MainUI")]
public class HUD : ObjectBinding
{
    public Image RealSupply;
    public Image EffectSupply;

    public HUD(){
        //激活节点
        Transform.SetParent(Global.Instance.transform);
        //初始化Var
        RealSupply = Find("Supply").GetComponent<Image>();
        EffectSupply = Find("EffectSupply").GetComponent<Image>();
        //初始满补给槽
        EffectSupply.fillAmount = 1f;
        RealSupply.fillAmount = 1f;
        //
        Find("SupplyText").GetComponent<Text>().text = "100/100";
    }
    //一旦开始寻路是后面的颜色变浅，前面的颜色不变。
    public void PreviewSupply(int costSupply){
        int curSupply = MapManager.Instance.playerInMap.supply;
        int previewSupply = curSupply - costSupply;
        Find("SupplyText").GetComponent<Text>().text = curSupply+"-> "+previewSupply+"/"+GameData.MaxSupply;
        //Debug.Log("pre = " + previewSupply);
        RealSupply.fillAmount = (float)previewSupply/(float)GameData.MaxSupply;
        //Debug.Log("percent:"+SupplyBar.fillAmount);
    }

    /// <summary>
    /// 获取缓慢减少Supply的速度
    /// </summary>
    public float GetReduceNum(bool isX,FpResult result){
        //当前消耗占fillCount的值
        float sumPer = (float)result.Sumcost/GameData.MaxSupply;
        //每帧减少的fill的速度
        float num=0;
        //是X轴斜向移动消耗帧数23，Y轴纵向移动消耗帧数13,获得平均速度
        if(isX){
            num = sumPer/(GameData.obliqueFrames*(result.Path.Count-1));
        }else{
            num = sumPer/(GameData.verticalFrames*(result.Path.Count-1));
        }
        return num;
    }

    public void ReduceEffectSupply(float cost){
        EffectSupply.fillAmount -= cost;
    }

    /// <summary>
    /// 返回是否需要减少缓动效果
    /// </summary>
    public bool NeedReduceEffect(){
        return RealSupply.fillAmount < EffectSupply.fillAmount ;
    }

}
