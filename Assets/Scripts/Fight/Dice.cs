using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 骰子可以投出的面的种类
/// 目前对应的贴图和spritesheet的顺序是一样的.
/// </summary>
public enum DiceSide {
    紫能量I,
    紫能量II,
    紫空,

    红力量,
    红技巧,
    红空,

    蓝智慧,
    蓝观察,
    蓝空,

    黄空,
    黄稳定,
    黄迅速,

    灰空,
    灰工程路障,
    灰火炮,
    灰重力锤,

    绿空,
    绿炮弹,
    绿建材,
}


/// <summary>
/// 骰子 包含六个面
/// </summary>
public class Dice {
    [AddPool("Prefabs/Fight/DiceObj")]
    public class DiceObj : ObjectBinding {
        public Dice Dice;

        public SpriteRenderer image;
        public SpriteRenderer redImage;
        public SpriteRenderer greenImage;


        public DiceObj(Dice dice) {
            this.Dice = dice;
            redImage = Source.transform.GetChild(0).GetComponent<SpriteRenderer>();
            greenImage = Source.transform.GetChild(1).GetComponent<SpriteRenderer>();
            image = Source.transform.GetChild(2).GetComponent<SpriteRenderer>();
        }

    }

    public DiceObj obj;

    public void CreateGameObj() {
        obj = new DiceObj(this);
    }
    public void DeleteGameObj() {
        obj._Delete();
        obj = null;
    }

    private DiceSide topSide;
    /// <summary>
    /// 朝上生效的一个面,更改它会自动更改贴图
    /// </summary>
    public DiceSide TopSide {
        set {
            topSide = value;
            if (obj != null) {
                obj.image.sprite = BattleMode.Instance.LoadSprite(topSide);
            }
        }
        get => topSide;
    }


    public enum Type {
        Available,
        UnAvailable,
        Selected,
        Default
    }
    private Type selectType;
    /// <summary>
    /// 边框模式,更改它会自动更改边框
    /// </summary>
    public Type SelectType {
        get => selectType;
        set {
            selectType = value;
            if (obj == null) {
                return;
            }

            switch (selectType) {
                case Type.Available:
                    obj.redImage.enabled = false;
                    obj.greenImage.enabled = true;
                    break;
                case Type.UnAvailable:
                    obj.redImage.enabled = true;
                    obj.greenImage.enabled = false;
                    break;
                case Type.Default:
                    obj.redImage.enabled = false;
                    obj.greenImage.enabled = false;
                    break;
            }
        }
    }




    /// <summary>
    /// 重新投掷骰子 会改变TopSide的值
    /// </summary>
    public DiceSide Random() {
        if (sides.Count != 6) {
            Debug.LogError("骰子的面不为6 请检查错误");
            return TopSide;
        }
        TopSide = sides[UnityEngine.Random.Range(0, 5)];

        return TopSide;
    }

    #region 骰子的面和设置方法
    public List<DiceSide> sides = new List<DiceSide>();

    /// <summary>
    /// 禁止创建空骰子
    /// </summary>
    public Dice(DiceSide t) {
        AddSide(t);
        TopSide = sides[0];
    }

    public Dice(DiceSide t, int count) {
        AddSide(t, count);
        TopSide = sides[0];
    }

    /// <summary>
    /// 添加指定个某面
    /// </summary>
    public Dice AddSide(DiceSide t, int count) {
        //溢出无条件返回
        if (count + sides.Count > 6) {
            return this;
        }

        for (int i = 0; i < count; i++) {
            sides.Add(t);
        }
        return this;
    }

    /// <summary>
    /// 使用某面填充剩余面
    /// </summary>
    public Dice AddSide(DiceSide t) {
        int count = 6 - sides.Count;

        for (int i = 0; i < count; i++) {
            sides.Add(t);
        }
        return this;
    }

    #endregion


}


