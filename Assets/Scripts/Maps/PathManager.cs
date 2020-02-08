using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 寻路结果类
/// </summary>
public class FpResult {
    public bool IsFinded { get; set; }
    public List<HexCell> Path { get; set; }
    public int Sumcost { get; set; }

    public FpResult() { }

    public FpResult(bool _isfinded, List<HexCell> _path, int _sumCost) {
        Sumcost = _sumCost;
        Path = _path;
        IsFinded = _isfinded;
    }

    public static FpResult Fail = new FpResult() {
        IsFinded = false,
        Path = null,
        Sumcost = 0
    };
}

public class PathManager : Manager<PathManager> {

    public class FpData {
        public float fromcost; //G值:从起点到该节点的消耗(pre+filed之和)
        public float destdis; //H值:估计值,用该点到终点的曼哈顿距离充当
        public float sumCost { get => fromcost + destdis; }

        public HexCell prepathcell; //前继节点
        public FpData() {
            fromcost = 0f;
            destdis = 0f;

            prepathcell = null;
        }
    }

    public HashSet<HexCell> closed;
    public HashSet<HexCell> open;
    private Dictionary<HexCell, FpData> cellsdata;
    private List<PathPoint> pointLine;

    public PathManager() {
        closed = new HashSet<HexCell>();
        open = new HashSet<HexCell>();
        cellsdata = new Dictionary<HexCell, FpData>();
    }

    public override void Start(EventHelper helper) { }

    /// <summary>
    /// 显示路径
    /// </summary>
    public FpResult GetPath(HexCell from, HexCell dest) {

        //按照对应的权重方式寻路
        AStarFindPath(from, dest);

        //TODO:取消颜色显示,使用路线sprite或shader显示
        HexCell cur = dest;
        int sumCost = 0;

        List<HexCell> finalpath = new List<HexCell>();
        while (cur != from) {
            cur.Find("DebugImg").GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.3f);
            finalpath.Add(cur);
            sumCost += cur.FieldCost;
            if (sumCost > PlayerManager.Instance.Player.supply) {
                return FpResult.Fail;
            }
            cur = cellsdata[cur].prepathcell;
        }
        finalpath.Add(from);
        finalpath.Reverse();
        SpawnPath(finalpath);
        FpResult result = new FpResult(true, finalpath, sumCost);

