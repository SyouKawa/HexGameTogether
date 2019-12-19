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
public class BattleMode : MonoBehaviour {
    public int PlayerHP = 10;
    public int EnemyHP = 10;
    public int PlayerShield = 0;

    /// <summary>
    /// 技能释放器 用于进行技能结算辅助
    /// </summary>
    public SkillCaster Caster;

    /// <summary>
    /// 所有可供释放的技能
    /// </summary>
    public List<Skill> TotalSkills;

    /// <summary>
    /// 角色拥有的骰子组
    /// </summary>
    public List<Dice> BeginSides;
    
    /// <summary>
    /// 当前可选择骰子库,可随意重投
    /// </summary>
    public List<Dice> Dices;


    public int UnSelectedSidesCount = 0;

    private void Update() {
        UnSelectedSidesCount = Dices.Count;
    }

    /// <summary>
    /// 当选择一个骰子时触发的事件
    /// </summary>
    public System.Action OnChangeDiceEvent;

    public System.Action OnBattleStartEvent;
    
    public Transform SkillPanelTrans;


    public static BattleMode Instance;
    private void Awake() {
        Instance = this;

        LoadIcon();

        Caster = new SkillCaster();
        //角色拥有的测试技能
        TotalSkills = GetTestSkills();

    }

    private void Start() {

        //拥有的测试骰子
        BeginSides = GetTestDices();

        Dices = new List<Dice>();
        Dices.AddRange(BeginSides);

        OnChangeDiceEvent?.Invoke();

        foreach (Dice dice in Dices) {
            dice.Random();
        }

        OnBattleStartEvent.Invoke();
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
    public Sprite LoadSprite(DiceSide side) {
        return diceSidesSprites[(int)side];
    }
    #endregion

    public void InitBattle() {
    }

    #region 测试用例
    /* 技能组
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
    public static List<Skill> GetTestSkills() {
        List<Skill> result = new List<Skill>() {
            new Skill() {
                name = "火炮开火",
                discrition = "2伤 炮+弹",
                needSides = new List<DiceSide>() { DiceSide.灰火炮, DiceSide.绿炮弹 },
                logic = "Attack(2);"
            },
            new Skill() {
                name = "迅速射击",
                discrition = "2伤 炮+弹+红技巧 射击后获得炮*1",
                needSides = new List<DiceSide>() { DiceSide.灰火炮, DiceSide.绿炮弹, DiceSide.红技巧},
                logic = "Attack(2); AddDice();"
            },
            new Skill() {
                name = "强力射击",
                discrition = "4伤 炮+弹+能量1",
                needSides = new List<DiceSide>() { DiceSide.灰火炮, DiceSide.绿炮弹, DiceSide.紫能量I},
                logic = "Attack(4);"
            },
            new Skill() {
                name = "爆炸射击",
                discrition = "4伤 炮+弹+红力量",
                needSides = new List<DiceSide>() { DiceSide.灰火炮, DiceSide.绿炮弹, DiceSide.红力量},
                logic = "Attack(4);"
            },
            new Skill() {
                name = "精准射击",
                discrition = "2伤 炮+黄稳定",
                needSides = new List<DiceSide>() { DiceSide.灰火炮, DiceSide.黄稳定},
                logic = "Attack(2);"
            },
            new Skill() {
                name = "脆弱路障",
                discrition = "1盾 路障",
                needSides = new List<DiceSide>() {  DiceSide.灰工程路障},
                logic = "Shield(1);"
            },
            new Skill() {
                name = "能量屏障",
                discrition = "2盾 路障+能量1",
                needSides = new List<DiceSide>() {  DiceSide.灰工程路障, DiceSide.紫能量I},
                logic = "Shield(2);"
            },
            new Skill() {
                name = "强效路障",
                discrition = "路障+建材",
                needSides = new List<DiceSide>() {  DiceSide.灰工程路障, DiceSide.绿建材},
                logic = "Shield(4);"
            },
            new Skill() {
                name = "坚固路障",
                discrition = "3盾 路障+黄迅速+蓝观察",
                needSides = new List<DiceSide>() {  DiceSide.灰工程路障, DiceSide.黄迅速, DiceSide.蓝观察},
                logic = "Shield(3);"
            }
        };

        return result;
    }


    /* 己方骰子组
    * 紫色半满 * 2 (3 0)
    * 黄色半满 * 2 (2 2 0)
    * 蓝色半满 * 1
    * 炮 * 2 (4 0)
    * 路障 * 1 (4 0)
    * 弹药 * 1 (6)
    * 建材 * 1 (6)
   */
    public static List<Dice> GetTestDices() {

        List<Dice> result = new List<Dice>();
        Dice EnergyHalf() => new Dice(DiceSide.紫能量I, 3).AddSide(DiceSide.紫空);
        Dice MachinHalf() => new Dice(DiceSide.黄稳定, 2).AddSide(DiceSide.黄迅速, 2).AddSide(DiceSide.黄空);
        Dice IntHalf() => new Dice(DiceSide.蓝智慧, 2).AddSide(DiceSide.蓝观察, 2).AddSide(DiceSide.蓝空);

        result.Add(EnergyHalf());
        result.Add(EnergyHalf());
        result.Add(MachinHalf());
        result.Add(MachinHalf());
        result.Add(IntHalf());

        result.Add(new Dice(DiceSide.灰工程路障, 4).AddSide(DiceSide.灰空));
        result.Add(new Dice(DiceSide.灰火炮, 4).AddSide(DiceSide.灰空));
        result.Add(new Dice(DiceSide.灰火炮, 4).AddSide(DiceSide.灰空));

        result.Add(new Dice(DiceSide.绿炮弹));
        result.Add(new Dice(DiceSide.绿建材));

        foreach (Dice dice in result) {
            dice.CreateGameObj();
        }

        return result;
    }
    #endregion
}
