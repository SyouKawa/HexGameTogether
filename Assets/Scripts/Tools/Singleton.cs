using System;
using System.Collections.Generic;

/// <summary>
/// Manager基类
/// 保持单例,且有默认的初始化等方法
/// </summary>
public abstract class Manager<T> : Singleton<T> where T : class, new(){
    public abstract void Start(EventHelper helper);
}

public abstract class Singleton<T> where T : class, new() {
    private static T _instance;

    public static T Instance {
        get {
            if (_instance == null) {
                _instance = new T();
            }
            return _instance;
        }
    }
}