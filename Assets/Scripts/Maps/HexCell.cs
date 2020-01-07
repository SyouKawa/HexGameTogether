using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[PrefabPath("Prefabs/Map/BasicHexCell")]
public class HexCell : ObjectBinding{
    public Vector3 pos;
    public Vector2Int MapPos;
    public SpriteRenderer Img;
    public GameStaticData.FieldType type;

    public int fieldcost;//通过该节点本身的消耗

    public HexCell(Vector2Int _MapPos) {
        MapPos = _MapPos;
        Img = Nodes["Img"].GetComponent<SpriteRenderer>();
        SetFieldType(GameStaticData.FieldType.EdgeSea);//默认不可行动
    }

    public HexCell(Vector2Int _MapPos,GameObject _cell) 
    {
        MapPos = _MapPos;
        Img = Nodes["Img"].GetComponent<SpriteRenderer>();
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

    /// <summary>
    /// (辅助)在标签上显示信息
    /// </summary>
    public void SetDebugInfo(string str){
        TextMesh debuger = Source.GetComponentInChildren<TextMesh>();
        debuger.text = debuger.text + "\n" + str;
    }

    public void SetImgOrder(int order) {
        Img.sortingOrder = order;
    }

    public void SetPos() {
        TextMeshPro debuger = Nodes["DebugText"].GetComponent<TextMeshPro>();
        debuger.text = MapPos.ToString();
    }
}