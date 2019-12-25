﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class PathHelper{

    /// <summary>
    /// 获取寻路结果
    /// </summary>
    public static FpResult GetFindPathResult() {

        return null;
    }
}

/// <summary>
/// 寻路结果类
/// </summary>
public class FpResult
{
    public bool isfinded { get; set; }
    public List<HexCell> path { get; set; }
    public int sumcost { get; set; }
}

public class PathManager {

    public class FpData {
        public float fromcost;//G值:从起点到该节点的消耗(pre+filed之和)
        public float destdis;//H值:估计值,用该点到终点的曼哈顿距离充当
        public float sumCost {get => fromcost + destdis;}

        public HexCell prepathcell;//前继节点

        public FpData() {
            fromcost = 0f;
            destdis = 0f;

            prepathcell = null;
        }
    }

    public class CellsData {
        private Dictionary<HexCell, FpData> data;
        public CellsData() {
            data = new Dictionary<HexCell, FpData>();
        }

        public void Add(HexCell _cell,FpData _fpdata) {
            data.Add(_cell, _fpdata);
        }

        public FpData this[HexCell cell]{
            get { 
                    if (!data.ContainsKey(cell)) {
                        data[cell] = new FpData();
                    }
                    return data[cell]; 
                }

            set { data[cell] = value; }
        }

        public void Remove(HexCell cell) {
            data.Remove(cell);
        }
    }

    //存放目的地列表
    public List<HexCell> DestPoints;
    public HexCell Begin;
    public HashSet<HexCell> closed;
    public HashSet<HexCell>open;
    private CellsData cellsdata;

    public PathManager(){
        DestPoints = new List<HexCell>();
        closed = new HashSet<HexCell>();
        open = new HashSet<HexCell>();
        cellsdata = new CellsData();
    }

    /// <summary>
    /// 将点击选中Tile作为下一个目的地
    /// </summary>
    public void GoDestPoint(HexCell cell) {
        DestPoints.Add(cell);
        AStarFindPath(Begin, cell, FindNextCell);
        GetPath();
        ResetFindPathData();
     }
    /// <summary>
    /// 取消选中的目的地
    /// </summary>
    public void CancelPathPoint(HexCell cell) {
        //已经作为前置移动目标,且显示过路径的,不可以移除(需要一步一步移除)
        if(cell == DestPoints[DestPoints.Count - 1]) {
            DestPoints.Remove(cell);
        }
    }

    /// <summary>
    /// 显示路径
    /// </summary>
    public List<HexCell> GetPath() {
        //TODO:取消颜色显示,使用路线sprite或shader显示
        HexCell cur = DestPoints[DestPoints.Count - 1];
        List<HexCell> finalpath = new List<HexCell>();
        while(cur != Begin) {
            Debug.Log(cellsdata[cur]+" "+cellsdata[cur].prepathcell);
            cur.Img.color = new Color(1,0,0);
            finalpath.Add(cur);
            cur = cellsdata[cur].prepathcell;
        }
        finalpath.Add(Begin);
        finalpath.Reverse();
        return finalpath;
    }

    /// <summary>
    /// 获取以传入cell为中心的相邻可行进cells
    /// </summary>
    public List<HexCell> AdjacentHex(HexCell centercell)
    {
        int x = centercell.MapPos.x;
        int y = centercell.MapPos.y;
        HexCell[,] cells = MapManager.Instance.cells;
        List<HexCell> adj = new List<HexCell> {
            cells[x + 1,y + 1],
            cells[x - 1, y - 1],
            cells[x , y + 1],
            cells[x , y - 1],
            cells[x + 1 , y],
            cells[x - 1 , y] };
        for (int i = adj.Count - 1; i >= 0; i--)
        {
            //如果本身就属于closed,则在邻接表中删除该节点
            //(已经在当前路径的节点,也在closed中,所以不在下方再加入是否属于loadingPath的判断)
            if (closed.Contains(adj[i]))
            {
                adj.Remove(adj[i]);
            }
            else//如果本身不在,closed,再接着判别
            {
                //如果属于不能在当前载具情况下行动的边境海,就将其剔除,并加入closed列表
                if (adj[i].type == GameStaticData.FieldType.EdgeSea || adj[i].type == GameStaticData.FieldType.Lake)
                {
                    closed.Add(adj[i]);
                    adj.Remove(adj[i]);
                }
            }
        }
        return adj;
    }

