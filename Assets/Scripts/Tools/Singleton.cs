using System;
using System.Collections.Generic;

/// <summary>
/// 通用泛型单例类 
/// 使用形式 class Type : Singleton<Type>{}
/// </summary>
public abstract class Singleton<T> where T : class,new() {

    public abstract void Start(EventHelper helper);

    private static T _instance;
    public static T GetInstance() {

        if (_instance == null) {
            _instance = new T();
        }
        return _instance;
    }
}