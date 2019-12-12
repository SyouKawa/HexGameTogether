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

        cells[2, 1].Img.GetComponent<SpriteRenderer>().sprite = null;

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
        //显示cell的游戏坐标
        Text label = Instantiate(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(cells[col, row].cell.transform.position.x, cells[col, row].cell.transform.position.y);
        label.text = col.ToString() + "," + row.ToString();
        label.name = "(" + col.ToString() + "," + row.ToString() + ")";
    }
}