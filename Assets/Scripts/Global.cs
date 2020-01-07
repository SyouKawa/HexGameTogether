using System;
using System.Reflection;
using UnityEngine;
using static GameStaticData;


public class EventHelper {
    //Actions
    public GameAction OnGameLoadEvent = new GameAction();
    public GameAction OnWorldLoadEvent = new GameAction();
    public GameAction OnUpdateEvent = new GameAction();
}


public class Global : MonoBehaviour {
    public static Global Instance { get; private set; }

    //Inspector可直接测试数值
    public int width = 10;
    public int height = 10;
    public float defaultCameraSize = 70f;
    public float cameraRollSpeed = 5f;
    public bool isDebug = true;
    
    public EventHelper eventHelper;

    private void Awake() {
        Instance = this;

        //用Inspector变量初始化静态变量
        MapHeight = height;
        MapWidth = width;
        DefaultCameraSize = defaultCameraSize;
        CameraRollSpeed = cameraRollSpeed;

        //对象池建立
        ObjectPoolData.InitPool(transform.GetChild(0));
        //初始化事件集
        eventHelper = new EventHelper();
        //遍历程序集. 获取Singleton的子类,初始化Singleton
        foreach (Type type in Utils.AllTypes) {
            try {
                Type superType = typeof(Singleton<>).MakeGenericType(type);
                if (type.IsSubclassOf(superType)) {
                    //获取单例并创建出这个单例对象
                    MethodInfo info = superType.GetMethod("GetInstance");
                    object Instance = info.Invoke(null, info.GetParameters());
                    //调用其Binding方法
                    MethodInfo info2 = type.GetMethod("Start");
                    object[] objs = new object[1] { eventHelper };
                    info2.Invoke(Instance, objs);
                }
            }
            catch {
                //Utils.LogError("WTF  {0}", type.FullName);
                continue;
            }
        }

        //触发事件1
        eventHelper.OnGameLoadEvent.Invoke();
        //触发事件2
        eventHelper.OnWorldLoadEvent.Invoke();
    }

    public void Update() {
        eventHelper.OnUpdateEvent.Invoke();
    }

}
