using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    //地图大小
    public int width = 1;
    public int height = 6;

    ///预制体引用列表
    //基本六边形Prfb
    public GameObject cellPrefab;
    //坐标显示Prfb
    public Text cellLabelPrefab;

    ///功能子组件
    //显示坐标图层
    Canvas gridCanvas;

    HexCell[,] cells;
    private void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();

        cells = new HexCell[height , width];

        SpawnMap();

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
    /// <param name="row">所在行.</param>
    /// <param name="col">所在列.</param>
    /// <param name="pos">在世界坐标中的位置.</param>
    void CreateCell(int row,int col, Vector3 pos){
        //按坐标新建每一个cell
        cells[row, col] = new HexCell(new Vector2(row, col), Instantiate(cellPrefab));
        cells[row, col].cell.transform.position = new Vector3(pos.x + GameStaticData.ConstHorizonDis * col, pos.y + GameStaticData.MinInnerRadius * col, 0f);
        cells[row, col].cell.transform.SetParent(transform, false);
        cells[row,col].cell.name = "cell:" + row.ToString() + "," + col.ToString();
        //按照倍率调节图片缩放
        cells[row, col].Img.transform.localScale = new Vector3(GameStaticData.Rates, GameStaticData.Rates,0f);
        //显示cell的游戏坐标
        Text label = Instantiate(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(cells[row, col].cell.transform.position.x, cells[row, col].cell.transform.position.y);
        label.text = row.ToString() + "," + col.ToString();
        label.name = "(" + row.ToString() + "," + col.ToString() + ")";
    }
}