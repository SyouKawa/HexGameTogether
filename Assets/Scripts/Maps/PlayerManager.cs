using System.Collections.Generic;
using UnityEngine;
using static GameData;

public class PlayerManager : Manager<PlayerManager> {
    public PlayerInMap Player;

    public override void Start(EventHelper helper) {
        helper.AfterWorldLoadEvent += CreatePlayer;
    }

    public void CreatePlayer() {
        Player = new PlayerInMap();
        Player.CurCell = MapManager.Instance.GetCell(7, 4);
    }

    [PrefabPath("Prefabs/Map/PlayerInMap")]
    public class PlayerInMap : PrefabBinding {
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
                Transform.position = value.Transform.position;
            }
        }

        public void PlayerGoTo(HexCell Dest) {
            //获得移动方向的单位向量
            Vector2 dir = (Vector2) (Dest.Transform.position - CurCell.Transform.position).normalized;
            //以单位向量各轴的分量距离为每帧增量，移动Player
            Transform.position = new Vector3(Transform.position.x + dir.x, Transform.position.y + dir.y, 0f);
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
                                //Debug.Log("Moving");
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
                    //清空Debug显示
                    PathManager.Instance.FreeFindPathData();
                    //开始寻路
                    curpath = PathManager.Instance.GetPath(CurCell, cell);
                    MapManager.Instance.infoHUD.PreviewSupply(curpath.Sumcost);
                    if (!curpath.IsFinded) {
                        Debug.Log("Cant Catch.");
                        return;
                    }
                    lastCell = cell;
                }
                //左键选中目的地（不是当前所在地)
                if (Input.GetMouseButtonDown(0) && curCell != cell) {
                    //减去此次的消耗
                    supply -= curpath.Sumcost;
                    MapManager.Instance.infoHUD.SetSupplyText(HUD.TextMode.Normal, supply);
                    PathManager.Instance.FreeFindPathData();
                    checkState = CheckState.InMoving;
                    return;
                }
            }
        }

        void UpdateMoving() {
            //获取离下一Cell的距离
            float dis = Vector2.Distance(curpath.Path[index].Transform.position, Transform.position);

            if (dis > 1f) {
                //如果未抵达下一Cell，Player前进
                PlayerGoTo(curpath.Path[index]);
                //Debug.Log("+1");
                //判断是否需要缓慢减少的效果
                if (MapManager.Instance.infoHUD.NeedReduceEffect()) {
                    //如果需要，则判断移动方向为垂直移动还是斜向移动（两者平均速度不同）
                    bool isOblique = false;
                    //Y轴没动说明是X轴向斜移动
                    if (curpath.Path[index].MapPos.y - CurCell.MapPos.y == 0) {
                        isOblique = true;
                    }
                    //获取每帧减少的量
                    float delta = MapManager.Instance.infoHUD.GetReduceNum(isOblique, curpath);
                    //缓慢减少
                    MapManager.Instance.infoHUD.ReduceEffectSupply(delta);
                }
            } else {
                if (index < curpath.Path.Count - 1) {
                    CurCell = curpath.Path[index];
                    index++;
                } else {
                    CurCell = curpath.Path[index];
                    index = 1; //重置辅助标记
                    checkState = CheckState.InMap;
                    CameraController.Instance.SetPosition(curCell.Transform);
                }
            }
        }
    }
}