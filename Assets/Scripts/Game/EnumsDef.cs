    /// <summary>
    /// 地形分类
    /// </summary>
    public enum FieldType {
        Mountain, //山地
        Lake, //湖
        Forest, // 森林
        EdgeSea, //边境海(将地形环绕一周,非行动区)
        Plain // 平原
     }

    public enum CheckState {
        InMap,//在大地图
        InBattle,//在战斗中
        InFind,//在寻路
        InMoving,//在行进
    }