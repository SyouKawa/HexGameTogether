using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 一个暂定的Unity对象控制类的模板
/// 这个模板不可以直接被实例化,一定要被继承. 绑定的数据类型是子类的类型
/// API暂定,待整理
/// </summary>
public abstract class ObjectBinding {
    /// <summary>
    /// Unity对象的数据源
    /// </summary>
    public GameObject Source { get; set; }
    /// <summary>
    /// Unity Transform
    /// </summary>
    public Transform Transform { get => Source.transform; }

    /// <summary>
    /// 构造的时候从对象池获取数据源
    /// </summary>
    public ObjectBinding() {
        Source = ObjectManager.GetInstantiate(this);
    }

    /// <summary>
    /// 删除的时候还回数据源 一定要手动释放对象
    /// </summary>
    public virtual void _Delete() {
        ObjectManager.ReturnInstantiate(Source, GetType());
    }

    /// <summary>
    /// 释放一个列表
    /// </summary>
    public static void DeleteList<T>(List<T> list) where T : ObjectBinding {
        foreach (T obj in list) {
            obj._Delete();
        }
        list.Clear();
    }
}

/// <summary>
/// 下面控制类的一个子图片单元
/// </summary>
[AddPool("Prefabs/Fight/ImageHUD")]
public class SkillReferenceItemIcon : ObjectBinding {
    public Image image;
    public SkillReferenceItemIcon() {
        image = Source.GetComponent<Image>();
    }
}

/// <summary>
/// 技能参考面板的单个对象控制类. 
/// </summary>
[AddPool("Prefabs/Fight/SkillHUD")]
public class SkillReferenceItem : ObjectBinding {
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
