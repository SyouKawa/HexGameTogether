using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData{

    public const int MaxSupply = 200;

    public const float Rates = 10f;
    public const float MaxOuterRadius = 1.7f * Rates;//最大外径
    public const float MinInnerRadius = 0.68f * Rates;//最小内径
    public const float ConstHorizonDis = 2.2f * Rates;//固定水平间距

    public const int EdgeNum = 6;//Tile环绕边数
    public const float infinite = 99999f;
    public static System.Random random = new System.Random(1);


    
    public static int MapWidth = 10;
    public static int MapHeight = 10;
    public static float DefaultCameraSize = 70f;
    public static float CameraRollSpeed = 5f;

    public static bool ShowMapDebugInfo = true;
    
    //环境配置比例
    public static Dictionary<string, float> FieldGenRate = new Dictionary<string, float> {
        {"Moun",30},{"Lake",10},{"Forest",60},{"Plain",100}
     };
    public static int WaveRate = 15;
}
