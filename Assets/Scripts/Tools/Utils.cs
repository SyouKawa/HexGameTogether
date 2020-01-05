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
    public static void LogError(string msg,params object[] objs) {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat(msg, objs);
        Debug.LogError(sb.ToString());
    }

    private static List<Type> allTypes;
    public static List<Type> AllTypes {
        get {
            if(allTypes == null) {
                allTypes = Assembly.GetExecutingAssembly().GetTypes().ToList();
            }
            return allTypes;
        } }

}