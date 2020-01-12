using System.Collections.Generic;
using UnityEngine;
using static GameData;

public partial class MapManager : Manager<MapManager> {
    //管理的地图
    private MapObj map { get; set; }

    /// <summary>
    /// 通过这个接口来访问Cell避免越界
    /// </summary>
    public HexCell GetCell(int x, int y) {
        if (x < 0 || x > MapWidth || y < 0 || y > MapHeight) {
            Log.Warning("访问越界,试图访问坐标[{0},{1}]", x, y);
        }
        return map.Cells[x, y];
    }
    public HexCell GetCell(Vector2Int vec) {
        return GetCell(vec.x, vec.y);
    }
    public void SetCell(Vector2Int mapPos, FieldType type){
        map.SetCell(mapPos,type);
    }


    public MapManager() { }

    public HUD infoHUD;

    public override void Start(EventHelper helper) {
        helper.OnWorldLoadEvent += SpawnMap;
        helper.AfterWorldLoadEvent += SetDebugInfoState;
        helper.AfterWorldLoadEvent += InitBuiding;
    }

    public void SpawnMap() {
        //实例化Map节点
        map = new MapObj();

        //生成显示Player信息的CameraUI层
        infoHUD = new HUD();
    }

    /// <summary>
    /// 调整Debug信息的显示状态
    /// </summary>
    public void SetDebugInfoState() {
        for (int i = 0; i < GameData.MapHeight; i++) {
            for (int j = 0; j < GameData.MapWidth; j++) {
                map.Cells[j, i].Find("Debuger").SetActive(GameData.ShowMapDebugInfo);
            }
        }
    }

    [PrefabPath("Prefabs/Map/MapBaseNode")]
    private class MapObj : PrefabBinding {
        private HexCell[, ] cells;

        public HexCell[, ] Cells { get => cells; set => cells = value; }

        int width = GameData.MapWidth;
        int height = GameData.MapHeight;

        public MapObj() {
            //调整节点
            Transform.SetParent(Global.Instance.transform);
            Transform.name = "MapNode";

            cells = new HexCell[width, height];
            //循环生成地形Cell
            for (int row = 0; row < MapHeight; row++) {
                for (int col = 0; col < MapWidth; col++) {
                    Vector2Int mapPos = new Vector2Int(col, row);
                    cells[col, row] = new HexCell(mapPos);
                    SetCell(mapPos, GetRandomField());
                }
            }
            //所有地图自动修整边界区域
            SetEdgeSea();
        }

        /// <summary>
        /// 创建一个单独的Cell
        /// </summary>
        /// <param name="row">所在行(45度方向坐标系的Y值).</param>
        /// <param name="col">所在列(45度方向坐标系的X值).</param>
        public void SetCell(Vector2Int mapPos, FieldType type) {
            int col = mapPos.x;
            int row = mapPos.y;
            //当前基准坐标
            Vector3 pos = new Vector3(-ConstHorizonDis * row, MinInnerRadius * row, 0f);

            //按坐标新建每一个cell(数组下标按坐标,而非行列,所以row和col位置互换)
            cells[col, row].FieldType = type;

            //为该地形roll一个显示Tile
            var Tiles = ResManager.Instance.FieldTiles;
            Sprite curImg = Tiles[cells[col, row].FieldType][random.Next(0, Tiles[cells[col, row].FieldType].Count)];

            //设置cell贴图
            cells[col, row].CellRenderer.sprite = curImg;

            //调整坐标,并按照row+col之和修改z值保证屏幕远近的遮挡关系(如果为湖海,则调节orderinLayer为更低层级)
            cells[col, row].Source.transform.position = new Vector3(pos.x + ConstHorizonDis * col, pos.y + MinInnerRadius * col, col + row);
            if (cells[col, row].FieldType == FieldType.Lake) {
                cells[col, row].CellRenderer.sortingOrder = -1;
            }
            cells[col, row].Transform.SetParent(Transform, false);
            cells[col, row].Name = Utils.FormatString("Cell:{0},{1}", col.ToString(), row.ToString());

            //按照倍率调节图片缩放
            cells[col, row].CellRenderer.transform.localScale = GameData.RatesV3;
            //设置DebugText
            cells[col, row].DebugTextMesh.text = cells[col, row].MapPos.ToString();
        }

        /// <summary>
        /// (每次生成固定调用)生成地图时修剪边境位置为海洋
        /// </summary>
        public void SetEdgeSea() {
            int width = MapWidth;
            int height = MapHeight;

            //局部函数
            void CreatWaterCell(HexCell cell) {
                //配置浪海比例
                bool isWave = Random.Range(0, 100) > WaveRate?false : true;
                Sprite curImg = null;
                if (isWave) {
                    curImg = ResManager.Instance.GetRandomFieldImg(FieldType.EdgeSea);
                } else {
                    curImg = ResManager.Instance.GetRandomFieldImg(FieldType.Lake);
                }
                cell.CellRenderer.sprite = curImg;
                cell.CellRenderer.sortingOrder = -1;
                cell.FieldType = FieldType.EdgeSea;
            }

            for (int i = 0; i < width; i++) {
                //下宽
                CreatWaterCell(cells[i, 0]);
                //上宽
                CreatWaterCell(cells[i, height - 1]);
            }
            for (int i = 0; i < height; i++) {
                //左高
                CreatWaterCell(cells[0, i]);
                //右高
                CreatWaterCell(cells[width - 1, i]);
            }
        }

        /// <summary>
        /// 随机产生一个地形类型
        /// </summary>
        public FieldType GetRandomField() {
            int roll = random.Next(0, 100);

            if (roll <= FieldGenRate["Lake"]) {
                return FieldType.Lake;
            }
            if (roll > FieldGenRate["Lake"] & roll <= FieldGenRate["Moun"]) {
                return FieldType.Mountain;
            }
            if (roll > FieldGenRate["Moun"] & roll <= FieldGenRate["Forest"]) {
                return FieldType.Forest;
            }
            if (roll > FieldGenRate["Forest"] & roll <= FieldGenRate["Plain"]) {
                return FieldType.Plain;
            }
            return FieldType.EdgeSea; //默认值
        }
    }

}

public partial class MapManager {

    public void InitBuiding() {
        SetCell(new Vector2Int(2, 2), FieldType.Plain);
        SetCell(new Vector2Int(2, 3), FieldType.Plain);
        SetCell(new Vector2Int(3, 3), FieldType.Plain);
        SetCell(new Vector2Int(3, 2), FieldType.Plain);
        GetCell(2,2).ShowBuiding(HexCell.BuildingType.Portal);
        GetCell(2,3).ShowBuiding(HexCell.BuildingType.Shop);
        GetCell(3,2).ShowBuiding(HexCell.BuildingType.Station);
        GetCell(3,3).ShowBuiding(HexCell.BuildingType.Union);
    }

}