using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//TODO&Tips 
//增加一个Count只读属性
//增加Remove方法 重载-运算符 注意异常处理
//当有一个Action被添加进这个类的时候,通过反射获取其信息并保存为字符串,用于出问题之后返回该方法对应的信息
//当Invoke的时候,处理方法内部执行时产生的错误,并可以正常抛出Log.Error.且不会影响其他方法的Invoke

//可选,不重要: 通过协程异步加载Invoke.
//注: 其实事件可以完全不带参数,这里可以采用一种学名叫做"黑板Blackboard"的设计模式,
//其实和透传的概念很像,就是在调用之前在静态区设置好相应的变量,然后方法去静态区(公共域)去取值,这样来避免调用时发生的复杂的参数传递.
//如果使用的变量少用GameData就很好,如果又多又杂,那么就可以专门搞一个静态类来放置这些变量.


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
            action.Invoke();
        }
    }
}