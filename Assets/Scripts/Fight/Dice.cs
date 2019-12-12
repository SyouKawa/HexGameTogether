﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 骰子可以投出的面的种类
/// 目前对应的贴图和spritesheet的顺序是一样的.
/// </summary>
public enum DiceSides {
    紫能量I,
    紫能量II,
    紫空,

    红力量,
    红技巧,
    红空,

    蓝智慧,
    蓝观察,
    蓝空,

    黄空,
    黄稳定,
    黄迅速,
    
    工程路障,
    火炮,
    重力锤,
    炮弹,
    建材,
}

/// <summary>
/// 人物可以组合出的技能,通常技能都是伴随人物
/// 根据人物的特性从而可以进行组合 释放技能时很多时候都会消耗消耗品
/// </summary>
public class Skill {
    /// <summary>
    /// 名字
    /// </summary>
    public string name;
    /// <summary>
    /// 描述
    /// </summary>
    public string discrition;
    /// <summary>
    /// 释放的前提条件
    /// </summary>
    public List<DiceSides> needDices;
    /// <summary>
    /// 释放后发生的事情
    /// </summary>
    public virtual void Use() {

    }
}

//火炮 + 能量1 + 弹药 可以释放强力射击
public class A1Skill : Skill {
    public A1Skill() {
        name = "精准射击";
        discrition = "强力射击";
        needDices = new List<DiceSides>() { DiceSides.火炮, DiceSides.紫能量I, DiceSides.炮弹 };
    }
    public override void Use() {

    }
}


/// <summary>
/// 精准的符合一定为1种或小于1种. 
/// 如果两个技能的前置条件完全一样,那么他们互斥,不可能同时出现,通常可以根据技能稀有度进行排序,只能携带更强的技能
/// </summary>
public class SkillSearchResult {
    //释放的结果 小于等于1种
    Skill Result;
    //可能参考的结果 都有可能
    List<Skill> Reference;
}

/// <summary>
/// 负责释放技能的类
/// </summary>
public class SkillCaster {

    /// <summary>
    /// 所有当前被选择的资源
    /// </summary>
    public List<DiceSides> SelectedSides;

    /// <summary>
    /// 所有可供释放的技能
    /// </summary>
    public List<Skill> TotalSkills;

    /// <summary>
    /// 根据当前已经选择的词条,可以释放的技能 
    /// </summary>
    public SkillSearchResult CanCastSkill() {



        SkillSearchResult result = new SkillSearchResult();
        return result;
    }
}
