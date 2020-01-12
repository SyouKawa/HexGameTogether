using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

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

}