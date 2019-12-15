using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 骰子可以投出的面的种类
/// 目前对应的贴图和spritesheet的顺序是一样的.
/// </summary>
public enum DiceSides {
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
    
    灰工程路障,
    灰火炮,
    灰重力锤,
    灰空,

    炮弹,
    建材,
}


/// <summary>
/// 骰子 包含六个面
/// </summary>
public class Dice {
    public List<DiceSides> sides = new List<DiceSides>();

    public Dice() { }

    public Dice(DiceSides t) {
        AddSide(t);
    }

    public Dice(DiceSides t, int count) {
        AddSide(t, count);
    }

    /// <summary>
    /// 添加指定个某面
    /// </summary>
    public Dice AddSide(DiceSides t, int count) {
        //溢出无条件返回
        if (count + sides.Count > 6)
            return this;
        for (int i = 0; i < count; i++) {
            sides.Add(t);
        }
        return this;
    }

    /// <summary>
    /// 使用某面填充剩余面
    /// </summary>
    public Dice AddSide(DiceSides t) {
        int count = 6 - sides.Count;

        for (int i = 0; i < count; i++) {
            sides.Add(t);
        }
        return this;
    }

    /// <summary>
    /// 返回投掷的结果
    /// </summary>
    public DiceSides GetResult() {
        return sides[UnityEngine.Random.Range(0, 5)];
    }

    /*
     * 己方骰子组
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
        Dice energyHalf = new Dice(DiceSides.紫能量I, 3).AddSide(DiceSides.紫空);
        Dice machinHalf = new Dice(DiceSides.黄稳定, 2).AddSide(DiceSides.黄迅速, 2).AddSide(DiceSides.黄空);
        Dice intHalf = new Dice(DiceSides.蓝智慧, 2).AddSide(DiceSides.蓝观察, 2).AddSide(DiceSides.蓝空);
        result.Add(energyHalf);
        result.Add(energyHalf);
        result.Add(machinHalf);
        result.Add(machinHalf);
        result.Add(intHalf);

        result.Add(new Dice(DiceSides.灰工程路障, 4).AddSide(DiceSides.灰空));
        result.Add(new Dice(DiceSides.灰火炮, 4).AddSide(DiceSides.灰空));
        result.Add(new Dice(DiceSides.灰火炮, 4).AddSide(DiceSides.灰空));

        result.Add(new Dice(DiceSides.炮弹));
        result.Add(new Dice(DiceSides.建材));

        return result;
    }
}


