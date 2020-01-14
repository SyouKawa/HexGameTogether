using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public partial class HUD {
    public RectTransform BagTrans;

    public bool IsExpand = false;

    public void InitBag() {
        BagTrans = FindRecTrans("Bag");
        Vector2 posBegin = BagTrans.anchoredPosition;
        Vector2 posEnd = BagTrans.anchoredPosition + new Vector2(0, 200);

        FindRecTrans("Bag.ButtonUp").GetComponent<Button>().onClick.AddListener(() => {
            IsExpand = !IsExpand;
            FindRecTrans("Bag.ButtonUp").gameObject.SetActive(false);
            FindRecTrans("Bag.ButtonDown").gameObject.SetActive(true);
            Startcoroutine(Tween(posBegin, posEnd));
        });

        FindRecTrans("Bag.ButtonDown").GetComponent<Button>().onClick.AddListener(() => {
            IsExpand = !IsExpand;
            FindRecTrans("Bag.ButtonUp").gameObject.SetActive(true);
            FindRecTrans("Bag.ButtonDown").gameObject.SetActive(false);
            Startcoroutine(Tween(posEnd, posBegin));
        });

        GameObject obj1 = FindRecTrans("Bag.PanelMain.Icon1").gameObject;
        EventTriggerListener.Get( obj1).onClick += ()=>{Log.Debug("Click1");};

                GameObject obj2 = FindRecTrans("Bag.PanelMain.Icon2").gameObject;
        EventTriggerListener.Get( obj2).onClick += ()=>{Log.Debug("Click2");};
    }

    /// UI弹出效果的补间动画
    public IEnumerator Tween(Vector2 begin, Vector2 end) {
        //播放倍速 5 -> 1/5 -> 0.2s完成动画
        float TweenMoveSpeed = 5f;
        //默认回弹 1/4距离 回弹速度是弹出速度的5倍
        Vector2 over = (end - begin)/4;
        float step = 0;
        while (step < 1f) {
            BagTrans.anchoredPosition = Vector2.Lerp(begin, end + over, step);
            step += Time.deltaTime * TweenMoveSpeed;
            yield return null;
        }
        step = 0;
        while (step < 1f) {
            BagTrans.anchoredPosition = Vector2.Lerp(end + over, end, step);
            step += Time.deltaTime * TweenMoveSpeed * 5;
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
    private class ItemIcon {
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