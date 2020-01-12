using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
/*
HexCell:
    Img -> CellImg
    type -> fieldType

public变量和属性均使用首字母大写了

ObjectBinding:
   Nodes -> Find

MapManager: 
    最好没有返回值不要用Get 有返回值的用Get,正好是反过来的
    同理 有参数的可能会用Set,没有参数的肯定不能是Set
    SetEdgeSea -> SpawnEdgeSea 
    SetWaterCell -> CreatWaterCell 
    RandomFieldByRates-> GetRandomField 

CameraUI -> HUD

Path:
    FpResult(bool fail) -> FpResult.FailResult
    FPHelper -> FPManager 清除怎么被注释掉了
*/
[PrefabPath("Prefabs/Map/BasicHexCell")]
public partial class HexCell : PrefabBinding {

    public HexCell(Vector2Int _MapPos) {
        MapPos = _MapPos;
        CellRenderer = Find("CellImg").GetComponent<SpriteRenderer>();
        DebugTextMesh = Find("DebugText").GetComponent<TextMeshPro>();
        DebugBGRenderer = Find("DebugImg").GetComponent<SpriteRenderer>();

        InitBuilding();
    }

    /// <summary>
    /// 对应的地图坐标
    /// </summary>
    public Vector2Int MapPos;

    //图块组件
    public SpriteRenderer CellRenderer;

    /// <summary>
    /// 地形的消耗
    /// </summary>
    public int FieldCost { get; set; }

    private FieldType fieldType = FieldType.EdgeSea;
    /// <summary>
    /// 地形
    /// </summary>
    public FieldType FieldType {
        get {
            return fieldType;
        }
        set {
            fieldType = value;
            switch (value) {
                case FieldType.Forest:
                    FieldCost = 5;
                    break;
                case FieldType.Mountain:
                    FieldCost = 10;
                    break;
                case FieldType.Plain:
                    FieldCost = 1;
                    break;
            }
        }
    }

    public TextMeshPro DebugTextMesh { get; set; }

    public SpriteRenderer DebugBGRenderer { get; set; }
}

public partial class HexCell {
    public SpriteRenderer BuildingRenderer;

    public bool HasBuilding = false;

    public enum BuildingType {
        Shop,
        Portal,
        Union,
        Station
    }

    public BuildingType buildingType;

    public void InitBuilding() {
        BuildingRenderer = Find("Building").GetComponent<SpriteRenderer>();
        Find("BuildingRoot").transform.localScale = GameData.RatesV3;
    }

    public void ShowBuiding(BuildingType type) {
        HasBuilding = true;
        buildingType = type;
        BuildingRenderer.gameObject.SetActive(true);
        Sprite img;
        switch (type) {
            case BuildingType.Portal:
                img = ResourceHelper.Instance.PortalImg;
                break;
            case BuildingType.Shop:
                img = ResourceHelper.Instance.ShopImg;
                break;
            case BuildingType.Station:
                img = ResourceHelper.Instance.StationImg;
                break;
            case BuildingType.Union:
                img = ResourceHelper.Instance.UnionImg;
                break;
            default:
                img = ResourceHelper.Instance.PortalImg;
                break;
        }
        BuildingRenderer.sprite = img;
    }
}