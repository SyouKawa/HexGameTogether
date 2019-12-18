using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathManager : MonoBehaviour
{
    public static PathManager Instance { get; private set; }

    public List<HexCell> PathPoints;
    public List<HexCell> SearchCells;
    public Dictionary<int,HexCell> closed;
    public Dictionary<int, HexCell> open;

    private void Awake(){
        Instance = this;
        PathPoints = new List<HexCell>();
        SearchCells = new List<HexCell>();
        closed = new Dictionary<int,HexCell>();
        open = new Dictionary<int, HexCell>();
    }

    private void Start()
    {
        
    }

    public void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null){
            //collider处于Img上,而存储的是Img的父节点的Hash
            int tempHash = hit.collider.transform.parent.gameObject.GetHashCode();
            HexCell curCell = MapManager.Instance.CellObjects[tempHash];
            //Debug.Log("Target MapPosition: " + MapManager.Instance.CellObjects[tempHash].MapPos);

            if (Input.GetMouseButton(0)) {
                AddPathPoint(curCell);
             }
            if (Input.GetMouseButton(1)) {
                CancelPathPoint(curCell);
             }
        }
    }

    /// <summary>
    /// 将点击选中Tile作为下一个目的地
    /// </summary>
    public void AddPathPoint(HexCell cell) {
        PathPoints.Add(cell);
        var pos = MapManager.Instance.testFrom;
        AStarFindPath(MapManager.Instance.cells[pos.x,pos.y], cell, ShowAdj);
     }
    /// <summary>
    /// 取消选中的目的地
    /// </summary>
    public void CancelPathPoint(HexCell cell) {
        //AStarFindPath(cell, cell, HideAdj);
    }

    /// <summary>
    /// 寻路
    /// </summary>
    public void AStarFindPath(HexCell from,HexCell dist,Func<List<HexCell>,HexCell,HexCell >Judge) {
        List<HexCell>tempList = MapManager.Instance.AdjacentHex(from);
        HexCell nextCell = Judge(tempList,dist);

        if (!closed.ContainsKey(from.GetHashCode())) {
            closed.Add(from.GetHashCode(), from);
        }

        //print("next is "+nextCell.MapPos);
        if(nextCell.GetHashCode() != dist.GetHashCode()) {
            AStarFindPath(nextCell, dist, ShowAdj);
         }
        else { 
            print("finded!");
        }
        //print("now return"+from.MapPos);
        return;
    }

    public HexCell ShowAdj(List<HexCell>adj,HexCell dest) {
        float minCost = GameStaticData.infinite;
        int minIndex = 0;
        for(int i=0;i<adj.Count ;i++) {
            if (closed.ContainsKey(adj[i].GetHashCode())) continue;
            closed.Add(adj[i].GetHashCode(), adj[i]);
            adj[i].Img.color = new Color(0, 1, 0);
            float eva = Vector2Int.Distance(adj[i].MapPos, dest.MapPos);
            float cost = adj[i].passCost + eva;
            if(minCost > cost) {
                minCost = cost;
                minIndex = i;
            }
        }
        adj[minIndex].Img.color = new Color(0, 0, 0);
        return adj[minIndex];
    }

    public HexCell HideAdj(List<HexCell> adj)
    {
        for (int i = 0; i < adj.Count; i++)
        {
            adj[i].Img.color = new Color(1, 1, 1);
        }
        return null;
    }

}
