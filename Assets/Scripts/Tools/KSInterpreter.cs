using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

/*
 *  第一个版本准备支持的逻辑
 *  MethodName() 准备执行的方法
 *  参数支持所有种类的参数,int,string,enum等等.不需要额外的格式逗号分隔.
 *  TODO:
 *  0.完善的异常处理机制
 *  1.增加套娃支持,可以用一个函数的返回值作为另一个函数的参数
 *  2.增加扩展类支持,允许使用::符来执行其他类的方法.可以兼容多个类
 *  Attack(E2,2);  来调用Attack(string,int) 自动把参数转化为想要的类型,如果转化出错那么抛出错误,不然无视类型强行转换
 *  Attack(GetHp(),2);
 */

/// <summary>
/// 脚本语言解释器 KyaScript KS脚本
/// </summary>
public class KSInterpreter<ClassType> where ClassType : class {
    #region 绑定类型和构造函数
    public ClassType Instance;
    public Type SourceClassType;
    /// <summary>
    /// 默认构造函数,编译器绑定的是静态类
    /// </summary>
    public KSInterpreter() {
        Instance = null;
        SourceClassType = typeof(ClassType);
    }
    /// <summary>
    /// 动态构造,编译器绑定的是一个类的对象
    /// </summary>
    public KSInterpreter(ClassType instance) { 
        Instance = instance;
        SourceClassType = typeof(ClassType);
    }
    #endregion

    /// <summary>
    /// 动态执行方法
    /// </summary>
    public object InvokeMethod(object instance, string methodname, object[] para) {
        BindingFlags flag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;

        Type type = SourceClassType;


        MethodInfo info = type.GetMethod(methodname, flag);

        var v = info.GetParameters();

        return info.Invoke(instance, para);
    }

    /// <summary>
    /// 强制转换类型 将(目前只有字符串)转换为需要的类型
    /// </summary>
    public object SafeConvert(object obj, Type type) {
        object result = null;
        //如果要获取的是一个枚举,那么将obj强转字符转再转枚举
        if (type.IsEnum) {
            try {
                result = Enum.Parse(type, (string)SafeConvert(obj, typeof(string)));
            }
            catch {
                Console.WriteLine("强转枚举出错");
            }
        }
        //默认使用Convert强转
        else {
            try {
                result = Convert.ChangeType(obj, type);
            }
            catch {
                Console.WriteLine("强转Convert出错");
            }
        }

        return result;
    }

    /// <summary>
    /// 执行脚本,运行脚本逻辑
    /// </summary>
    public void Run(string code) {
        //将代码分割为语句
        string[] Statements = code.Split(';');

        foreach (string sta in Statements) {
            RunStatement(sta);
        }
    }

    /// <summary>
    /// 执行单条语句 之后得做好这个的异常处理
    /// </summary>
    private void RunStatement(string code) {
        if (code.Length == 0)
            return;
        string[] ss = code.Split('(');
        string methodName = ss[0];
        string[] paraSource = ss[1].Substring(0, ss[1].Length - 1).Split(',');

        MethodInfo methodInfo = SourceClassType.GetMethod(methodName);
        ParameterInfo[] paraInfos = methodInfo.GetParameters();

        object[] paraObj = new object[paraSource.Length];
        for (int i = 0; i < paraSource.Length; i++) {
            paraObj[i] = SafeConvert(paraSource[i], paraInfos[i].ParameterType);
        }

        InvokeMethod(Instance, methodName, paraObj);
    }
}


/// <summary>
/// 没啥用
/// </summary>
public static class RefHelper {
    /// <summary>
    /// 动态执行方法
    /// </summary>
    public static T InvokeMethod<T>(object instance, string methodname, object[] para) {
        BindingFlags flag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;

        Type type = instance.GetType();

        MethodInfo info = type.GetMethod(methodname, flag);

        var v = info.GetParameters();

        return (T)info.Invoke(instance, para);
    }

    /// <summary>
    /// 如果可以转换,那么进行转换,否则返回default(T) 不产生中断
    /// </summary>
    public static T SafeConvert<T>(object obj) {
        //先置为默认值
        T result = default;
        //如果要获取的是一个枚举,那么将obj强转字符转再转枚举
        Type type = typeof(T);
        if (type.IsEnum) {
            try {
                result = (T)Enum.Parse(type, SafeConvert<string>(obj));
            }
            catch {
                Console.WriteLine("change wrong");
            }
        }
        //默认使用Convert强转
        else {
            try {
                result = (T)Convert.ChangeType(obj, typeof(T));
            }
            catch {
                Console.WriteLine("change wrong");
            }
        }

        return result;
    }

}