    /// <summary>
    /// 计算传入位置在当前选择路径中的G+H(G和H权重相等)消耗,并返回
    /// </summary>
    public float BalanceSumCost(HexCell cur) {
        // G = 从起点走到当前节点的地形消耗
        float pathcost = 0f;
        if (cellsdata[cur].prepathcell == null) {
            cellsdata[cur].fromcost = 0f;
        }
        else {
            cellsdata[cur].fromcost = cellsdata[cellsdata[cur].prepathcell].fromcost + cur.fieldcost;
        }
        //H = 从该点到最新目的地的曼哈顿距离
        cellsdata[cur].destdis = Vector2Int.Distance(cur.MapPos, DestPoints[DestPoints.Count-1].MapPos);

        //G+H的值
        pathcost = cellsdata[cur].fromcost + cellsdata[cur].destdis;
        return pathcost;
      }

    /// <summary>
    /// A*寻路接口
    /// </summary>
    public void AStarFindPath(HexCell from,HexCell dest,Func<HexCell,HexCell>FindFunc) {

        //处理当前节点
        float curCost = BalanceSumCost(from);
        if (!open.Contains(from)) {
            open.Add(from);
           //cellsdata.Add(from,new FpData());
        }
        //寻找下一个节点
        HexCell nextCell = FindFunc(from);

        //如果未找到则递归调用寻找
        if(nextCell!= dest) {
            AStarFindPath(nextCell, dest, FindNextCell);
         }
        //找到时,返回到上层递归
        return;
    }

    /// <summary>
    /// 根据新传入当前节点及其邻接节点列表,更新openList
    /// </summary>
    public void UpdateOpenList(HexCell cur ,List<HexCell> adj) {
        //先算出当前邻接列表中的消耗最少的前进节点,并将所有节点加入open的待机节点
        for (int i = 0; i < adj.Count; i++)
        {

            //检查在open列表中是否存在该节点,不存在则添加.
            if (!open.Contains(adj[i]))
            {
                open.Add(adj[i]);
                //cellsdata.Add(adj[i], new FpData());
                cellsdata[adj[i]].prepathcell = cur;//直接为新临界点添加前继节点
            }
            else
            {//检查以新节点作为临时前继节点,是否消耗更低
                float newcost = adj[i].fieldcost + cellsdata[cur].fromcost + Vector2Int.Distance(adj[i].MapPos, DestPoints[DestPoints.Count - 1].MapPos);
                if (cellsdata[adj[i]].fromcost > newcost) //沿其他路径到该点的消耗比从此路径要大,则更新open中的路径(该节点的前继节点)
                {
                    //更新前继节点,保证路径的更新
                    cellsdata[adj[i]].prepathcell = cur;
                    BalanceSumCost(adj[i]);
                }
            }
            //TODO:将辅助染色显示变更为上浮显示路径
            adj[i].Img.color = new Color(0, 1, 0);
            adj[i].SetText(BalanceSumCost(adj[i]).ToString());
        }
    }

    /// <summary>
    /// 由当前节点选择出最优的下一个节点
    /// </summary>
    public HexCell FindNextCell(HexCell cur) {
        //获取入参cell的邻接cells(不包括水行区域及loadingPath,两者已被放入closed并在传入之前剔除)
        open.Remove(cur);
        cellsdata.Remove(cur);
        //只要作为过一次路径节点,则会被归入closed,防止纠错路径时再次回到该点
        closed.Add(cur);

        //获取邻接节点,并更新open列表
        List<HexCell> adj = AdjacentHex(cur);
        UpdateOpenList(cur, adj);

        //选择路径:选择open中的最小消耗节点,并沿其寻找新的路径
        float minCost = GameStaticData.infinite;
        HexCell next = null;
        foreach(HexCell cell in open) {
            //选择消耗最小的作为next
            if (cellsdata[cell].sumCost < minCost) {
                next = cell;
                minCost = cellsdata[cell].sumCost;
            }
        }
        next.Img.color = new Color(0, 0, 1);
        return next;
    }

    /// <summary>
    /// 重置每次寻路使用的存储数据
    /// </summary>
    public void ResetFindPathData(){
        //TODO:在清空open和closed之前,应该把列表中的cells的动态cost都清除掉
        closed = new HashSet<HexCell>();
        open = new HashSet<HexCell>();
        cellsdata = new CellsData();
    }

}
