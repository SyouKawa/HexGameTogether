using System.Collections.Generic;
using UnityEngine;
using static GameStaticData;

//TODO:添加MapHelper存放对外接口

public class MapManager : Singleton<MapManager> {
    //管理的地图
    public Map map { get; private set; }
    //辅助Canvas
    public Canvas helperCanvas => map.Source.GetComponentInChildren<Canvas>();

    public MapManager() {
        //TODO:UnityDebug时地图大小从Global取,
        //Update: 已经改为从StaticData中读取了
    }



    public PlayerInMap player;

    public override void Start(EventHelper helper) {
        helper.OnWorldLoadEvent += SpawnMap;
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
        map.SetEdgeSea();
        //调整节点
        map.Transform.SetParent(Global.Instance.transform);
        map.Transform.name = "MapNode";

        //生成Player
        player = new PlayerInMap {
            CurCell = map.cells[2, 2]
        };
    }
}

[PrefabPath("Prefabs/Map/MapBaseNode")]
public class Map : ObjectBinding {
    public HexCell[,] cells;

    public Map(int width, int height) {
        cells = new HexCell[height, width];
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
        FieldType type = RandomFieldByRates();
        cells[col, row].SetFieldType(type);
        List<Object[]> Tiles = ResManager.GetInstance().FieldTiles;

        //为该地形roll一个显示Tile
        Sprite curImg = (Sprite)(Tiles[(int)cells[col, row].type][random.Next(0, Tiles[(int)cells[col, row].type].Length)]);
        cells[col, row].Img.GetComponent<SpriteRenderer>().sprite = curImg;

        //调整坐标,并按照row+col之和修改z值保证屏幕远近的遮挡关系(如果为湖海,则调节orderinLayer为更低层级)
        cells[col, row].Source.transform.position = new Vector3(pos.x + ConstHorizonDis * col, pos.y + MinInnerRadius * col, col + row);
        if (cells[col, row].type == FieldType.Lake) {
            cells[col, row].SetImgOrder(-1);
        }
        cells[col, row].Source.transform.SetParent(MapManager.GetInstance().map.Transform, false);
        cells[col, row].Source.name = "cell:" + col.ToString() + "," + row.ToString();

        //按照倍率调节图片缩放
        cells[col, row].Img.transform.localScale = new Vector3(Rates, Rates, 0f);
        cells[col, row].ShowPos();
    }

    /// <summary>
    /// (每次生成固定调用)生成地图时修剪边境位置为海洋
    /// </summary>
    public void SetEdgeSea() {
        //int width = Global.Instance.mapManager.width;
        int width = MapWidth;

        List<Object[]> Tiles = ResManager.GetInstance().FieldTiles;
        for (int i = 0; i < width; i++) {
            Sprite curImg = (Sprite)(Tiles[3][Random.Range(0, Tiles[3].Length)]);
            cells[0, i].Img.GetComponent<SpriteRenderer>().sprite = curImg;
            cells[0, i].SetImgOrder(-1);

            cells[0, i].type = FieldType.EdgeSea;
            cells[i, 0].Img.GetComponent<SpriteRenderer>().sprite = curImg;
            cells[i, 0].SetImgOrder(-1);
            cells[i, 0].type = FieldType.EdgeSea;
            //TODO:后排换成纯水
            cells[width - 1, i].Img.GetComponent<SpriteRenderer>().sprite = curImg;
            cells[width - 1, i].SetImgOrder(-1);
            cells[width - 1, i].type = FieldType.EdgeSea;
            cells[i, width - 1].Img.GetComponent<SpriteRenderer>().sprite = curImg;
            cells[i, width - 1].SetImgOrder(-1);
            cells[i, width - 1].type = FieldType.EdgeSea;
        }
    }

    /// <summary>
    /// 随机产生一个地形类型
    /// </summary>
    public GameStaticData.FieldType RandomFieldByRates() {
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
        return FieldType.EdgeSea;//默认值
    }
}
