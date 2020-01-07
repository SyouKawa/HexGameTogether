using System.Collections;
using UnityEngine;
using static GameData;

[PrefabPath("Prefabs/Map/PlayerInMap")]
public class PlayerInMap : ObjectBinding
{
    public int supply = 40;
    private HexCell lastCell;
    private FpResult curpath;
    private HexCell curCell;
    public CheckState checkState = CheckState.InMap;
    private int index = 1;
     

    public PlayerInMap()
    {
        Global.Instance.EventHelper.OnUpdateEvent += CheckClickInMap;
    }

    public HexCell CurCell{
        get => curCell;
        set => SetPlayer(value);
    }
    /// <summary>
    /// 由入参Cell设置玩家在地图中的位置
    /// </summary>
    void SetPlayer(HexCell cell) {
        Transform.SetParent(cell.Transform);
        Transform.localPosition = Vector3.zero;
        curCell = cell;
        CameraController.GetInstance().SetPosition(curCell.Transform);
    }

    /// <summary>
    /// 检测地图界面的点击
    /// </summary>
    private void CheckClickInMap()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hits.Length != 0)
        {
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null)
                {
                    Debug.Log("Checking");
                    Debug.Log(hit.collider.name);
                    switch (checkState)
                    {
                        case CheckState.InMap:
                            UpdateMap(hit);
                            break;
                        case CheckState.InFind:
                            Debug.Log("Finding");
                            UpdateFind(hit);
                            break;
                        case CheckState.InMoving:
                            Debug.Log("Moving");
                            UpdateMoving();
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 大地图模式下的点击检测
    /// </summary>
    private void UpdateMap(RaycastHit2D hit)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (hit.collider.tag == "MapCell")
            {
                //TODO:显示点击Cell的详细信息
                HexCell curCell = GetClass<HexCell>(hit.collider.transform.parent.gameObject);
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
    /// 寻路模式下的点击检测
    /// </summary>
    private void UpdateFind(RaycastHit2D hit)
    {
        if (hit.collider.tag == "MapCell")
        {
            HexCell cell = GetClass<HexCell>(hit.collider.transform.parent.gameObject);
            //非寻路区域直接返回
            if (cell.type == FieldType.EdgeSea || cell.type == FieldType.Lake)
            {
                return;
            }
            if (cell != lastCell && cell != curCell)
            {
                Debug.Log("Set dest");
                //清空Debug显示
                PathManager.GetInstance().FreeFindPathData();
                //开始寻路
                curpath = PathHelper.GetFindPathResult(CurCell, cell);
                if (!curpath.isfinded)
                {
                    Debug.Log("Cant Catch.");
                }
                lastCell = cell;
            }
            //左键选中目的地（不是当前所在地)
            if (Input.GetMouseButtonDown(0) && curCell!= cell)
            {
                PathManager.GetInstance().FreeFindPathData();
                checkState = CheckState.InMoving;
                Debug.Log(curpath);
                return;
            }
        }
    }

    void UpdateMoving() {
        float dis = Vector2.Distance(curpath.path[index].Transform.position, Transform.position);
        if(dis > 1f) {
            Transform.Translate(0.5f*(curpath.path[index].Transform.position-Transform.position).normalized, Space.Self);
        }
        else {
            if (index < curpath.path.Count-1) {
                index++;
            }
            else {
                Debug.Log("Ending");
                SetPlayer(curpath.path[index]);
                index = 1;//重置辅助标记
                checkState = CheckState.InMap;
            }
        }
    }
}
