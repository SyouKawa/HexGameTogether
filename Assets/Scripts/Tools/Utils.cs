using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

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
}

public class testjkasjfg : ObjectBinding {

}
