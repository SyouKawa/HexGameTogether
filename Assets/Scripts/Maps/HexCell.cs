using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddPool("Prefabs/Map/BasicHexCell")]
public class HexCell
{
    public Vector3 pos;
    public Vector2Int MapPos;
    public GameObject cell;
    public SpriteRenderer Img;
    public UnityEngine.UI.Text text;
    public GameStaticData.FieldType type;

    public int fieldcost;//通过该节点本身的消耗
    public float dynamicost;//G值:从起点到该节点的消耗(pre+filed之和)
    public float destdis;//H值:估计值,用该点到终点的曼哈顿距离充当
    public HexCell prepathcell;

    public HexCell(Vector2Int _MapPos) {
        MapPos = _MapPos;
        cell = ObjectManager.GetInstantiate(this);
        Img = cell.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        SetFieldType(GameStaticData.FieldType.EdgeSea);//默认不可行动
        ResetFindPathData();
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

    public void SetText(string str) {
        text.text = text.text + "\n" + str;
    }
    /// <summary>
    /// 重置寻路时所需要的数据
    /// </summary>
    public void ResetFindPathData() {
        fieldcost = 0;
        dynamicost = 0;
        destdis = 0;
        prepathcell = null;
    }
}
