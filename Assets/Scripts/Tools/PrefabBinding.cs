using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 指定类说绑定的Unity对应的Prefab的路径
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class PrefabPath : Attribute {
    public string path;

    public PrefabPath(string path) {
        this.path = path;
    }
}

/// 一个加强类.可以用于处理更复杂的UI层级问题
// 首先把name -> parent.name
public abstract class ExtendPrefabBinding : PrefabBinding {
    /// <summary>
    /// 递归地获得所有子节点
    /// </summary>
    /// <param name="trans">Trans.</param>
    protected override void RecursiveNode(Transform trans) {
        AddNode(trans, "");
        // foreach(var str in  nodes.Keys){
        //     Log.Warning(str);
        // }
    }

    public RectTransform FindRecTrans(string name){
        return Find(name)?.transform as RectTransform;
    }

    private void AddNode(Transform trans, string cur) {
        foreach (Transform child in trans) {
            string name;
            if (cur != "") {
                name = cur + "." + child.name;
            } else {
                name = child.name;
            }
            //避免重名添加
            if (nodes.ContainsKey(name)) {
                Log.Warning("当前对象{0} 重复添加了节点:{1}", Source.name, name);
            } else
                nodes.Add(name, child.gameObject);
            if (child.childCount != 0) {
                AddNode(child, name);
            }
        }
    }
}

/// <summary>
/// 继承这个类来Unity的Prefab
/// 调用构造函数时,初始化这个Prefab
/// 调用_Delete()函数来销毁这个Prefab
/// 挂载PrefabPath特性来指定要绑定哪个Prefab
/// </summary>
public abstract class PrefabBinding {
    /// <summary>
    /// Unity对象的数据源
    /// </summary>
    public GameObject Source { get; set; }

    public string Name {
        get { return Source.name; }
        set {
            Source.name = value;
        }
    }

    /// <summary>
    /// Unity Transform
    /// </summary>
    public Transform Transform {get;set;}
    /// <summary>
    /// 根据子节点名称存储所有节点
    /// </summary>
    protected Dictionary<string, GameObject> nodes;

    /// <summary>
    /// 通过字符串获取GameObject 加入异常处理
    /// </summary>
    public GameObject Find(string name) {
        if (nodes.ContainsKey(name)) {
            return nodes[name];
        } else {
            Log.Error("当前对象{0} 未找到节点:{1}", Source.name, name);
            return null;
        }
    }

    /// <summary>
    /// 构造的时候从对象池获取数据源
    /// </summary>
    public PrefabBinding() {
        BaseInit();
    }

    protected virtual void BaseInit(){
        Source = ObjectPool.Instance.GetInstantiate(this);
        Transform = Source.transform;
        //递归地存储节点
        nodes = new Dictionary<string, GameObject>();
        RecursiveNode(Transform);
    }


    /// <summary>
    /// 递归地获得所有子节点
    /// </summary>
    /// <param name="trans">Trans.</param>
    protected virtual void RecursiveNode(Transform trans) {
        foreach (Transform child in trans) {
            //避免重名添加
            if (nodes.ContainsKey(child.name)) {
                Log.Warning("当前对象{0} 重复添加了节点:{1}", Source.name, child.name);
            } else
                nodes.Add(child.name, child.gameObject);
            if (child.childCount != 0) {
                RecursiveNode(child);
            }
        }
    }

    /// <summary>
    /// 删除的时候还回数据源 一定要手动释放对象
    /// </summary>
    public virtual void _Delete() {
        ObjectPool.Instance.ReturnInstantiate(Source, GetType());
    }

    /// <summary>
    /// 释放一个列表
    /// </summary>
    public static void DeleteList<T>(List<T> list) where T : PrefabBinding {
        foreach (T obj in list) {
            obj._Delete();
        }
        list.Clear();

    }

    /// <summary>
    /// 这个调用策略自己编写的函数需要返回IEnumerator
    /// </summary>
    public void Startcoroutine(IEnumerator routine) {
        Global.Instance.StartCoroutine(routine);
    }

    /// <summary>
    /// 通过对象获取类型
    /// </summary>
    /// <typeparam name="T">要获取的框架类型</typeparam>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static T GetClass<T>(GameObject gameObject) where T : class {
        return ObjectPool.Instance.GetClass<T>(gameObject);
    }

    public static T GetClass<T>(Collider2D collider) where T : PrefabBinding{
        return ObjectPool.Instance.GetClass<T>(collider);
    }
}