        return result;
    }

    //根据寻路结果创建一条用于显示的路径
    public void SpawnPath(List<HexCell> path){
        pointLine = new List<PathPoint>();
        for(int i=0;i<path.Count - 1;i++){
            //目标向量
            Vector2 dir = (Vector2)(path[i+1].Transform.position - path[i].Transform.position).normalized;
            //计算与目标向量之间的夹角
            float angle = Vector2.SignedAngle(Vector2.right,dir);
            //Debug.Log("cur angle:"+angle);
            //创建路径点并旋转方向
            PathPoint point = new PathPoint(path[i]);
            point.Transform.RotateAround(path[i].Transform.position,Vector3.forward,angle);
            //加入路径列表
            pointLine.Add(point);
        }
    }

    /// <summary>
    /// 获取以传入cell为中心的相邻可行进cells
    /// </summary>
    public List<HexCell> FindAdjacentHex(HexCell centercell) {
        int x = centercell.MapPos.x;
        int y = centercell.MapPos.y;
        MapManager map = MapManager.Instance;
        //HexCell[, ] cells = MapManager.Instance.Map.Cells;
        List<HexCell> adj = new List<HexCell> {
            map.GetCell(x+1,y+1),
            map.GetCell(x - 1, y - 1),
            map.GetCell(x, y + 1),
            map.GetCell(x, y - 1),
            map.GetCell(x + 1, y),
            map.GetCell(x - 1, y),
        };
        for (int i = adj.Count - 1; i >= 0; i--) {
            //如果本身就属于closed,则在邻接表中删除该节点
            //(已经在当前路径的节点,也在closed中,所以不在下方再加入是否属于loadingPath的判断)
            if (closed.Contains(adj[i])) {
                adj.Remove(adj[i]);
            } else //如果本身不在,closed,再接着判别
            {
                //如果属于不能在当前载具情况下行动的边境海,就将其剔除,并加入closed列表
                if (adj[i].FieldType == FieldType.EdgeSea || adj[i].FieldType == FieldType.Lake) {
                    closed.Add(adj[i]);
                    adj.Remove(adj[i]);
                }
            }
        }
        return adj;
    }

    //为什么是Balance...

    /// <summary>
    /// 计算传入位置在当前选择路径中的G+H(G和H权重相等)消耗,并返回
    /// </summary>
    public float BalanceSumCost(HexCell cur, HexCell dest) {
        // G = 从起点走到当前节点的地形消耗
        float pathcost = 0f;
        if (cellsdata[cur].prepathcell == null) {
            cellsdata[cur].fromcost = 0f;
        } else {
            cellsdata[cur].fromcost = cellsdata[cellsdata[cur].prepathcell].fromcost + cur.FieldCost;
        }
        //H = 从该点到最新目的地的曼哈顿距离
        cellsdata[cur].destdis = Vector2Int.Distance(cur.MapPos, dest.MapPos);

        //G+H的值
        pathcost = cellsdata[cur].fromcost + cellsdata[cur].destdis;
        return pathcost;
    }

    /// <summary>
    /// A*寻路接口
    /// </summary>
    public void AStarFindPath(HexCell from, HexCell dest) {
        //开始处理当前节点
        if (!open.Contains(from)) {
            open.Add(from);
            cellsdata.Add(from, new FpData());
        }
        //寻找下一个节点
        HexCell nextCell = FindNextCell(from, dest);

        //如果未找到则递归调用寻找
        if (nextCell != dest) {
            AStarFindPath(nextCell, dest);
        }
        //找到时,返回到上层递归
        return;
    }

    /// <summary>
    /// 根据新传入当前节点及其邻接节点列表,更新openList
    /// </summary>
    public void UpdateOpenList(HexCell cur, HexCell dest, List<HexCell> adj) {
        //先算出当前邻接列表中的消耗最少的前进节点,并将所有节点加入open的待机节点
        for (int i = 0; i < adj.Count; i++) {
            //检查在open列表中是否存在该节点,不存在则添加.(因为之前是直接存在HexCell里,但现在是新建FpData,所以要对应)
            if (!open.Contains(adj[i])) {
                open.Add(adj[i]);
                cellsdata.Add(adj[i], new FpData());
                cellsdata[adj[i]].prepathcell = cur; //直接为新临界点添加前继节点
            } else { //检查以新节点作为临时前继节点,是否消耗更低
                float newcost = adj[i].FieldCost + cellsdata[cur].fromcost + Vector2Int.Distance(adj[i].MapPos, dest.MapPos);
                if (cellsdata[adj[i]].fromcost > newcost) //沿其他路径到该点的消耗比从此路径要大,则更新open中的路径(该节点的前继节点)
                {
                    //更新前继节点,保证路径的更新
                    cellsdata[adj[i]].prepathcell = cur;
                    BalanceSumCost(adj[i], dest);
                }
            }
            //TODO:将辅助染色显示变更为上浮显示路径
            
            adj[i].Find("DebugImg").GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 0.3f);
            
            //adj[i].SetDebugInfo(BalanceSumCost(adj[i], dest).ToString());
            adj[i].DebugTextMesh.text = adj[i].MapPos.ToString()+ "\n"+ BalanceSumCost(adj[i], dest).ToString("F1");
        }
    }

    /// <summary>G
    /// 由当前节点选择出最优的下一个节点
    /// </summary>
    public HexCell FindNextCell(HexCell cur, HexCell dest) {
        //获取入参cell的邻接cells(不包括水行区域及loadingPath,两者已被放入closed并在传入之前剔除)
        open.Remove(cur);
        //cellsdata.Remove(cur);这里不能移除celldata的内容,因为要获取前驱节点

        //只要作为过一次路径节点,则会被归入closed,防止纠错路径时再次回到该点
        closed.Add(cur);

        //获取邻接节点,并更新open列表
        List<HexCell> adj = FindAdjacentHex(cur);
        UpdateOpenList(cur, dest, adj);

        //选择路径:选择open中的最小消耗节点,并沿其寻找新的路径
        float minCost = GameData.infinite;
        HexCell next = null;
        foreach (HexCell cell in open) {
            //选择消耗最小的作为next
            if (cellsdata[cell].sumCost < minCost) {
                next = cell;
                minCost = cellsdata[cell].sumCost;
            }
        }
        next.Find("DebugImg").GetComponent<SpriteRenderer>().color = new Color(0, 0, 1, 0.3f);
        return next;
    }

    /// <summary>
    /// 重置每次寻路使用的存储数据
    /// </summary>
    public void FreeFindPathData() {
        //清空Debug信息
        foreach (HexCell cell in closed) { ResetCell(cell); }
        foreach (HexCell cell in open) { ResetCell(cell); }
        //清空寻路数据
        closed = new HashSet<HexCell>();
        open = new HashSet<HexCell>();
        cellsdata = new Dictionary<HexCell, FpData>();
        //销毁路径点并归还对象池
        if(pointLine != null){
            PrefabBinding.DeleteList(pointLine);
        }

        void ResetCell(HexCell cell){
            cell.DebugBGRenderer.color = new Color(0, 0, 0, 0);
            cell.DebugTextMesh.text = cell.MapPos.ToString();
        }
    }

}