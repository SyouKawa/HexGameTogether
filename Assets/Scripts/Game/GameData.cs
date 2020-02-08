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

    //不同平台和运行模式读取数据时的通用路径列表
    //平台（eg：OSX，Windows，Android，IOS）
    //运行模式（eg：Editor编辑器模式和Player运行模式）
    public static Dictionary<RuntimePlatform,string> ExcelPaths = new Dictionary<RuntimePlatform, string>{
        
        //不同平台的Editor模式
        {RuntimePlatform.OSXEditor,"/Resources/ExcelData/GameData.xlsx"},
        {RuntimePlatform.WindowsEditor,"\\Resources\\ExcelData\\GameData.xlsx"},
        
        //不同平台的App模式（读取AssetBundle或files的路径） ,以下仅为样例
        {RuntimePlatform.Android,"/data/app/HexGame.apk/!/assets/GameData.data"},
        {RuntimePlatform.WindowsPlayer,"\\res\\data\\ExcelGameData.bin"},
        {RuntimePlatform.IPhonePlayer,"/com.AssetsBundles/data/GameData.data"}
        
        //TODO：不同平台的Web模式（读取BinFile）
    };
}
