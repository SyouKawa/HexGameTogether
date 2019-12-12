using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 每一个角色都是由骰子和技能组成的.
/// 装备和等级可以使这个角色获得额外的骰子或技能,每个角色可以选择的装备种类是受限的
/// </summary>
public class Character {

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
    public static BattleMode Instance;

    private void Awake() {
        Instance = this;

        Object[] sprites;
        sprites = Resources.LoadAll("hexGameIcon");
        for (int i = 1; i < sprites.Length; i++) {
            diceSidesSprites.Add((Sprite)sprites[i]);
        }
    }

    private List<Sprite> diceSidesSprites = new List<Sprite>();
    public Sprite LoadSprite(DiceSides side) {
        return diceSidesSprites[(int)side];
    }




}
