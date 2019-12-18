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
    public List<DiceSide> needSides;
    /// <summary>
    /// 绑定的释放逻辑,暂时使用ks语言来执行
    /// </summary>
    public string logic;


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

    private KSInterpreter<KSHelper> interpreter;
    public SkillCaster() {
        interpreter = new KSInterpreter<KSHelper>(new KSHelper());
    }
    public SkillSearchResult Result;

    public void Select() {

    }

    /// <summary>
    /// 根据当前已经选择的词条,可以释放的技能 
    /// </summary>
    public SkillSearchResult CanCastSkill() {
        SkillSearchResult result = new SkillSearchResult();
        return result;
    }

    public void Cast(Skill skill) {
        interpreter.Run(skill.logic);
    }
}

public class KSHelper {
    private BattleMode battle;

    public KSHelper() {
        battle = BattleMode.Instance;
    }

    #region 技能的释放逻辑,用于供ks调用
    public void Attack(int damage) {
        battle.EnemyHP -= damage;
    }

    /// <summary>
    /// 临时增加一个骰子
    /// </summary>
    public void AddDice() {
        new Dice(DiceSide.灰火炮, 4).AddSide(DiceSide.灰空);
    }

    public void Shield(int shield) {
        battle.PlayerShield += shield;
    }
    #endregion
}