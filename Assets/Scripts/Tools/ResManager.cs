using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResManager : Manager<ResManager>{

    //加载显示的Tile数组
    public List<Object[]> FieldTiles { get; private set; }

    public override void Start(EventHelper helper) {
        helper.OnGameLoadEvent += LoadRes;
    }

    /// <summary>
    /// 加载游戏需要的资源
    /// </summary>
    public void LoadRes()
    {
        //初始化Tile使用的数组
         FieldTiles = new List<Object[]>();

        //加载显示的Tile数组
        int length = System.Enum.GetValues(typeof(FieldType)).Length;
        for (int i = 0; i < length; i++)
        {
            FieldTiles.Add(Resources.LoadAll("Sprites/tiles_" + ((FieldType)i).ToString() + "_colored", typeof(Sprite)));
        }
    }
}
