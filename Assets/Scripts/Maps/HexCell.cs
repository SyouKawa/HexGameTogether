using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell
{
    public Vector3 pos;
    public Vector2Int MapPos;
    public GameObject cell;
    public SpriteRenderer Img;
    public GameStaticData.FieldType type;
    public int passCost;

    public HexCell(Vector2Int _MapPos) {
        MapPos = _MapPos;
        CreateGameObject();
    }

    public void CreateGameObject() {
        cell = ObjectManager.GetInstantiate(this);
        Img = cell.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        SetFieldType(GameStaticData.FieldType.EdgeSea);//默认不可行动
    }

    public void DeleteGameObject() {
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
            case GameStaticData.FieldType.Forest : passCost = 3; break;
            case GameStaticData.FieldType.Mountain: passCost = 10; break;
            case GameStaticData.FieldType.Plain: passCost = 1; break;
        }
    }

    public void SetImgOrder(int order) {
        Img.sortingOrder = order;
    }
}
