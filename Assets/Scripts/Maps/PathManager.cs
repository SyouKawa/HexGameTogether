using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.SceneManagement;

public class PathManager : MonoBehaviour
{
    public static PathManager Instance { get; private set; }

    //存放目的地列表
    public List<HexCell> DestPoints;
    public HexCell Begin;
    public List<HexCell> finalPath;
    public HashSet<HexCell> closed;
    public Dictionary<HexCell, float> open;

    private void Awake(){
        Instance = this;
        DestPoints = new List<HexCell>();
        finalPath = new List<HexCell>();
        closed = new HashSet<HexCell>();
        open = new Dictionary<HexCell, float>();
    }


    public void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null){

            HexCell curCell = ObjectManager.GetClass<HexCell>(hit.collider.transform.parent.gameObject);

            if (Input.GetMouseButtonDown(0)) {
                Begin = curCell;
                Begin.Img.color = new Color(0, 0, 1);
             }
            if (Input.GetMouseButtonDown(1)) {
                AddDestPoint(curCell);
            }
            if (Input.GetMouseButtonDown(2)) {
                UnityEngine.SceneManagement.SceneManager.LoadScene("TestSc");
                GameStaticData.random = new System.Random(1);
            }
        }
    }

    /// <summary>
    /// 将点击选中Tile作为下一个目的地
    /// </summary>
    public void AddDestPoint(HexCell cell) {
        DestPoints.Add(cell);
        AStarFindPath(Begin, cell, FindNextCell);
        ShowPath();
     }
    /// <summary>
    /// 取消选中的目的地
    /// </summary>
    public void CancelPathPoint(HexCell cell) {
        //AStarFindPath(cell, cell, HideAdj);
    }

    public void ShowPath() {
        HexCell cur = DestPoints[DestPoints.Count - 1];
        while(cur != Begin) {
            cur.Img.color = new Color(1,0,0);
            cur = cur.prepathcell;
        }
    }

    /// <summary>
    /// 计算传入位置在当前选择路径中的G+H消耗,并返回
    /// </summary>
    public float SumCost(HexCell cur) {
        // G = 从起点走到当前节点的地形消耗
        float pathcost = 0f;
        if (cur.prepathcell == null) {
            cur.dynamicost = 0f;
        }
        else {
            cur.dynamicost = cur.prepathcell.dynamicost + cur.fieldcost;
        }
        pathcost = cur.dynamicost;

        //从该点到最新目的地的曼哈顿距离,充当该点的H值
        cur.destdis = Vector2Int.Distance(cur.MapPos, DestPoints[DestPoints.Count-1].MapPos);
        //G+H的值
        pathcost += cur.destdis;

        return pathcost;
      }

    /// <summary>
    /// 寻路
    /// </summary>
    public void AStarFindPath(HexCell from,HexCell dest,Func<HexCell,HexCell>FindFunc) {

        float curCost = SumCost(from);

        if (!open.ContainsKey(from)) {
            open.Add(from, curCost);
        }

        HexCell nextCell = FindFunc(from);

        //如果未找到则递归调用寻找
        if(nextCell!= dest) {
            AStarFindPath(nextCell, dest, FindNextCell);
         }
        //找到时,返回上层递归
        return;
    }

    public HexCell FindNextCell(HexCell cur) {
        //获取入参cell的邻接cells(不包括水行区域及loadingPath,两者已被放入closed并在传入之前剔除)
        open.Remove(cur);
        //只要作为过一次路径节点,则会被归入closed,防止纠错路径时再次回到该点
        closed.Add(cur);
        List<HexCell> adj = AdjacentHex(cur);

        //先算出当前邻接列表中的消耗最少的前进节点,并将所有节点加入open的待机节点
        for(int i=0;i<adj.Count ;i++) {

            //检查在open列表中是否存在该节点,不存在则添加.
            if (!open.ContainsKey(adj[i]))
            {
                adj[i].prepathcell = cur;//直接为新临界点添加前继节点
                open.Add(adj[i], SumCost(adj[i]));
            }
            else {//检查以新节点作为临时前继节点,是否消耗更低
                float newcost = adj[i].fieldcost + cur.dynamicost + Vector2Int.Distance(adj[i].MapPos, DestPoints[DestPoints.Count - 1].MapPos);
                if (adj[i].dynamicost > newcost) //沿其他路径到该点的消耗比从此路径要大,则更新open中的路径(该节点的前继节点)
                {
                    //更新前继节点,保证路径的更新
                    adj[i].prepathcell = cur;
                    open[adj[i]] = SumCost(adj[i]);
                }
            }
            adj[i].Img.color = new Color(0, 1, 0);
            adj[i].SetText(SumCost(adj[i]).ToString());
        }

        float minCost = GameStaticData.infinite;
        HexCell next = null;
        //选择路径:选择open中的最小消耗节点,并沿其寻找新的路径
        foreach(KeyValuePair<HexCell,float> pair in open) {
            //选择消耗最小的作为next
            if (pair.Value < minCost) {
                next = pair.Key;
                minCost = next.dynamicost + next.destdis;//G+h
            }
        }

        next.Img.color = new Color(0, 0, 1);
        return next;
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

}
