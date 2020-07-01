using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[PrefabPath("Prefabs/Map/PathPoint")]
public class PathPoint : PrefabBinding
{
    public PathPoint(HexCell cell){
        //激活路径点并调节显示倍率
        Transform.parent = Global.Instance.transform;
        //清空从对象池中提取的，要二次使用的路径点的旧旋转角
        Transform.rotation = new Quaternion(0,0,0,0);
        //生成在对应cell的位置
        Transform.position = cell.Transform.position;
    }
}
