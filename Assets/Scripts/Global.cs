using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{
    public static Global Instance { get; private set; }
    public ObjectPoolData objectManager;
    public MapManager mapManager;

    private void Awake()
    {
        Instance = this;

        //建立地图管理器
        mapManager = Instantiate(Resources.Load<MapManager>("Prefabs/Map/MapManager"));

        //建立对象池管理器
        objectManager = new ObjectPoolData();
        objectManager.poolRootTransform = transform;
        objectManager.InitPool();
    }
}
