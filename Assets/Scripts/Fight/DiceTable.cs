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
    private void Start() {
        battle = BattleMode.Instance;

        battle.OnBattleStartEvent += () => {
            battle.OnChangeDiceEvent += SetDicesTransform;
            battle.OnChangeDiceEvent += SetSkillReference;
            //先生成一波参考技能
            battle.Caster.RefreshReference();
            battle.OnChangeDiceEvent.Invoke();
        };
    }

    /// <summary>
    /// 将骰子放置在合适的位置上,绑定OnChangeDiceEvent
    /// </summary>
    public void SetDicesTransform() {
        List<Dice> unselect = new List<Dice>();
        List<Dice> select = new List<Dice>();

        foreach (Dice dice in battle.Dices) {
            if (dice.SelectType == Dice.Type.Selected) {
                select.Add(dice);
            }
            else {
                unselect.Add(dice);
            }
        }

        int i = 0;
        foreach (Dice dice in unselect) {
            Transform trans = dice.obj.Transform;
            trans.SetParent(transform);
            trans.localPosition = new Vector3(i, 0, 0);
            i++;
        }

        i = 0;
        foreach (Dice dice in select) {
            Transform trans = dice.obj.Transform;
            trans.SetParent(transform);
            trans.localPosition = new Vector3(i, -1, 0);
            i++;
        }
    }

    private List<SkillReferenceItem> objs = new List<SkillReferenceItem>();
    /// <summary>
    /// 改变技能参考面板的数据 绑定OnChangeDiceEvent
    /// </summary>
    public void SetSkillReference() {
        //foreach (SkillReferenceItem obj in objs) {
        //    obj._Delete();
        //}
        //objs.Clear();

        ObjectBinding.DeleteList(objs);

        if(battle.Caster.PrepareSkill != null) {
            SkillReferenceItem skillObj = new SkillReferenceItem(battle.Caster.PrepareSkill);
            skillObj.Source.transform.SetParent(battle.SkillPanelTrans);
            objs.Add(skillObj);
        }
        foreach (Skill skill in battle.Caster.ReferenceSkill) {
            SkillReferenceItem skillObj = new SkillReferenceItem(skill);
            skillObj.Source.transform.SetParent(battle.SkillPanelTrans);
            objs.Add(skillObj);
        }
    }

    /// <summary>
    /// 检测鼠标,当鼠标点击一个骰子时,触发改变
    /// </summary>
    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null) {
                Dice hitDice = ObjectBinding.GetClass<Dice.DiceObj>(hit.collider.gameObject).Dice;
                Selected(hitDice);
            }
        }
    }


    private void Selected(Dice dice) {
        //如果点击了一个没有被选中的骰子
        if (dice.SelectType != Dice.Type.Selected) {
            //如果之前没有被选中,那么检查带上它之后是否可以被选中
            if (battle.Caster.RefreshReference(dice)) {
                dice.SelectType = Dice.Type.Selected;
                Debug.Log("选中:"+dice.TopSide.ToString());
                battle.OnChangeDiceEvent.Invoke();
            }
            else {
                Debug.Log("选择失败:" + dice.TopSide.ToString());
            }
        }
        else {
            dice.SelectType = Dice.Type.Default;
            battle.Caster.RefreshReference();
            Debug.Log("放弃选择:" + dice.TopSide.ToString());
            battle.OnChangeDiceEvent.Invoke();
        }
    }


}
