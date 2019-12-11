using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    //地图大小
    public int width = 6;
    public int height = 6;

    ///预制体引用列表
    //基本六边形Prfb
    public HexCell cellPrefab;
    //坐标显示Prfb
    public Text cellLabelPrefab;

    ///功能子组件
    //显示坐标图层
    Canvas gridCanvas;

    HexCell[] cells;
    private void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();

        cells = new HexCell[height * width];
        for(int i = 0,row = 0;row<height ;row++) { 
            for(int col = 0;col<width ;col++ ) {
                SpawnCell(row, col, i++);
            }
        }
    }

    void SpawnCell(int row,int col,int i) {
        Vector3 pos;
        pos.x = (col + (row * 0.5f - row / 2)) * (GameStaticData.innerRadius * 2f);//偏移量(奇数行向右偏移半个六边形,偶数行不偏)+每个六边形的间距
        pos.y = row * (GameStaticData.outerRadius * 1.5f);//高度差为外径的1.5倍
        pos.z = 0f;
        //按坐标新建每一个cell
        HexCell cell = cells[i] = Instantiate(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = pos;
        cell.name = "cell:" + row.ToString() + "," + col.ToString();

        //显示cell的游戏坐标
        Text label = Instantiate(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(pos.x, pos.y);
        label.text = row.ToString() + "," + col.ToString();
        label.name = "("+ row.ToString() + "," + col.ToString()+")";
    }
}
