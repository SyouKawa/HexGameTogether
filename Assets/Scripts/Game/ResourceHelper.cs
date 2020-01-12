using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//这个类纯数图方便测试用
public class ResourceHelper : MonoBehaviour
{
    public Sprite ShopImg;
    public Sprite PortalImg;
    public Sprite UnionImg;
    public Sprite StationImg;

    public static ResourceHelper Instance;
    void Awake(){
        Instance = this;
    }
}
