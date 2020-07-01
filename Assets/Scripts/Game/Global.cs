using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class EventHelper {
    //Actions
    public GameAction OnGameLoadEvent = new GameAction();
    public GameAction OnWorldLoadEvent = new GameAction();
    public GameAction OnUpdateEvent = new GameAction();
    public GameAction AfterWorldLoadEvent = new GameAction();
}

public class Global : MonoBehaviour {
    public static Global Instance { get; private set; }
    //Inspector可直接测试数值
    public int MapWidth = 10;
    public int MapHeight = 10;
    public float DefaultCameraSize = 70f;
    public float CameraRollSpeed = 5f;
    public float CameraDriveSpeed = 0.5f;

    public float YCellDis = 6.7f ;//固定垂直间距
    public float XCellDis = 22.7f ;//固定水平间距
    public bool ShowMapDebugInfo;

    public EventHelper EventHelper { get; set; }

    private void Awake() {
        Instance = this;

        //用Inspector变量初始化静态变量
        SetConfigData();
        //对象池建立
        ObjectPool.InitPool(transform.GetChild(0));
        //初始化事件集
        EventHelper = new EventHelper();
        //遍历程序集. 获取Singleton的子类,初始化Singleton
        InvokeManager();
    }

    private void Start(){
        //触发事件1
        EventHelper.OnGameLoadEvent.Invoke();
        //触发事件2
        EventHelper.OnWorldLoadEvent.Invoke();
        //3
        EventHelper.AfterWorldLoadEvent.Invoke();
    }

    public void Update() {
        EventHelper.OnUpdateEvent.Invoke();
    }

    /// <summary>
    /// 读取Global中的所有变量,如果GameStaticData中有同名变量,那么尝试进行数值更新
    /// </summary>
    private void SetConfigData() {
        int count = 0;
        //1.获取本类中所有变量
        FieldInfo[] infos = GetType().GetFields();
        foreach (FieldInfo info in infos) {
            //2.查找Data类中是否有同名的此变量
            FieldInfo dataInfo = typeof(GameData).GetField(info.Name);
            if (dataInfo != null) {
                try {
                    //3.同步二者的数值
                    dataInfo.SetValue(null, info.GetValue(this));
                    count++;
                } catch {
                    Log.Error("{0}同步Inspector到GameData出错,类型{1}->{2}", info.Name, info.FieldType.Name, dataInfo.FieldType.Name);
                }
            } else {
                Log.Info("未同步变量:{0},GameData无此变量", info.Name);
            }
        }
        Log.Info("同步Inspector完成,成功同步了{0}个变量", count);
    }

    private void InvokeManager() {
        int count = 0;
        foreach (Type type in Utils.AllTypes) {
            Type superType;
            //1判断是否是Manager子类
            try {
                superType = typeof(Manager<>).MakeGenericType(type);
            } catch {
                continue;
            }

            if (type.IsSubclassOf(superType)) {
                //2获取单例并创建出这个单例对象
                PropertyInfo info = typeof(Singleton<>).MakeGenericType(type).GetProperty("Instance");
                object Instance = info.GetValue(null);

                //3调用其Start方法
                MethodInfo info2 = type.GetMethod("Start");
                object[] objs = new object[1] { EventHelper };
                info2.Invoke(Instance, objs);
                count++;
                Log.Info("脚本已加载:{0}", type.FullName);
            }
        }
        Log.Info("加载完成,本次共加载{0}", count);
    }
}