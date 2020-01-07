﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 初始化的时候,挂这个的会在最开始执行
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class EventBinding : Attribute {

}

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
    /// 根据子节点名称存储所有节点
    /// </summary>
    public Dictionary<string,GameObject> Nodes { get; private set; }

    /// <summary>
    /// 构造的时候从对象池获取数据源
    /// </summary>
    public ObjectBinding() {
        Source = ObjectHelper.GetInstantiate(this);
        //递归地存储节点
        Nodes = new Dictionary<string, GameObject>();
        RecursiveNode(Transform);
        //foreach(KeyValuePair<string,GameObject> v in Nodes) { Debug.Log(v.Key); }
    }
    /// <summary>
    /// 递归地获得所有子节点
    /// </summary>
    /// <param name="trans">Trans.</param>
    private void RecursiveNode(Transform trans) {
        foreach (Transform child in trans) {
            Nodes.Add(child.name, child.gameObject);
            if(child.childCount != 0) {
                RecursiveNode(child);
            }
        }
    }

    /// <summary>
    /// 删除的时候还回数据源 一定要手动释放对象
    /// </summary>
    public virtual void _Delete() {
        ObjectHelper.ReturnInstantiate(Source, GetType());
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
