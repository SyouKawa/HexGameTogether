using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathManager : MonoBehaviour
{
    public static PathManager Instance { get; private set; }

    //存放目的地列表
    public List<HexCell> DestPoints;
    public Dictionary<int,HexCell> loadingPath;
    public Dictionary<int,HexCell> closed;
    public Dictionary<HexCell, float> open;

    private void Awake(){
        Instance = this;
        DestPoints = new List<HexCell>();
        loadingPath = new Dictionary<int, HexCell>();
        closed = new Dictionary<int,HexCell>();
        open = new Dictionary<HexCell, float>();
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

            if (Input.GetMouseButtonDown(0)) {
                AddPathPoint(curCell);
             }
            if (Input.GetMouseButtonDown(1)) {
                CancelPathPoint(curCell);
             }
        }
    }

    /// <summary>
    /// 将点击选中Tile作为下一个目的地
    /// </summary>
    public void AddPathPoint(HexCell cell) {
        DestPoints.Add(cell);
        var pos = MapManager.Instance.testFrom;
        AStarFindPath(MapManager.Instance.cells[pos.x,pos.y], cell, FindNextCell);
     }
    /// <summary>
    /// 取消选中的目的地
    /// </summary>
    public void CancelPathPoint(HexCell cell) {
        //AStarFindPath(cell, cell, HideAdj);
    }
    /// <summary>
    /// 计算传入位置在当前选择路径中的G+H消耗,并返回
    /// </summary>
    public float SumCost(HexCell cur) {
        float pathcost = cur.passCost;
        //TODO:取消第一个from的值
        //沿当前路径走过时的总消耗,充当G值
        foreach(var cell in loadingPath) {
            pathcost += cell.Value.passCost;
        }
        //从该点到最新目的地的曼哈顿距离,充当H值
        float eva = Vector2Int.Distance(cur.MapPos, DestPoints[DestPoints.Count-1].MapPos);

        return pathcost;
      }

    /// <summary>
    /// 寻路
    /// </summary>
    public void AStarFindPath(HexCell from,HexCell dest,Func<HexCell,HexCell>Judge) {
        //将路径起点加入loadingPath
        loadingPath.Add(from.GetHashCode(),from);
        Debug.Log(from.GetHashCode() + "-=-" + from.MapPos);
        HexCell nextCell = Judge(from);

        if (!closed.ContainsKey(from.GetHashCode())) {
            closed.Add(from.GetHashCode(), from);
        }
        //如果未找到则递归调用寻找
        if(nextCell.GetHashCode() != dest.GetHashCode()) {
            AStarFindPath(nextCell, dest, FindNextCell);
         }
        //找到时,返回上层递归
        return;
    }

    public HexCell FindNextCell(HexCell cur) {
        //获取入参cell的邻接cells(不包括水行区域及loadingPath,两者已被放入closed并在传入之前剔除)
        List<HexCell> adj = AdjacentHex(cur);

        float minCost = GameStaticData.infinite;
        int minIndex = 0;
        //先算出当前邻接列表中的消耗最少的前进节点,并将所有节点加入open的待机节点
        for(int i=0;i<adj.Count ;i++) {
            adj[i].Img.color = new Color(0, 1, 0);
            float cost = SumCost(adj[i]);
            //检查在open列表中是否存在该节点,不存在则添加.
            if (!open.ContainsKey(adj[i]))
            {
                open.Add(adj[i], cost);
            }
            //如果是更少的消耗节点,则将最少消耗更新为该节点
            if (minCost > cost) {
                minCost = cost;
                minIndex = i;
            }
        }
        //TODO:如果open列表中有很靠前的前置节点比这个更优要怎么办.
        //如果open中的该节点的消耗比当前的值要小,说明前面有节点通过更少的消耗抵达当前点,要更新路径
        if (open[adj[minIndex]]<minCost) {
            //去除路径中的当前节点,并更新open列表中的值
            loadingPath.Remove(cur.GetHashCode());
            open[cur] = SumCost(cur);
            //将更小的前继节点作为路径值,并将其放入closed列表中  的  工作是在下一个递归做的
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
            if (closed.ContainsKey(adj[i].GetHashCode()))
            {
                adj.Remove(adj[i]);
            }
            else//如果本身不在,closed,再接着判别
            {

                //如果属于不能在当前载具情况下行动的边境海,就将其剔除,并加入closed列表
                if (adj[i].type == GameStaticData.FieldType.EdgeSea || adj[i].type == GameStaticData.FieldType.Lake)
                {
                    closed.Add(adj[i].GetHashCode(), adj[i]);
                    adj.Remove(adj[i]);
                }
            }
        }
        return adj;
    }

}
