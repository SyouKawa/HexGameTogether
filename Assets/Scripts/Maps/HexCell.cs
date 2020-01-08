using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[PrefabPath("Prefabs/Map/BasicHexCell")]
public class HexCell : ObjectBinding{
    public Vector3 pos;
    public Vector2Int MapPos;
    public SpriteRenderer Img;
    public GameData.FieldType type;

    public int fieldcost;//通过该节点本身的消耗

    public HexCell(Vector2Int _MapPos) {
        MapPos = _MapPos;
        Img = Nodes["Img"].GetComponent<SpriteRenderer>();
        SetFieldType(GameData.FieldType.EdgeSea);//默认不可行动
    }

    public HexCell(Vector2Int _MapPos,GameObject _cell) 
    {
        MapPos = _MapPos;
        Img = Nodes["Img"].GetComponent<SpriteRenderer>();
        SetFieldType(GameData.FieldType.EdgeSea);//默认不可行动
    }

    public void SetFieldType(GameData.FieldType _type) {
        type = _type;
        switch (type) {
            case GameData.FieldType.Forest : fieldcost = 5; break;
            case GameData.FieldType.Mountain: fieldcost = 10; break;
            case GameData.FieldType.Plain: fieldcost = 1; break;
        }
    }

    /// <summary>
    /// (辅助)在标签上显示信息
    /// </summary>
    public void SetDebugInfo(string str){
        TextMeshPro debuger = Source.GetComponentInChildren<TextMeshPro>();
        if(debuger != null) {
            debuger.text = MapPos.ToString() + "\n" + str;
        }
    }

    public void SetImgOrder(int order) {
        Img.sortingOrder = order;
    }

    public void SetPos() {
        TextMeshPro debuger = Nodes["DebugText"].GetComponent<TextMeshPro>();
        if (debuger != null){
            debuger.text = MapPos.ToString();
        }
    }

    public void RestDebugData() {
        Nodes["DebugImg"].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        Nodes["DebugText"].GetComponent<TextMeshPro>().text = MapPos.ToString();
    }

    public void EnableDebugData(bool state) {
        Nodes["Debuger"].SetActive(state);
    }

    //TODO: 仅供建议，未测试，可能有逻辑错误
    //通常来说 只要方法名出现了GetXXX SetXXX 都可以改用属性. 比如这里的很多Set Get
    private bool debugMode = false;
    public bool DebugMode {
        get {
            return debugMode;
        }
        set {
            debugMode = value;
            Nodes["Debuger"].SetActive(value);
            if (!debugMode) {
                Nodes["DebugImg"].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
                Nodes["DebugText"].GetComponent<TextMeshPro>().text = MapPos.ToString();
            }
        }
    }
}