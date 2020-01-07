using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 通过挂载这个特性来对某个类进行建池,必须挂载这个特性来进行类型对象绑定操作
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class PrefabPath : Attribute {
    public string path;

    public PrefabPath(string path) {
        this.path = path;
    }
}

/// <summary>
/// Unity对象控制类的模板,继承这个类来和UnityObj绑定
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
        Source = ObjectPoolData.Instance.GetInstantiate(this);
    }

    /// <summary>
    /// 删除的时候还回数据源 一定要手动释放对象
    /// </summary>
    public virtual void _Delete() {
        ObjectPoolData.Instance.ReturnInstantiate(Source, GetType());
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

    /// <summary>
    /// 通过对象获取类型
    /// </summary>
    /// <typeparam name="T">要获取的框架类型</typeparam>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static T GetClass<T>(GameObject gameObject) where T : class {
        return ObjectPoolData.Instance.GetClass<T>(gameObject);
    }
}
