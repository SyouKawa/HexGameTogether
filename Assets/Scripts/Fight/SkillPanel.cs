using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 下面控制类的一个子图片单元
/// </summary>
[PrefabPath("Prefabs/Fight/ImageHUD")]
public class SkillReferenceItemIcon : PrefabBinding {
    public Image image;
    public SkillReferenceItemIcon() {
        image = Source.GetComponent<Image>();
    }
}

/// <summary>
/// 技能参考面板的单个对象控制类. 
/// </summary>
[PrefabPath("Prefabs/Fight/SkillHUD")]
public class SkillReferenceItem : PrefabBinding {
    public Text text;
    public Transform abiliTrans;
    private List<SkillReferenceItemIcon> icons;

    public SkillReferenceItem(Skill sourceSkill) {
        icons = new List<SkillReferenceItemIcon>();

        abiliTrans = Source.transform.GetChild(0);
        text = Source.transform.GetChild(1).GetComponent<Text>();

        StringBuilder sb = new StringBuilder();
        sb.AppendLine(sourceSkill.name);
        sb.AppendLine(sourceSkill.discrition);
        text.text = sb.ToString();

        foreach (DiceSide side in sourceSkill.needSides) {
            SkillReferenceItemIcon icon = new SkillReferenceItemIcon();
            icons.Add(icon);
            icon.image.sprite = BattleMode.Instance.LoadSprite(side);
            icon.Source.transform.SetParent(abiliTrans);
        }
    }

    /// <summary>
    /// 一个典型的例子. 它要先释放自己管理的子对象,然后再释放自己
    /// </summary>
    public override void _Delete() {
        foreach (SkillReferenceItemIcon icon in icons) {
            icon._Delete();
        }
        base._Delete();
    }

}
