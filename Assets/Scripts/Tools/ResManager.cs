﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResManager : Singleton<ResManager>{

    //加载显示的Tile数组
    public List<Object[]> FieldTiles { get; private set; }

    /// <summary>
    /// 加载游戏需要的资源
    /// </summary>
    public void LoadRes()
    {
        //初始化Tile使用的数组
         FieldTiles = new List<Object[]>();

        //加载显示的Tile数组
        int length = System.Enum.GetValues(typeof(GameStaticData.FieldType)).Length;
        for (int i = 0; i < length; i++)
        {
            FieldTiles.Add(Resources.LoadAll("Sprites/tiles_" + ((GameStaticData.FieldType)i).ToString() + "_colored", typeof(Sprite)));
        }
    }
}