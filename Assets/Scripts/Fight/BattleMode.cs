using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 每一个角色都是由骰子和技能组成的.
/// 装备和等级可以使这个角色获得额外的骰子或技能,每个角色可以选择的装备种类是受限的
/// </summary>
public class Character {
    public List<Dice> dices;
    public List<Skill> skills;

    public static Character CreateC1() {
        return new Character() {
            dices = new List<Dice>() { },
            skills = new List<Skill>() { }
        };
    }
}

/*
 * 己方场地空间:
 * 可用骰子组
 * 已选择骰子组
 * 可用技能组
 * 己方血条
 * 敌方血条
 * 
 * 当一场战斗开始时,将这个角色的技能组加入可用技能组
    当一个回合开始时,将这个角色的骰子加入初始骰子组
    一个回合结束时,所有骰子都被弃置.等待下一个回合开始
 */

/// <summary>
/// 
/// </summary>
public class BattleMode : MonoBehaviour
{
    public int PlayerHP = 10;
    public int EnemyHP = 10;
    public int PlayerShield = 0;

    public SkillCaster caster = new SkillCaster();

    /// <summary>
    /// 攻击敌人
    /// </summary>
    public void AttackEnemy(int damage) {
        EnemyHP -= damage;
    }
    /// <summary>
    /// 临时增加一个骰子
    /// </summary>
    public void AddDice(Dice dice) {

    }

    public static BattleMode Instance;
    private void Awake() {
        Instance = this;

        LoadIcon();

    }

    #region LoadIcon
    private List<Sprite> diceSidesSprites = new List<Sprite>();
    private void LoadIcon() {
        Object[] sprites;
        sprites = Resources.LoadAll("hexGameIcon");
        for (int i = 1; i < sprites.Length; i++) {
            diceSidesSprites.Add((Sprite)sprites[i]);
        }
    }
    public Sprite LoadSprite(DiceSides side) {
        return diceSidesSprites[(int)side];
    }
    #endregion

    public void InitBattle() {
    }



}
