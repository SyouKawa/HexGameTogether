using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class GameData{

    public static int MaxSupply = 100;

    public static Vector3 Rates = new Vector3(10,10,1);
    

    public static float YCellDis = 0.675f  ;//固定垂直间距
    public static float XCellDis = 2.258f  ;//固定水平间距

    public static int EdgeNum = 6;//Tile环绕边数
    public static float infinite = 99999f;
    public static System.Random random = new System.Random(1);

    //斜向移动单位Cell消耗的帧数
    public static int obliqueFrames = 23;
    //纵向移动单位Cell消耗的帧数
    public static int verticalFrames = 13;
    
    //地面背景色
    public static Color32 orange = new Color32(255,197,101,255);
    //水面背景色
    public static Color32 gray = new Color32(66,69,65,255);

    
    public static int MapWidth = 10;
    public static int MapHeight = 10;
    public static float DefaultCameraSize = 70f;
    public static float CameraRollSpeed = 5f;
    public static float CameraDriveSpeed = 0.5f;

    public static bool ShowMapDebugInfo = true;
    
    //环境配置比例
    public static Dictionary<string, float> FieldGenRate = new Dictionary<string, float> {
        {"Moun",30},{"Lake",10},{"Forest",60},{"Plain",100}
     };
    public static int WaveRate = 15;
}
