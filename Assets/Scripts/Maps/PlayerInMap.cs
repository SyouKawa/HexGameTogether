using UnityEngine;
using static GameStaticData;

[PrefabPath("Prefabs/Map/PlayerInMap")]
public class PlayerInMap : ObjectBinding {
    private HexCell curCell;

    public PlayerInMap() {
        Global.Instance.eventHelper.OnUpdateEvent += CheckClickInMap;
    }

    public HexCell CurCell {
        get => curCell;
        set {
            Transform.SetParent(value.Transform);
            Transform.localPosition = Vector3.zero;
            curCell = value;
            CameraController.GetInstance().SetPosition(curCell.Transform);
        }
    }

    public CheckState checkState = CheckState.InMap;

    /// <summary>
    /// 检测地图界面的点击
    /// </summary>
    private void CheckClickInMap() {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hits.Length != 0) {
            foreach (RaycastHit2D hit in hits) {
                if (hit.collider != null) {
                    Debug.Log("Checking");
                    switch (checkState) {
                        case CheckState.InMap:
                            UpdateMap(hit);
                            break;
                        case CheckState.InFind:
                            UpdateFind(hit);
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
                HexCell curCell = ObjectHelper.GetClass<HexCell>(hit.collider.transform.parent.gameObject);
                Debug.Log(curCell.MapPos);
            }
            if (hit.collider.tag == "Player") {
                //进入寻路模式
                checkState = CheckState.InFind;
            }
        }
    }


    private HexCell lastCell;

    /// <summary>
    /// 寻路模式下的点击检测
    /// </summary>
    private void UpdateFind(RaycastHit2D hit) {
        if (hit.collider.tag == "MapCell") {
            HexCell cell = ObjectHelper.GetClass<HexCell>(hit.collider.transform.parent.gameObject);
            if(cell != lastCell && cell != curCell) {
                Debug.Log("Set dest");
                //寻路
                PathHelper.GetFindPathResult(CurCell, cell);
                lastCell = cell;
            }
        }
    }
}
