using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
    public List<DiceSides> needSides;

    protected BattleMode battle;
    /// <summary>
    /// 释放后发生的事情
    /// </summary>
    public virtual void Use() {

    }
}
/*
 * 技能组
 * 火炮开火 2伤 炮+弹
 * 迅速射击 2伤 炮+弹+红技巧 射击后获得炮*1
 * 强力射击 4伤 炮+弹+能量1
 * 爆炸射击 4伤 炮+弹+红力量
 * 精准射击 2伤 炮+黄稳定
 * 脆弱路障 1盾 路障
 * 能量屏障 2盾 路障+能量1
 * 强效路障 4盾 路障+建材
 * 坚固路障 3盾 路障+黄迅速+蓝观察
*/

public class A1Skill : Skill {
    public A1Skill() {
        name = "火炮开火";
        discrition = "2伤 炮+弹";
        needSides = new List<DiceSides>() { DiceSides.灰火炮, DiceSides.炮弹 };
    }
    public override void Use() {
        battle.AttackEnemy(2);
    }
}

public class A2Skill : Skill {
    public A2Skill() {
        name = "迅速射击";
        discrition = "2伤 炮+弹+红技巧 射击后获得炮*1";
        needSides = new List<DiceSides>() { DiceSides.灰火炮, DiceSides.炮弹, DiceSides.红技巧};
    }
    public override void Use() {
        battle.AttackEnemy(2);
        battle.AddDice(new Dice(DiceSides.灰火炮, 4).AddSide(DiceSides.灰空));
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