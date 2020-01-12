using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

[PrefabPath("Prefabs/UI/MainUI")]
public class HUD : ExtendPrefabBinding
{
    public Image RealSupply;
    public Image EffectSupply;

    public enum TextMode{
        Preview,
        Normal
    }

    public HUD(){
        //激活节点
        Transform.SetParent(Global.Instance.transform);
        //初始化Var
        RealSupply = Find("SupplyBar.Supply").GetComponent<Image>();
        EffectSupply = Find("SupplyBar.EffectSupply").GetComponent<Image>();
        //初始满补给槽
        EffectSupply.fillAmount = 1f;
        RealSupply.fillAmount = 1f;
        //
        Find("SupplyBar.SupplyText").GetComponent<Text>().text = "100/100";
    }

    /// <summary>
    /// 可变参数为：[0]当前Supply [1]当前previewSupply(预览消耗后的Supply)
    /// </summary>
    public void SetSupplyText(TextMode mode,params int[] nums){
        
        Text shower = Find("SupplyBar.SupplyText").GetComponent<Text>();
        
        StringBuilder content = new StringBuilder();
        switch (mode){
            case TextMode.Preview:
                content.AppendFormat("{0}-> {1}/{2}",nums[0],nums[1],GameData.MaxSupply);
            break;
            case TextMode.Normal:
                content.AppendFormat("{0}/{1}",nums[0],GameData.MaxSupply);
            break;
        }
        shower.text = content.ToString();
    }
    public void PreviewSupply(int costSupply){
        int curSupply = MapManager.Instance.playerInMap.supply;
        int previewSupply = curSupply - costSupply;
        SetSupplyText(TextMode.Preview,curSupply,previewSupply);
        RealSupply.fillAmount = (float)previewSupply/(float)GameData.MaxSupply;
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
