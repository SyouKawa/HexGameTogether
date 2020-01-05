using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[PrefabPath("Prefabs/Map/PlayerInMap")]
public class PlayerInMap : ObjectBinding
{
    private HexCell curCell;

    public PlayerInMap() {
        //Source.transform.position = new Vector3(curCell.MapPos.x,curCell.MapPos.y, 0f);
        //Transform.SetParent(curCell.Transform);
        //Transform.localPosition = Vector3.zero;
    }
    /// <summary>
    /// 获取当前站立的Cell
    /// </summary>
    public HexCell GetCurPosCell() {
        return curCell;
    }
}
