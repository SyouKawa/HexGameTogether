﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    //地图大小
    public int width;
    public int height;

    ///预制体引用列表
    //基本六边形Prfb
    public GameObject cellPrefab;
    //坐标显示Prfb
    public Text cellLabelPrefab;
    //加载显示的Tile数组
    List<Object[]> FieldTiles = new List<Object[]>();

    ///功能子组件
    //显示坐标图层
    Canvas gridCanvas;

    HexCell[,] cells;
    private void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();

        cells = new HexCell[height , width];

        LoadRes();

        SpawnMap();
        SetEdgeSea();

    }
    /// <summary>
    /// 加载游戏需要的资源
    /// </summary>
    void LoadRes() {
        //加载显示的Tile数组
        int length = System.Enum.GetValues(typeof(GameStaticData.FieldType)).Length;
        for(int i=0;i<length ;i++ ) {
            print("Sprites/tiles_" + ((GameStaticData.FieldType)i).ToString() + "_colored");
            FieldTiles.Add(Resources.LoadAll("Sprites/tiles_"+((GameStaticData.FieldType)i).ToString()+"_colored", typeof(Sprite)));
        }
    }

    void RandomGenerateField(HexCell curcell) {
        //为当前cell roll 一个地形
        curcell.SetFieldType(RandomFieldByRates());
        //为该地形roll一个显示Tile
        Sprite curImg = (Sprite)(FieldTiles[(int)curcell.type][Random.Range(0, FieldTiles[(int)curcell.type].Length)]);
        curcell.Img.GetComponent<SpriteRenderer>().sprite = curImg;
    }

    GameStaticData.FieldType RandomFieldByRates()
    {
        int roll = Random.Range(0, 100);
        print("cur roll" + roll);

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

    void SetEdgeSea(){
        for(int i=0;i<width ;i++) {
            Sprite curImg = (Sprite)(FieldTiles[3][Random.Range(0, FieldTiles[3].Length)]);
            cells[0, i].Img.GetComponent<SpriteRenderer>().sprite = curImg;
            cells[i,0].Img.GetComponent<SpriteRenderer>().sprite = curImg;
            cells[width - 1, i].Img.GetComponent<SpriteRenderer>().sprite = curImg;
            cells[i, width - 1].Img.GetComponent<SpriteRenderer>().sprite = curImg;
        }
    }

    /// <summary>
    /// 生成菱形地图坐标
    /// </summary>
    void SpawnMap() {
        Vector3 pos;
        for (int row = 0;row<height;row++) {
            //当前行(每个col)的基准坐标
            pos = new Vector3(-GameStaticData.ConstHorizonDis*row,GameStaticData.MinInnerRadius*row,0f);
            for(int col=0;col<width;col++) {
                CreateCell(row, col, pos);
            }
        }
    }

    /// <summary>
    /// 创建一个单独的Cell
    /// </summary>
    /// <param name="row">所在行(45度方向坐标系的Y值).</param>
    /// <param name="col">所在列(45度方向坐标系的X值).</param>
    /// <param name="pos">在世界坐标中的位置.</param>
    void CreateCell(int row,int col, Vector3 pos){
        //按坐标新建每一个cell(数组下标按坐标,而非行列,所以row和col位置互换)
        cells[col, row] = new HexCell(new Vector2(row, col), Instantiate(cellPrefab));
        cells[col, row].cell.transform.position = new Vector3(pos.x + GameStaticData.ConstHorizonDis * col, pos.y + GameStaticData.MinInnerRadius * col, 0f);
        cells[col, row].cell.transform.SetParent(transform, false);
        cells[col, row].cell.name = "cell:" + col.ToString() + "," + row.ToString();
        //按照倍率调节图片缩放
        cells[col, row].Img.transform.localScale = new Vector3(GameStaticData.Rates, GameStaticData.Rates,0f);
        // 随机地形及显示Tile
        RandomGenerateField(cells[col, row]);

        //显示cell的游戏坐标
        Text label = Instantiate(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(cells[col, row].cell.transform.position.x, cells[col, row].cell.transform.position.y);
        label.text = col.ToString() + "," + row.ToString();
        label.name = "(" + col.ToString() + "," + row.ToString() + ")";
    }
}