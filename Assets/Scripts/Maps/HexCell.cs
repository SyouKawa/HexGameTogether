using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell
{
    public Vector3 pos;
    public Vector2 MapPos;
    public GameObject cell;
    public GameObject Img;

    public HexCell(Vector2 _MapPos,GameObject _cell) 
    {
        MapPos = _MapPos;
        cell = _cell;
        Img = cell.transform.GetChild(0).gameObject;
    }
}
