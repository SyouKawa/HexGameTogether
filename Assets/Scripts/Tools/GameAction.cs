using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Action的封装. 需要new 重载了+运算符,可以像Action一样操作
/// </summary>
public class GameAction {
    private List<Action> source = new List<Action>();

    public void Add(Action action) {
        source.Add(action);
    }

    public static GameAction operator +(GameAction source, Action add) {
        source.Add(add);
        return source;
    }

    public void Invoke() {
        foreach (var action in source) {
            try{
                action.Invoke();
            }catch(SystemException e){
                //使用堆栈获取函数位置，使用空格分割后，出错位置信息位于倒数第二个string中
                System.String ErrorAddress = e.StackTrace;
                string[] con = ErrorAddress.Split(' ');
                //获取函数信息
                MethodInfo info = action.GetMethodInfo();
                Log.Warning("{0}中的{1}函数执行异常,已跳过\n,错误位于:{2}",action.Target,info,con[con.Length-2]);
            }
        }
    }
}