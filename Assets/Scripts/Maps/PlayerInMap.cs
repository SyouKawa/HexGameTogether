using System.Collections;
using UnityEngine;
using static GameData;


//做了一点小改动 不跟随图块

[PrefabPath("Prefabs/Map/PlayerInMap")]
public class PlayerInMap : ObjectBinding {
    public int supply = 100;
    private HexCell lastCell;
    private FpResult curpath;
    private HexCell curCell;
    public CheckState checkState = CheckState.InMap;
    private int index = 1;

    public PlayerInMap() {
        Transform.SetParent(Global.Instance.transform);
        Global.Instance.EventHelper.OnUpdateEvent += CheckClickInMap;
    }

    public HexCell CurCell {
        get => curCell;
        set {
            curCell = value;
            //Transform.SetParent(value.Transform);
            //Transform.localPosition = Vector3.zero;
            Transform.position = value.Transform.position;
            CameraController.Instance.SetPosition(curCell.Transform);
        }
    }

    /// <summary>
    /// 检测地图界面的点击
    /// </summary>
    private void CheckClickInMap() {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hits.Length != 0) {
            foreach (RaycastHit2D hit in hits) {
                if (hit.collider != null) {
                    //Debug.Log("Checking");
                    //Debug.Log(hit.collider.name);
                    switch (checkState) {
                        case CheckState.InMap:
                            UpdateMap(hit);
                            break;
                        case CheckState.InFind:
                            //Debug.Log("Finding");
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
    private void UpdateMap(RaycastHit2D hit) {
        if (Input.GetMouseButtonDown(0)) {
            if (hit.collider.tag == "MapCell") {
                //TODO:显示点击Cell的详细信息
                HexCell curCell = GetClass<HexCell>(hit.collider.transform.parent.gameObject);
                Debug.Log(curCell.MapPos);
            }
            if (hit.collider.tag == "Player") {
                //进入寻路模式
                checkState = CheckState.InFind;
            }
        }
    }

    /// <summary>
    /// 寻路模式下的点击检测
    /// </summary>
    private void UpdateFind(RaycastHit2D hit) {
        if (hit.collider.tag == "MapCell") {
            HexCell cell = GetClass<HexCell>(hit.collider.transform.parent.gameObject);
            //非寻路区域直接返回
            if (cell.FieldType == FieldType.EdgeSea || cell.FieldType == FieldType.Lake) {
                return;
            }
            if (cell != lastCell && cell != curCell) {
                Debug.Log("Set dest");
                //清空Debug显示
                PathManager.Instance.FreeFindPathData();
                //开始寻路
                curpath = PathManager.Instance.GetPath(CurCell, cell);
                Debug.Log("sumCost = " + curpath.Sumcost);
                MapManager.Instance.PlayerInfoUI.PreviewSupply(curpath.Sumcost);
                if (!curpath.IsFinded) {
                    Debug.Log("Cant Catch.");
                }
                lastCell = cell;
            }
            //左键选中目的地（不是当前所在地)
            if (Input.GetMouseButtonDown(0) && curCell != cell) {
                //减去此次的消耗
                supply -= curpath.Sumcost;
                PathManager.Instance.FreeFindPathData();
                checkState = CheckState.InMoving;
                return;
            }
        }
    }

    void UpdateMoving() {
        float dis = Vector2.Distance(curpath.Path[index].Transform.position, Transform.position);
        if (dis > 1f) {
            //仿匀速移动
            Transform.Translate(0.5f * (curpath.Path[index].Transform.position - Transform.position).normalized, Space.Self);
            //TODO:Supply符合路径消耗地逐渐减少
            if (MapManager.Instance.PlayerInfoUI.SupplyBar.fillAmount > MapManager.Instance.PlayerInfoUI.PreviewBar.fillAmount) {
                MapManager.Instance.PlayerInfoUI.SupplyBar.fillAmount -= 0.001f;
            }
        } else {
            if (index < curpath.Path.Count - 1) {
                index++;
            } else {
                Debug.Log("Ending");
                CurCell = curpath.Path[index];
                //SetPlayer(curpath.Path[index]);
                index = 1; //重置辅助标记
                checkState = CheckState.InMap;
            }
        }
    }
}