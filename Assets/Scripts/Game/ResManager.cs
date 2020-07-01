using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ResManager : Manager<ResManager>{

    public Dictionary<FieldType,List<Sprite>> FieldTiles{ get; private set; }
    public override void Start(EventHelper helper) {
        helper.OnGameLoadEvent += LoadRes;
    }

    /// <summary>
    /// 加载游戏需要的资源
    /// </summary>
    public void LoadRes()
    {
        //初始化Tile使用的数组
        FieldTiles = new Dictionary<FieldType, List<Sprite>>();
        //优化后
        foreach(FieldType field in System.Enum.GetValues(typeof(FieldType))){
            string path = Utils.FormatString("Sprites/tiles_{0}_colored",field.ToString());
            FieldTiles.Add(field,Resources.LoadAll<Sprite>(path).ToList());
        }

        BuidingImg = Resources.LoadAll<Sprite>("Sprites/locations_colored").ToList();
    }

    public Sprite GetRandomFieldImg(FieldType type){
        return FieldTiles[type][GameData.random.Next(0, FieldTiles[type].Count)];
    }
    public List<Sprite> BuidingImg;

}
