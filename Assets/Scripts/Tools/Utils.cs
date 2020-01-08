using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;

/// <summary>
/// 日志类封装 日志级别 尽可能避免大量刷屏
/// Error 严重错误,影响正常运行. 对接Debug.LogError
/// Warning 使用方式错误,但是不影响运行. 对接Debug.LogWarning
/// Info 运行信息 对接Debug.Log
/// Debug 调试信息 对接Debug.Log
/// </summary>
public static class Log {

    /// <summary>
    /// 封装LogError 增加独立标识和stringbuilder兼容
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="objs"></param>
    public static void Error(string msg, params object[] objs) {
        StringBuilder sb = new StringBuilder();
        sb.Append("[FrameError:]");
        sb.AppendFormat(msg, objs);
        UnityEngine.Debug.LogError(sb.ToString());
    }

    /// <summary>
    /// 封装LogError 增加独立标识和stringbuilder兼容
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="objs"></param>
    public static void Warning(string msg, params object[] objs) {
        StringBuilder sb = new StringBuilder();
        sb.Append("[FrameWarning:]");
        sb.AppendFormat(msg, objs);
        UnityEngine.Debug.LogError(sb.ToString());
    }


    /// <summary>
    /// Log对日志输出
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="objs"></param>
    public static void Info(string msg, params object[] objs) {
        StringBuilder sb = new StringBuilder();
        sb.Append("[FrameInfo:]");
        sb.AppendFormat(msg, objs);
        UnityEngine.Debug.Log(sb.ToString());
    }

    /// <summary>
    /// Log对Unity输出
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="objs"></param>
    public static void Debug(string msg, params object[] objs) {
        StringBuilder sb = new StringBuilder();
        sb.Append("[Debug:]");
        sb.AppendFormat(msg, objs);
        UnityEngine.Debug.Log(sb.ToString());
    }

}
     

/// <summary>
/// 常用数据操作静态类
/// </summary>
public static class Utils
{


    private static List<Type> allTypes;
    /// <summary>
    /// 获取程序集中的所有类型,用于初始化
    /// </summary>
    public static List<Type> AllTypes {
        get {
            if(allTypes == null) {
                allTypes = Assembly.GetExecutingAssembly().GetTypes().ToList();
            }
            return allTypes;
        } }

}