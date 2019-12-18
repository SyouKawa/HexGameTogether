using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStaticData{
    public const float Rates = 10f;
    public const float MaxOuterRadius = 1.7f * Rates;//最大外径
    public const float MinInnerRadius = 0.68f * Rates;//最小内径
    public const float ConstHorizonDis = 2.2f * Rates;//固定水平间距

    public const int EdgeNum = 6;//Tile环绕边数
    public const float infinite = 99999f;
    public static System.Random random = new System.Random(1);

    public static Dictionary<string, float> FieldGenRate = new Dictionary<string, float> {
        {"Moun",30},{"Lake",10},{"Forest",60},{"Plain",100}
     };


    /// <summary>
    /// 地形分类
    /// </summary>
    public enum FieldType {
        Mountain, //山地
        Lake, //湖
        Forest, // 森林
        EdgeSea, //边境海(将地形环绕一周,非行动区)
        Plain // 平原
     }
}
