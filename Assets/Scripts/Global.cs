using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{
    public static Global Instance { get; private set; }
    public ObjectPoolData objectManager;
    private void Awake()
    {
        Instance = this;
        objectManager = new ObjectPoolData();
        objectManager.poolRootTransform = transform;
        objectManager.InitPool();
    }
}

/// <summary>
/// 对象池接口
/// 使用说明: 
/// 1.使用[AddPool]特性添加和绑定对象池
/// 2.使用GetInstantiate获取实例
/// 3.使用ReturnInstantiate删除实例
/// 4.使用GetClass通过实例反向获取框架对象
/// </summary>
public static class ObjectManager {
    /// <summary>
    /// 从对象池中获取需要提供框架类的对象来进行双向绑定
    /// 会自动识别类型返回需要的GameObject
    /// </summary>
    public static GameObject GetInstantiate(object con) {
        return Global.Instance.objectManager.GetInstantiate(con);
    }

    /// <summary>
    /// 需要提供框架类型,还回对象
    /// </summary>
    public static void ReturnInstantiate<T>(GameObject obj) {
        Global.Instance.objectManager.ReturnInstantiate<T>(obj);
    }

    /// <summary>
    /// 通过对象获取类型
    /// </summary>
    /// <typeparam name="T">要获取的框架类型</typeparam>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static T GetClass<T>(GameObject gameObject) where T : class {
        return Global.Instance.objectManager.GetClass<T>(gameObject);
    }
}
