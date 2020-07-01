using System.Collections.Generic;

/*
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
*/


/// <summary>
/// 一个用于逻辑判断的辅助容器
/// </summary>
public class SidesContainer {
    public Dictionary<DiceSide, int> sideDic = new Dictionary<DiceSide, int>();

    public void AddSide(DiceSide side) {
        if (sideDic.ContainsKey(side)) {
            sideDic[side]++;
        }
        else {
            sideDic.Add(side, 1);
        }
    }

    /// <summary>
    /// 从Skill中获得容器
    /// </summary>
    public static SidesContainer Parse(Skill skill) {
        SidesContainer container = new SidesContainer();
        foreach(var s in skill.needSides) {
            container.AddSide(s);
        }
        return container;
    }

    /// <summary>
    /// 从Dice列表中获得容器
    /// </summary>
    public static SidesContainer Parse(List<Dice> dices) {
        SidesContainer container = new SidesContainer();
        foreach (var dice in dices) {
            container.AddSide(dice.TopSide);
        }
        return container;
    }


    public enum Result {
        /// <summary>
        /// 二者完全相等
        /// </summary>
        Equal,
        /// <summary>
        /// A是B的子集
        /// </summary>
        Except,
        /// <summary>
        /// 二者没有包含关系
        /// </summary>
        Not,
    }

    /// <summary>
    /// 比较两个容器
    /// </summary>
    public static Result Conbine(SidesContainer A,SidesContainer B) {
        bool allEqualFlag = true;
        foreach(var side in A.sideDic.Keys) {
            //如果B中包含A的这个key
            if (B.sideDic.ContainsKey(side)) {
                //如果二者完全相同,那么继续
                if(A.sideDic[side] == B.sideDic[side]) {
                    continue;
                }
                //如果二者数量不同,B需求的比A提供的要多,那么这个会作为参考,可以继续执行
                else if(A.sideDic[side] < B.sideDic[side]) {
                    allEqualFlag = false;
                }
                //如果A提供的已经超过了B需求的,那么返回失败
                else {
                    return Result.Not;
                }
            }
            //如果B中就没有,说明这个不可能
            else {
                return Result.Not;
            }
        }
        //只有当A和B种类数完全相同,每个小项都完全吻合时,才能断定二者相等
        if(allEqualFlag == true && A.sideDic.Count == B.sideDic.Count) {
            return Result.Equal;
        }
        //如果能执行到这里那么这个可以作为参考
        else {
            return Result.Except;
        }
    }
}

/// <summary>
/// 负责释放技能的类
/// </summary>
public class SkillCaster {
    private readonly KSInterpreter<KSHelper> interpreter;

    public SkillCaster() {
        interpreter = new KSInterpreter<KSHelper>(new KSHelper());
    }

    /// <summary>
    /// 当前是否有技能可供释放
    /// </summary>
    public bool CanCast { get => PrepareSkill != null; }
    /// <summary>
    /// 当前等待释放的技能,为空说明没有技能可供释放
    /// </summary>
    public Skill PrepareSkill = null;
    /// <summary>
    /// 技能释放参考列表
    /// </summary>
    public List<Skill> ReferenceSkill = new List<Skill>();

    /// <summary>
    /// 在添加一个额外的骰子的情况下.
    /// 如果返回为true,说明可以选择这个骰子,并刷新参考列表
    /// 如果返回为false,说明不可以选择,不刷新参考列表
    /// </summary>
    public bool RefreshReference(Dice dice) {
        List<Dice> select = GetSelectedDices();
        select.Add(dice);
        return GetReference(select);
    }

    /// <summary>
    /// 刷新可供释放的技能列表
    /// </summary>
    /// <returns></returns>
    public bool RefreshReference() {
        return GetReference(GetSelectedDices());
    }

    /// <summary>
    /// 获取当前被选中的骰子
    /// </summary>
    private List<Dice> GetSelectedDices() {
        List<Dice> select = new List<Dice>();
        foreach (Dice d in BattleMode.Instance.Dices) {
            if (d.SelectType == Dice.Type.Selected) {
                select.Add(d);
            }
        }
        return select;
    }

    /// <summary>
    /// 通过比对参数列表和技能列表,从而改变参考技能组
    /// </summary>
    private bool GetReference(List<Dice> select) {
        List<Skill> reference = new List<Skill>();
        Skill prepare = null;

        SidesContainer A = SidesContainer.Parse(select);
        foreach (var skill in BattleMode.Instance.TotalSkills) {
            SidesContainer B = SidesContainer.Parse(skill);
            switch (SidesContainer.Conbine(A, B)) {
                case SidesContainer.Result.Equal:
                    prepare = skill;
                    break;
                case SidesContainer.Result.Except:
                    reference.Add(skill);
                    break;
                case SidesContainer.Result.Not:
                    break;
            }
        }

        if (prepare != null || reference.Count != 0) {
            PrepareSkill = prepare;
            ReferenceSkill = reference;
            return true;
        }
        return false;
    }
}

/// <summary>
/// 绑定KS脚本的执行类,KS脚本只能执行这个类中的方法
/// </summary>
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