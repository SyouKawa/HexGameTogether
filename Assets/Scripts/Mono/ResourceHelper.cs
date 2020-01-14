using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

//这个类纯数图方便测试用,不算做框架的一部分,如果要用最好另外挂一个这样
public class ResourceHelper : MonoBehaviour {
    public Sprite ShopImg;
    public Sprite PortalImg;
    public Sprite UnionImg;
    public Sprite StationImg;

    public List<Sprite> ItemImgs;

    public static ResourceHelper Instance;
    void Awake() {
        Instance = this;

        int count = 0;
        foreach (ItemType t in System.Enum.GetValues(typeof(ItemType))) {
            if(t == ItemType.空白)
                continue;
            BagItemIcon.item_Image.Add(t, ItemImgs[count]);
            count++;
        }
    }
}