using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Global : MonoBehaviour
{
    public static Global Instance { get; private set; }

    public MapManager mapManager;

    //TODO: 封装一个Action
    public Action OnGameLoadEvent;
    public Action OnWorldLoadEvent;



    private void Awake()
    {
        Instance = this;
        //对象池
        OnGameLoadEvent += () => { ObjectPoolData.InitPool(transform); };

        OnWorldLoadEvent += () =>
        {
            //建立地图管理器
            mapManager = Instantiate(Resources.Load<MapManager>("Prefabs/Map/MapManager"));
            //TODO
        };

        OnGameLoadEvent.Invoke();

        OnWorldLoadEvent.Invoke();
    }
}
