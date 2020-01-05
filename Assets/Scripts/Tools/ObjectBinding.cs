using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 一种新的初始化方式,通过挂载这个特性来对某个类进行建池
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class PrefabPath : Attribute {
    public string path;

    public PrefabPath(string path) {
        this.path = path;
    }
}

/// <summary>
/// 一个暂定的Unity对象控制类的模板
/// 这个模板不可以直接被实例化,一定要被继承. 绑定的数据类型是子类的类型
/// API暂定,待整理
/// </summary>
public abstract class ObjectBinding {
    /// <summary>
    /// Unity对象的数据源
    /// </summary>
    public GameObject Source { get; set; }
    /// <summary>
    /// Unity Transform
    /// </summary>
    public Transform Transform { get => Source.transform; }

    /// <summary>
    /// 构造的时候从对象池获取数据源
    /// </summary>
    public ObjectBinding() {
        Source = ObjectManager.GetInstantiate(this);
    }

    /// <summary>
    /// 删除的时候还回数据源 一定要手动释放对象
    /// </summary>
    public virtual void _Delete() {
        ObjectManager.ReturnInstantiate(Source, GetType());
    }

    /// <summary>
    /// 释放一个列表
    /// </summary>
    public static void DeleteList<T>(List<T> list) where T : ObjectBinding {
        foreach (T obj in list) {
            obj._Delete();
        }
        list.Clear();
    }
}
