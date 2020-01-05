using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[PrefabPath("Prefabs/Map/PlayerInMap")]
public class PlayerInMap : ObjectBinding
{
    private HexCell curCell;

    public PlayerInMap() {
    }

    public HexCell CurCell {
        get { return curCell; }
        set {
            Transform.SetParent(value.Transform);
            Transform.localPosition = Vector3.zero;
            curCell = value;
            CameraController.GetInstance().SetPosition(curCell.Transform);
        }
    }

}
