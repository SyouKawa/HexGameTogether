using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;

public class RefHelper {


    /// <summary>
    /// 动态执行方法
    /// </summary>
    public T InvokeMethod<T>(object instance, string methodname, object[] para) {
        BindingFlags flag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;

        Type type = instance.GetType();

        MethodInfo info = type.GetMethod(methodname, flag);

        var v = info.GetParameters();

        return (T)info.Invoke(instance, para);
    }

    /// <summary>
    /// 如果可以转换,那么进行转换,否则返回default(T) 不产生中断
    /// </summary>
    public T SafeConvert<T>(object obj) {
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

