using System.Collections.Generic;
using UnityEngine;

/*
 * 点击骰子会将其移动到目标区 
 * 当目标区>=1时剩余的骰子会显示为绿色和红色
 * 点击目标区骰子会将其移动回来
 * 点击释放按钮释放技能,
 * 点击结束回合按钮结束回合
 */



/// <summary>
/// 用于管理骰子的位置和方向
/// </summary>
public class DiceTable : MonoBehaviour {


    private BattleMode battle;
    public void SetDices() {
        List<Dice> unselect = new List<Dice>();
        List<Dice> select = new List<Dice>();

        foreach(var dice in battle.Dices) {
            if(dice.SelectType == Dice.Type.Selected) {
                select.Add(dice);
            }
            else {
                unselect.Add(dice);
            }
        }

        int i = 0;
        foreach (var dice in unselect) {
            Transform trans = dice.obj.transform;
            trans.SetParent(transform);
            trans.localPosition = new Vector3(i, 0, 0);
            i++;
        }

        i = 0;
        foreach (var dice in select) {
            Transform trans = dice.obj.transform;
            trans.SetParent(transform);
            trans.localPosition = new Vector3(i, -1, 0);
            i++;
        }
    }

    private void Start() {
        battle = BattleMode.Instance;
        battle.OnChangeDiceEvent += SetDices;
    }

    private void Update() {
        if(Input.GetMouseButtonDown(0)) {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null) {
                Dice hitDice = ObjectManager.GetClass<Dice>(hit.collider.gameObject);
                hitDice.SelectType = Dice.Type.Selected;
                Debug.Log(hitDice.TopSide.ToString());
                battle.OnChangeDiceEvent.Invoke();
            }
        }


    }


}
