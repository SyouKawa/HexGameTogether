using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public class EventHelper {
    //Actions
    public GameAction OnGameLoadEvent = new GameAction();
    public GameAction OnWorldLoadEvent = new GameAction();
    public GameAction OnUpdateEvent = new GameAction();
    public GameAction OnValueChanged = new GameAction();
}

public class Global : MonoBehaviour {
    public static Global Instance { get; private set; }

    //Inspector可直接测试数值
    public int MapWidth = 10;
    public int MapHeight = 10;
    public float DefaultCameraSize = 70f;
    public float CameraRollSpeed = 5f;

    public bool ShowMapDebugInfo;

    public EventHelper EventHelper { get; set; }
    
    private void Awake() {
        Instance = this;

        //用Inspector变量初始化静态变量
        SetConfigData();
        //对象池建立
        ObjectPoolData.InitPool(transform.GetChild(0));
        //初始化事件集
        EventHelper = new EventHelper();
        //遍历程序集. 获取Singleton的子类,初始化Singleton
        InvokeManager();

        //触发事件1
        EventHelper.OnGameLoadEvent.Invoke();
        //触发事件2
        EventHelper.OnWorldLoadEvent.Invoke();

        MapManager.GetInstance().SwitchDebugInfoState(ShowMapDebugInfo);
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
                }
                catch {
                    Utils.Log("同步Inspector到GameData出错,请检查二者类型是否有误:{0}", info.Name);
                }
            }
            else {
                Utils.Log("未同步变量:{0},GameData无此变量", info.Name);
            }
        }
        Utils.Log("同步Inspector完成,成功同步了{0}个变量", count);
    }


    private void InvokeManager() {
        foreach (Type type in Utils.AllTypes) {
            try {
                //1判断是否是Singleton子类
                Type superType = typeof(Singleton<>).MakeGenericType(type);
                if (type.IsSubclassOf(superType)) {
                    //2获取单例并创建出这个单例对象
                    MethodInfo info = superType.GetMethod("GetInstance");
                    object Instance = info.Invoke(null, info.GetParameters());
                    //3调用其Start方法
                    MethodInfo info2 = type.GetMethod("Start");
                    object[] objs = new object[1] { EventHelper };
                    info2.Invoke(Instance, objs);
                    Utils.Log("脚本已加载:{0}", type.FullName);
                }
            }
            catch {
                //Utils.LogError("初始化脚本绑定出错,脚本类型:{0}", type.FullName);
                continue;
            }
        }
    }
}
