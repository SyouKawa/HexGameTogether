using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResManager{

    //加载显示的Tile数组
    private List<Object[]> FieldTiles = new List<Object[]>();

    /// <summary>
    /// 加载游戏需要的资源
    /// </summary>
    private void LoadRes()
    {
        //加载显示的Tile数组
        int length = System.Enum.GetValues(typeof(GameStaticData.FieldType)).Length;
        for (int i = 0; i < length; i++)
        {
            FieldTiles.Add(Resources.LoadAll("Sprites/tiles_" + ((GameStaticData.FieldType)i).ToString() + "_colored", typeof(Sprite)));
        }
    }
}
