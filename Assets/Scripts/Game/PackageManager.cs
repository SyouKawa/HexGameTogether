using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public partial class HUD {
    public RectTransform BagTrans;
    public void InitBag() {
        BagTrans = FindRecTrans("Bag");
        posBegin = BagTrans.anchoredPosition;
        posEnd = BagTrans.anchoredPosition + new Vector2(0, 200);

        FindRecTrans("Bag.Button").GetComponent<Button>().onClick.AddListener(() => {
            isExpand = !isExpand;
            if (isExpand) {
                Startcoroutine(UpBagHUD(posBegin, posEnd, new Vector2(0, 50)));
            } else {
                Startcoroutine(UpBagHUD(posEnd, posBegin, new Vector2(0, -50)));
            }
        });
    }
    public bool isExpand = false;

    private Vector2 posBegin;
    private Vector2 posEnd;

    private float BagUpDownSpeed = 5f;

    public IEnumerator UpBagHUD(Vector2 begin, Vector2 end, Vector2 over) {
        float step = 0;
        while (step < 1f) {
            BagTrans.anchoredPosition = Vector2.Lerp(begin, end + over, step);
            step += Time.deltaTime * BagUpDownSpeed;
            yield return null;
        }
        step = 0;
        while (step < 1f) {
            BagTrans.anchoredPosition = Vector2.Lerp(end + over, end, step);
            step += Time.deltaTime * BagUpDownSpeed * 5;
            yield return null;
        }
    }
}

public class PackageManager : Manager<PackageManager> {
    public override void Start(EventHelper helper) {
        helper.AfterWorldLoadEvent += () => {
            //BagTrans =
        };
    }

}

//背包里的单个Item
public class PackageItem {
    //序号,对应一个物品
    public Item item;
    //堆叠
    public int count;

    //在这个物品上进行合并接收操作
    public void Merge(PackageItem other) {

    }

    //对应的Icon 使用时将其绑定到对应的Transform上
    [PrefabPath("")]
    private class ItemIcon : PrefabBinding {
        public ItemIcon() {

        }

    }
}

public enum ItemType {

}

public class Item {
    //物品
    public int id = 0;
    //物品类型
    public ItemType type;
    //最大堆叠
    public int maxCount;
    //在这个物品上进行使用
    public void OnUse() {

    }

}