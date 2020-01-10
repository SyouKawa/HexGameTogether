using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

//TODO:: ObjectPoolData->ObjectPool
//ObjectPool -> SinglePool
//ObjectBinding增加Find方法

/// <summary>
/// 现在通常建议使用ObjectBinding来进行对象池绑定,不推荐使用这个api
/// 1.使用[PrefabPath]特性绑定一个Prefeb路径和初始化对象池
/// 2.使用GetInstantiate获取实例
/// 3.使用ReturnInstantiate删除实例
/// 4.使用GetClass通过实例反向获取框架对象
/// </summary>
public class ObjectPoolData {
    /// <summary>
    /// 通过类型找到对应的ObjectPool
    /// </summary>
    private Dictionary<System.Type, ObjectPool> PoolDic = new Dictionary<System.Type, ObjectPool>();

    /// <summary>
    /// 将GameObject翻译成类型
    /// </summary>
    private readonly Dictionary<System.Type, Dictionary<GameObject, object>> data = new Dictionary<System.Type, Dictionary<GameObject, object>>();

    /// <summary>
    /// 对象池新建的根节点,仍在屏幕外面.
    /// </summary>
    public Transform poolRootTransform;

    public static ObjectPoolData Instance;

    /// <summary>
    /// 将AddPool特性的类自动建池,初始化构造函数
    /// </summary>
    public static void InitPool(Transform rootTrans) {
        if (Instance != null) {
            return;
        }

        Instance = new ObjectPoolData() { poolRootTransform = rootTrans };

        int count = 0;

        foreach (Type type in Utils.AllTypes) {
            PrefabPath poolAtr = type.GetCustomAttribute<PrefabPath>();

            if (type.IsSubclassOf(typeof(ObjectBinding)) && poolAtr == null) {
                Log.Warning("{0}类继承了ObjectBinding,请为其添加PrefabPath特性来绑定一个对象", type.Name);
                return;
            }

            if (poolAtr != null) {
                if (!type.IsSubclassOf(typeof(ObjectBinding))) {
                    Log.Warning("{0}类不是ObjectBinding的子类,PrefabPath特性无效", type.Name);
                    return;
                }
                GameObject obj = Resources.Load<GameObject>(poolAtr.path);
                if (obj == null) {
                    Log.Error("未找到目标Prefeb,type:{0}, path:{1}", type.Name, poolAtr.path);
                }
                else {
                    count++;
                    //Log.Info("已建立对象池,type:{0} path:{1}" , type.Name , poolAtr.path);
                    Instance.AddPool(type, Resources.Load<GameObject>(poolAtr.path));
                }
            }
        }

        Log.Info("对象池初始化完成，共建立对象池{0}个", count);
    }

    /// <summary>
    /// 建立对象池,对象池通过框架类的类型绑定
    /// </summary>
    /// <param name="type">对象绑定的框架类类型</param>
    /// <param name="path">要建池的对象</param>
    public void AddPool(System.Type type, GameObject gameObject) {
        if (PoolDic.ContainsKey(type)) {
            Log.Warning("类型{0},已经被建立过对象池了,不要重复建立.",type.ToString());
            return;
        }

        //1 在这个节点底下创建一个节点,用于保存未激活的池对象
        Transform node = new GameObject(gameObject.name).transform;
        node.SetParent(poolRootTransform);
        node.localPosition = Vector3.zero;
        node.gameObject.SetActive(false);
        //2 新建对象池
        ObjectPool pool = new ObjectPool(type, gameObject, node, this);
        PoolDic.Add(type, pool);
        //3 添加新的反向绑定词典
        data.Add(type, new Dictionary<GameObject, object>());
    }

    /// <summary>
    /// 从对象池中获取需要提供框架类的对象来进行双向绑定
    /// 会自动识别类型返回需要的GameObject
    /// </summary>
    public GameObject GetInstantiate(object con) {
        return PoolDic[con.GetType()].GetInstantiate(con);
    }

    /// <summary>
    /// 需要提供框架类型,还回对象
    /// </summary>
    public void ReturnInstantiate<T>(GameObject obj) {
        PoolDic[typeof(T)].ReturnInstantiate(obj);
    }

    /// <summary>
    /// 需要提供框架类型,还回对象
    /// </summary>
    public void ReturnInstantiate(GameObject obj, System.Type type) {
        PoolDic[type].ReturnInstantiate(obj);
    }

    /// <summary>
    /// 通过对象获取类型
    /// </summary>
    /// <typeparam name="T">要获取的框架类型</typeparam>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public T GetClass<T>(GameObject gameObject) where T : class {
        Dictionary<GameObject, object> dic = data[typeof(T)];
        if (dic.ContainsKey(gameObject)) {
            return dic[gameObject] as T;
        }
        else {
            Debug.LogError("未找到框架类" + typeof(T).Name);
            return null;
        }
    }

    /// <summary>
    /// 单个对象池
    /// </summary>
    private class ObjectPool {
        /// <summary>
        /// 绑定的框架类的类型
        /// </summary>
        private readonly System.Type type;

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

        private ObjectPoolData manager;
        public ObjectPool(System.Type type, GameObject obj, Transform node, ObjectPoolData objectManager) {
            this.type = type;
            this.GamePrefeb = obj;
            this.PoolNode = node;
            manager = objectManager;
        }

        private void Create() {
            GameObject obj = UnityEngine.Object.Instantiate(GamePrefeb, PoolNode);
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
            HashSet<GameObject>.Enumerator em = ObjPoolSleep.GetEnumerator();
            em.MoveNext();
            //取出对象
            GameObject obj = em.Current;
            ObjPoolSleep.Remove(obj);

            obj.SetActive(true);

            ObjPoolActive.Add(obj);

            //设置对象词典
            manager.data[type].Add(obj, con);

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
            manager.data[type].Remove(obj);
        }
    }

}

