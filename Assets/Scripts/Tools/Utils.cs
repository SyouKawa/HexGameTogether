using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;
/// <summary>
/// 常用数据操作静态类
/// </summary>
public static class Utils
{
    /// <summary>
    /// Log对日志输出
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="objs"></param>
    public static void LogData(string msg, params object[] objs) {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat(msg, objs);
    }

    /// <summary>
    /// Log对Unity输出
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="objs"></param>
    public static void Log(string msg, params object[] objs) {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat(msg, objs);

        Debug.Log(sb.ToString());
    }

    /// <summary>
    /// 封装LogError 增加独立标识和stringbuilder兼容
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="objs"></param>
    public static void LogError(string msg, params object[] objs) {
        StringBuilder sb = new StringBuilder();
        sb.Append("[FrameWarning:]");
        sb.AppendFormat(msg, objs);
        Debug.LogError(sb.ToString());
    }

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