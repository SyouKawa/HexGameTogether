using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddPool("Prefabs/Map/CellDebugText")]
public class CellDebugText : ObjectBinding {
    public Text textCom;

    public CellDebugText() {
        textCom = Source.GetComponent<Text>();
    }

    /// <summary>
    /// (辅助)在标签上显示信息
    /// </summary>
    public void SetText(string str){
        textCom.text = textCom.text + "\n" + str;
    }
}

[AddPool("Prefabs/Map/BasicHexCell")]
public class HexCell
{
    public Vector3 pos;
    public Vector2Int MapPos;
    public GameObject cell;
    public SpriteRenderer Img;
    public CellDebugText debugtext;
    public GameStaticData.FieldType type;

    public int fieldcost;//通过该节点本身的消耗

    public HexCell(Vector2Int _MapPos) {
        MapPos = _MapPos;
        cell = ObjectManager.GetInstantiate(this);
        Img = cell.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        SetFieldType(GameStaticData.FieldType.EdgeSea);//默认不可行动
    }

    ~HexCell() {
        ObjectManager.ReturnInstantiate<HexCell>(cell);
    }


    public HexCell(Vector2Int _MapPos,GameObject _cell) 
    {
        MapPos = _MapPos;
        cell = _cell;
        Img = cell.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        SetFieldType(GameStaticData.FieldType.EdgeSea);//默认不可行动
    }

    public void SetFieldType(GameStaticData.FieldType _type) {
        type = _type;
        switch (type) {
            case GameStaticData.FieldType.Forest : fieldcost = 5; break;
            case GameStaticData.FieldType.Mountain: fieldcost = 10; break;
            case GameStaticData.FieldType.Plain: fieldcost = 1; break;
        }
    }

    public void SetImgOrder(int order) {
        Img.sortingOrder = order;
    }

    private void ShowPos() {
        //显示cell的游戏坐标
        debugtext = new CellDebugText();
        debugtext.rectTransform.SetParent(gridCanvas.transform, false);
        debugtext.rectTransform.anchoredPosition = new Vector2(cells[col, row].cell.transform.position.x, cells[col, row].cell.transform.position.y);
        debugtext.text = col.ToString() + "," + row.ToString();
        debugtext.name = "(" + col.ToString() + "," + row.ToString() + ")";
    }

}
