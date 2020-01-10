using System.Collections.Generic;
using UnityEngine;
using static GameData;

public class MapManager : Manager<MapManager> {
    //管理的地图
    public Map map { get; private set; }
    //辅助Canvas
    public Canvas helperCanvas => map.Source.GetComponentInChildren<Canvas>();

    public MapManager() { }
    public PlayerInMap playerInMap;
    public HUD PlayerInfoUI;

    public override void Start(EventHelper helper) {
        helper.OnWorldLoadEvent += SpawnMap;
        helper.AfterWorldLoadEvent += SetDebugInfoState;
    }

    /// <summary>
    /// 生成菱形地图坐标
    /// </summary>
    public void SpawnMap() {
        //实例化Map节点
        map = new Map(MapWidth, MapHeight);
        //循环生成地形Cell
        Vector3 pos;
        for (int row = 0; row < MapHeight; row++) {
            //当前行(每个col)的基准坐标
            pos = new Vector3(-ConstHorizonDis * row, MinInnerRadius * row, 0f);
            for (int col = 0; col < MapWidth; col++) {
                map.CreateCell(row, col, pos);
            }
        }
        //所有地图自动修整边界区域
        map.SpawnEdgeSea();
        //调整节点
        map.Transform.SetParent(Global.Instance.transform);
        map.Transform.name = "MapNode";

        //生成Player
        playerInMap = new PlayerInMap {
            CurCell = map.cells[7, 4]
        };
        //生成显示Player信息的CameraUI层
        PlayerInfoUI = new HUD();
    }

    /// <summary>
    /// 调整Debug信息的显示状态
    /// </summary>
    public void SetDebugInfoState() {
        for (int i = 0; i < GameData.MapHeight; i++) {
            for (int j = 0; j < GameData.MapWidth; j++) {
                map.cells[j, i].Find("Debuger").SetActive(GameData.ShowMapDebugInfo);
            }
        }
    }

    [PrefabPath("Prefabs/Map/MapBaseNode")]
    public class Map : ObjectBinding {
        public HexCell[, ] cells;

        public Map(int width, int height) {
            cells = new HexCell[width, height];
        }

        /// <summary>
        /// 创建一个单独的Cell
        /// </summary>
        /// <param name="row">所在行(45度方向坐标系的Y值).</param>
        /// <param name="col">所在列(45度方向坐标系的X值).</param>
        /// <param name="pos">在世界坐标中的位置.</param>
        public void CreateCell(int row, int col, Vector3 pos) {
            //按坐标新建每一个cell(数组下标按坐标,而非行列,所以row和col位置互换)
            cells[col, row] = new HexCell(new Vector2Int(col, row));

            // 随机地形及显示Tile
            FieldType type = GetRandomField();
            cells[col, row].FieldType = type;

            //TODO 整理这个
            List<Object[]> Tiles = ResManager.Instance.FieldTiles;
            //为该地形roll一个显示Tile
            Sprite curImg = (Sprite) (Tiles[(int) cells[col, row].FieldType][random.Next(0, Tiles[(int) cells[col, row].FieldType].Length)]);

            //cells[col, row].CellImg.GetComponent<SpriteRenderer>().sprite = curImg;
            cells[col, row].CellRenderer.sprite = curImg;

            //调整坐标,并按照row+col之和修改z值保证屏幕远近的遮挡关系(如果为湖海,则调节orderinLayer为更低层级)
            cells[col, row].Source.transform.position = new Vector3(pos.x + ConstHorizonDis * col, pos.y + MinInnerRadius * col, col + row);
            if (cells[col, row].FieldType == FieldType.Lake) {
                //cells[col, row].SetImgOrder(-1);
                cells[col, row].CellRenderer.sortingOrder = -1;
            }
            cells[col, row].Source.transform.SetParent(MapManager.Instance.map.Transform, false);
            cells[col, row].Source.name = "cell:" + col.ToString() + "," + row.ToString();

            //按照倍率调节图片缩放
            cells[col, row].CellRenderer.transform.localScale = new Vector3(Rates, Rates, 0f);
            //cells[col, row].SetPos();
            cells[col, row].DebugTextMesh.text = cells[col, row].MapPos.ToString();
        }

        /// <summary>
        /// (每次生成固定调用)生成地图时修剪边境位置为海洋
        /// </summary>
        public void SpawnEdgeSea() {
            int width = MapWidth;
            int height = MapHeight;

            List<Object[]> Tiles = ResManager.Instance.FieldTiles;
            //局部函数
            void CreatWaterCell(HexCell cell) {
                //配置浪海比例
                bool isWave = Random.Range(0, 100) > WaveRate?false : true;
                Sprite curImg = null;
                if (isWave) {
                    curImg = (Sprite) (Tiles[3][Random.Range(0, Tiles[3].Length)]);
                } else {
                    curImg = (Sprite) (Tiles[1][Random.Range(0, Tiles[1].Length)]);
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