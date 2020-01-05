using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO:添加MapHelper存放对外接口

public class MapManager : Singleton<MapManager>
{
    //地图大小
    public int width { get; private set; }
    public int height { get; private set; }
    //管理的地图
    public Map map { get; private set; }
    //辅助Canvas
    public Canvas helperCanvas { get { return map.Source.GetComponentInChildren<Canvas>(); } }

    public MapManager()
    {
        //TODO:UnityDebug时地图大小从Global取,
        //Demo时代码改为从配置中读
        width = Global.Instance.width;
        height = Global.Instance.height;
    }

    /// <summary>
    /// 生成菱形地图坐标
    /// </summary>
    public void SpawnMap()
    {
        //实例化Map节点
        map = new Map(width, height);
        //循环生成地形Cell
        Vector3 pos;
        for (int row = 0; row < height; row++)
        {
            //当前行(每个col)的基准坐标
            pos = new Vector3(-GameStaticData.ConstHorizonDis * row, GameStaticData.MinInnerRadius * row, 0f);
            for (int col = 0; col < width; col++)
            {
                map.CreateCell(row, col, pos);
            }
        }
        //所有地图自动修整边界区域
        map.SetEdgeSea();
        //调整节点
        map.Transform.SetParent(Global.Instance.transform);
        map.Transform.name = "MapNode";
    }
}

[PrefabPath("Prefabs/Map/MapBaseNode")]
public class Map : ObjectBinding
{
    public HexCell[,] cells;
    public Dictionary<int, HexCell> CellObjects;

    public Map(int width, int height)
    {
        CellObjects = new Dictionary<int, HexCell>();
        cells = new HexCell[height, width];
    }

    ~Map()
    {
        ObjectManager.ReturnInstantiate<Map>(Source);
    }

    /// <summary>
    /// 创建一个单独的Cell
    /// </summary>
    /// <param name="row">所在行(45度方向坐标系的Y值).</param>
    /// <param name="col">所在列(45度方向坐标系的X值).</param>
    /// <param name="pos">在世界坐标中的位置.</param>
    public void CreateCell(int row, int col, Vector3 pos)
    {
        //按坐标新建每一个cell(数组下标按坐标,而非行列,所以row和col位置互换)
        cells[col, row] = new HexCell(new Vector2Int(col, row));
        //将cell加入Hash表,保证寻路时射线获取GameObject时,可按照Hash与cell对应
        CellObjects.Add(cells[col, row].Source.GetHashCode(), cells[col, row]);

        // 随机地形及显示Tile
        GameStaticData.FieldType type = RandomFieldByRates();
        cells[col, row].SetFieldType(type);
        List<Object[]> Tiles = ResManager.GetInstance().FieldTiles;

        //为该地形roll一个显示Tile
        Sprite curImg = (Sprite)(Tiles[(int)cells[col, row].type][GameStaticData.random.Next(0, Tiles[(int)cells[col, row].type].Length)]);
        cells[col, row].Img.GetComponent<SpriteRenderer>().sprite = curImg;

        //调整坐标,并按照row+col之和修改z值保证屏幕远近的遮挡关系(如果为湖海,则调节orderinLayer为更低层级)
        cells[col, row].Source.transform.position = new Vector3(pos.x + GameStaticData.ConstHorizonDis * col, pos.y + GameStaticData.MinInnerRadius * col, col + row);
        if (cells[col, row].type == GameStaticData.FieldType.Lake)
        {
            cells[col, row].SetImgOrder(-1);
        }
        cells[col, row].Source.transform.SetParent(Global.Instance.mapManager.map.Transform, false);
        cells[col, row].Source.name = "cell:" + col.ToString() + "," + row.ToString();

        //按照倍率调节图片缩放
        cells[col, row].Img.transform.localScale = new Vector3(GameStaticData.Rates, GameStaticData.Rates, 0f);
        cells[col, row].ShowPos();
    }

    /// <summary>
    /// (每次生成固定调用)生成地图时修剪边境位置为海洋
    /// </summary>
    public void SetEdgeSea()
    {
        int width = Global.Instance.mapManager.width;
        List<Object[]> Tiles = ResManager.GetInstance().FieldTiles;
        for (int i = 0; i < width; i++)
        {
            Sprite curImg = (Sprite)(Tiles[3][Random.Range(0, Tiles[3].Length)]);
            cells[0, i].Img.GetComponent<SpriteRenderer>().sprite = curImg;
            cells[0, i].SetImgOrder(-1);

            cells[0, i].type = GameStaticData.FieldType.EdgeSea;
            cells[i, 0].Img.GetComponent<SpriteRenderer>().sprite = curImg;
            cells[i, 0].SetImgOrder(-1);
            cells[i, 0].type = GameStaticData.FieldType.EdgeSea;
            //TODO:后排换成纯水
            cells[width - 1, i].Img.GetComponent<SpriteRenderer>().sprite = curImg;
            cells[width - 1, i].SetImgOrder(-1);
            cells[width - 1, i].type = GameStaticData.FieldType.EdgeSea;
            cells[i, width - 1].Img.GetComponent<SpriteRenderer>().sprite = curImg;
            cells[i, width - 1].SetImgOrder(-1);
            cells[i, width - 1].type = GameStaticData.FieldType.EdgeSea;
        }
    }

    /// <summary>
    /// 随机产生一个地形类型
    /// </summary>
    public GameStaticData.FieldType RandomFieldByRates()
    {
        int roll = GameStaticData.random.Next(0, 100);

        if (roll <= GameStaticData.FieldGenRate["Lake"])
        {
            return GameStaticData.FieldType.Lake;
        }
        if (roll > GameStaticData.FieldGenRate["Lake"] & roll <= GameStaticData.FieldGenRate["Moun"])
        {
            return GameStaticData.FieldType.Mountain;
        }
        if (roll > GameStaticData.FieldGenRate["Moun"] & roll <= GameStaticData.FieldGenRate["Forest"])
        {
            return GameStaticData.FieldType.Forest;
        }
        if (roll > GameStaticData.FieldGenRate["Forest"] & roll <= GameStaticData.FieldGenRate["Plain"])
        {
            return GameStaticData.FieldType.Plain;
        }
        return GameStaticData.FieldType.EdgeSea;//默认值
    }
}
