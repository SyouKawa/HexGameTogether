using System.Collections.Generic;
using UnityEngine;

/*
 * 
 */

/// <summary>
/// 框架类和Unity类的双向绑定器,纯静态类
/// 框架类: 不和Unity挂钩的自定义类 Unity类: Unity对象,二者强制一对一绑定
/// Collider必须挂在最上层,否则检测不到(可能考虑对自定义获取的支持)
/// 使用说明: 
/// 1.使用AddPool添加和绑定对象池
/// 2.使用GetInstantiate获取实例
/// 3.使用ReturnInstantiate删除实例
/// 4.使用GetClass通过实例反向获取框架对象
/// </summary>
public static class ObjectManager {
    /// <summary>
    /// 通过类型找到对应的ObjectPool
    /// </summary>
    private static Dictionary<System.Type, ObjectPool> PoolDic = new Dictionary<System.Type, ObjectPool>();

    /// <summary>
    /// 将GameObject翻译成类型
    /// </summary>
    private static readonly Dictionary<System.Type, Dictionary<GameObject, object>> data = new Dictionary<System.Type, Dictionary<GameObject, object>>();

    /// <summary>
    /// 对象池新建的根节点,仍在屏幕外面.
    /// </summary>
    public static Transform poolRootTransform = Global.Instance.transform;

    /// <summary>
    /// 建立对象池,对象池通过框架类的类型绑定
    /// </summary>
    /// <param name="type">对象绑定的框架类类型</param>
    /// <param name="path">要建池的对象</param>
    public static void AddPool(System.Type type,GameObject gameObject) {
        if (PoolDic.ContainsKey(type)) {
            Debug.LogError("这个池子已经被建立过了,不能重复建立." + type.ToString());
        }
        //1 在这个节点底下创建一个节点,用于保存未激活的池对象
        Transform node = new GameObject(gameObject.name).transform;
        node.SetParent(poolRootTransform);
        node.localPosition = Vector3.zero;
        node.gameObject.SetActive(false);
        //2 新建对象池
        ObjectPool pool = new ObjectPool(type, gameObject,node);
        PoolDic.Add(type, pool);
        //3 添加新的反向绑定词典
        data.Add(type, new Dictionary<GameObject, object>());
    }

    /// <summary>
    /// 从对象池中获取需要提供框架类的对象来进行双向绑定
    /// 会自动识别类型返回需要的GameObject
    /// </summary>
    public static GameObject GetInstantiate(object con) {
        return PoolDic[con.GetType()].GetInstantiate(con);
    }

    /// <summary>
    /// 需要提供框架类型,还回对象
    /// </summary>
    public static void ReturnInstantiate<T>(GameObject obj) {
        PoolDic[typeof(T)].ReturnInstantiate(obj);
    }

    /// <summary>
    /// 通过对象获取类型
    /// </summary>
    /// <typeparam name="T">要获取的框架类型</typeparam>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static T GetClass<T>(GameObject gameObject) where T : class {
        var dic = data[typeof(T)];
        if (dic.ContainsKey(gameObject))
            return dic[gameObject] as T;
        else {
            Debug.LogError("未找到框架类" + typeof(T).Name);
            return null;
        }
    }

    /// <summary>
    /// 一个对象池,可能不是那么好用
    /// </summary>
    /// <typeparam name="T">对象池绑定的框架类的类型</typeparam>
    class ObjectPool {
        /// <summary>
        /// 绑定的框架类的类型
        /// </summary>
        private System.Type type;

        /// <summary>
        /// Unity对象的类型一定是GameObject
        /// </summary>
        public GameObject GamePrefeb;

        /// <summary>
        /// 未被激活的对象
        /// </summary>
        public HashSet<GameObject> ObjPoolSleep = new HashSet<GameObject>();

        /// <summary>
        /// 被激活的对象
        /// </summary>
        public HashSet<GameObject> ObjPoolActive = new HashSet<GameObject>();

        /// <summary>
        /// 在这个脚本挂载的节点下创建一个PoolNode节点用于挂载对象池未激活的对象
        /// </summary>
        public Transform PoolNode;

        public ObjectPool(System.Type type, GameObject obj, Transform node) {
            this.type = type;
            this.GamePrefeb = obj;
            this.PoolNode = node;
        }

        private void Create() {
            GameObject obj = Object.Instantiate(GamePrefeb, PoolNode);
            ObjPoolSleep.Add(obj);
            obj.SetActive(false);
        }

        /// <summary>
        /// 获取一个激活的对象 想要获得对应的GameObject既需要提供类型,也需要提供对象
        /// </summary>
        public GameObject GetInstantiate(object con) {
            if (ObjPoolSleep.Count == 0) {
                Create();
            }
            var em = ObjPoolSleep.GetEnumerator();
            em.MoveNext();
            //取出对象
            GameObject obj = em.Current;
            ObjPoolSleep.Remove(obj);

            obj.SetActive(true);

            ObjPoolActive.Add(obj);

            //设置对象词典
            data[type].Add(obj, con);

            return obj;
        }

        /// <summary>
        /// 还回一个对象
        /// </summary>
        public void ReturnInstantiate(GameObject obj) {
            ObjPoolActive.Remove(obj);

            obj.transform.SetParent(PoolNode);
            ObjPoolSleep.Add(obj);
            //删除对象词典
            data[type].Remove(obj);
        }
    }

}

