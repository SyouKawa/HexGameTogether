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
public class HexCell : ObjectBinding{
    public Vector3 pos;
    public Vector2Int MapPos;
    public SpriteRenderer Img;
    public CellDebugText debugtext;
    public GameStaticData.FieldType type;

    public int fieldcost;//通过该节点本身的消耗

    public HexCell(Vector2Int _MapPos) {
        MapPos = _MapPos;
        Img = Transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        SetFieldType(GameStaticData.FieldType.EdgeSea);//默认不可行动
    }

    ~HexCell() {
        ObjectManager.ReturnInstantiate<HexCell>(Source);
    }

    public HexCell(Vector2Int _MapPos,GameObject _cell) 
    {
        MapPos = _MapPos;
        Img = Transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
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

    public void ShowPos() {
        debugtext = new CellDebugText();
        //显示cell的游戏坐标
        debugtext.textCom.rectTransform.SetParent(Global.Instance.mapManager.helperCanvas.transform, false);
        debugtext.textCom.rectTransform.anchoredPosition = new Vector2(Transform.position.x,Transform.position.y);
        debugtext.textCom.text = MapPos.ToString();
        debugtext.textCom.transform.parent.name = MapPos.ToString();
    }
}