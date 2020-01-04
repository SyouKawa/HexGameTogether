using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using static GameStaticData;

public class Global : MonoBehaviour
{
    public static Global Instance { get; private set; }

    //Inspector可直接测试数值
    public int width;
    public int height;
    public PlayerInMap player;
    public CheckState checkState;

    //Managers
    public ResManager resManager { get; private set; }
    public MapManager mapManager { get; private set; }
    public PathManager pathManager { get; private set; }

    //Actions
    public Action OnGameLoadEvent;
    public Action OnWorldLoadEvent;
    public Action OnEveryFrameEvent;


    private void Awake()
    {
        Instance = this;
        //对象池
        OnGameLoadEvent += () => { 
            //建立对象池
            ObjectPoolData.InitPool(transform.GetChild(0));
            //获取各类管理器
            resManager = ResManager.GetInstance();
            mapManager = MapManager.GetInstance();
            pathManager = PathManager.GetInstance();
        };

        OnWorldLoadEvent += () =>{
            //加载资源
            resManager.LoadRes();
            //生成地图
            mapManager.SpawnMap();

            checkState = CheckState.InMap;
        };

        OnEveryFrameEvent += () =>{
            //添加地图中的点击检测事件
            CheckClickInMap();
            //TODO:其他按帧检测的事件
        };

        OnGameLoadEvent.Invoke();
        OnWorldLoadEvent.Invoke();
    }

    public void Update()
    {
        OnEveryFrameEvent.Invoke();
    }

    /// <summary>
    /// 检测地图界面的点击
    /// </summary>
    private void CheckClickInMap() {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            Debug.Log("Checking");
            switch (checkState) {

                case CheckState.InMap:
                    UpdateMap(hit);
                break;

                case CheckState.InBattle:
                    UpdateBattle(hit);
                break;
                
                case CheckState.InFind:
                    UpdateFind(hit);
                break;
            }
        }
    }

    /// <summary>
    /// 大地图模式下的点击检测
    /// </summary>
    private void UpdateMap(RaycastHit2D hit) {
        if (Input.GetMouseButtonDown(0))
        {
            if (hit.collider.tag == "MapCell")
            {
                //TODO:显示点击Cell的详细信息
                HexCell curCell = ObjectManager.GetClass<HexCell>(hit.collider.transform.parent.gameObject);
                Debug.Log(curCell.MapPos);
            }
            if (hit.collider.tag == "Player")
            {
                //进入寻路模式
                checkState = CheckState.InFind;
            }
        }
    }

    /// <summary>
    /// 战斗模式下的点击检测
    /// </summary>
    private void UpdateBattle(RaycastHit2D hit){
        
    }

    /// <summary>
    /// 寻路模式下的点击检测
    /// </summary>
    private void UpdateFind(RaycastHit2D hit) {
        if (Input.GetMouseButtonDown(0))
        {
            if (hit.collider.tag == "MapCell")
            {
                HexCell dest = ObjectManager.GetClass<HexCell>(hit.collider.transform.parent.gameObject);
                Debug.Log("Set dest");
                //寻路
                PathHelper.GetFindPathResult(player.GetCurPosCell(), dest);
                //寻路结束回到大地图模式
                checkState = CheckState.InMap;
            }
        }
    }
}
