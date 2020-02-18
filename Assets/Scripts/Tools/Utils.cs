using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 常用数据操作静态类
/// </summary>
public static class Utils {
    /// <summary>
    /// 获取格式化字符串
    /// </summary>
    public static string FormatString(string msg, params object[] objs) {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat(msg, objs);
        return sb.ToString();
    }

    private static List<Type> allTypes;
    /// <summary>
    /// 获取程序集中的所有类型,用于初始化
    /// </summary>
    public static List<Type> AllTypes {
        get {
            if (allTypes == null) {
                allTypes = Assembly.GetExecutingAssembly().GetTypes().ToList();
            }
            return allTypes;
        }
    }

    [Obsolete("Prefab在运行时不可检测，已废弃",true)]
    public static GameObject GetClassByChild<T>(GameObject curObj) where T: PrefabBinding{
        GameObject classObj=curObj;
        //Prefab路径
        string attrPath = typeof(T).GetCustomAttribute<PrefabPath>().path;
        //Attribute.IsDefined(curObj.GetType(),PrefabPath)
        string objPath = AssetDatabase.GetAssetPath(curObj);
        
        /*如果不是PrefabClass节点，则往上递归
        if(objPath!=null && attrPath == objPath){
            return classObj;
        }*/
        //如果当前是根节点，则直接返回当前GameObject
        if(curObj.transform.parent!=null){
            classObj = GetClassByChild<T>(curObj.transform.parent.gameObject);
        }else{
            Debug.Log("已超出GameObject递归目录，当前节点为Global，将返回Null");
            return null;
        }     
        return classObj;
    }

    /// <summary>
    /// 通过对运行平台的检测，返回游戏内读取数据的对应路径
    /// </summary>
    public static string GetPlatformPath(){
        try{
            switch(Application.platform){
                case RuntimePlatform.OSXEditor: return GameData.ExcelPaths[RuntimePlatform.OSXEditor];
                case RuntimePlatform.WindowsEditor: return GameData.ExcelPaths[RuntimePlatform.WindowsEditor];
                case RuntimePlatform.IPhonePlayer : return GameData.ExcelPaths[RuntimePlatform.IPhonePlayer];
                case RuntimePlatform.Android : return GameData.ExcelPaths[RuntimePlatform.Android];
                case RuntimePlatform.WindowsPlayer: return GameData.ExcelPaths[RuntimePlatform.WindowsPlayer];
            }
        }catch{
            Log.Error("无法读取当前平台,请检查权限后重启游戏");
            //TODO:封装成暂停函数
            Time.timeScale = 0;
            return null;
        }
        //TODO：日志输出未知平台问题
        Log.Error("未知平台,无法找到游戏运行所需要的数据，已退出\n");
        Application.Quit();
        return null;
    }
}