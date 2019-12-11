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
        for(int i = 0,col = 0;col<width ;col++) { 
            for(int row = 0;row<height ;row++ ) {
                SpawnCell(col, row, i++);
            }
        }
    }

    void SpawnCell(int col,int row,int i) {
        Vector3 pos;
        pos.x = col * (GameStaticData.MaxOuterRadius * 1.3f);//每个六边形的间距为最大外径的1.3倍
        pos.y = (row + (col * 0.5f - col / 2)) * (GameStaticData.MinInnerRadius*2f);//(奇数列向右偏移半个六边形,偶数列不偏)平角向上-列优先排列  的 高度差为最小内径的2倍
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
