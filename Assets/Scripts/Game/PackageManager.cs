using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PackageManager : Manager<PackageManager> {

    public BagC Bag;

    public override void Start(EventHelper helper) {
        helper.AfterWorldLoadEvent += () => {
            Bag = new BagC(MapManager.Instance.infoHUD);
        };
    }

    public class BagC {
        //30格子都保存在这里 
        public List<BagItemIcon> items = new List<BagItemIcon>();

        public BagC(HUD main) {
            for (int i = 1; i <= 10; i++) {
                GameObject obj = main.Find("Bag.PanelMain.Icon" + i.ToString());
                items.Add(new BagItemIcon(obj, i, ItemType.空白));
            }
            for (int i = 1; i <= 10; i++) {
                GameObject obj = main.Find("Bag.PanelBag1.Icon" + i.ToString());
                items.Add(new BagItemIcon(obj, i + 10, ItemType.禁用));
            }
            for (int i = 1; i <= 10; i++) {
                GameObject obj = main.Find("Bag.PanelBag2.Icon" + i.ToString());
                items.Add(new BagItemIcon(obj, i + 20, ItemType.禁用));
            }
            //有多少格子是可用的
            AvailableCount = 10;

            AddItem(ItemType.木材, 20);
            AddItem(ItemType.燃油, 30);
            AddItem(ItemType.食物, 20);
            AddItem(ItemType.金币, 100);
        }
        public int AvailableCount = 0;

        public bool AddItem(ItemType type, int count) {
            //合并到同类格子里
            for (int i = 0; i < AvailableCount; i++) {
                if (items[i].ItemType == type) {
                    items[i].Count += count;
                    return true;
                }
            }
            //找不到合并,那就找一个空的
            for (int i = 0; i < AvailableCount; i++) {
                if (items[i].ItemType == ItemType.空白) {
                    items[i].ItemType = type;
                    items[i].Count = count;
                    return true;
                }
            }
            //添加失败
            return false;
        }
    }

}

public partial class HUD {
    public RectTransform BagTrans;

    public bool IsExpand = false;

    public void InitBag() {
        BagTrans = FindRecTrans("Bag");
        //初始化展开折叠按钮
        InitDropBtn();

    }
    #region 展开/折叠按钮
    private void InitDropBtn() {
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
    }

    /// UI弹出效果的补间动画
    public IEnumerator Tween(Vector2 begin, Vector2 end) {
        //播放倍速 5 -> 1/5 -> 0.2s完成动画
        float TweenMoveSpeed = 5f;
        //默认回弹 1/4距离 回弹速度是弹出速度的5倍
        Vector2 over = (end - begin) / 4;
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
    #endregion
}

public enum ItemType {
    空白,
    禁用,
    金币,
    食物,
    燃油,
    木材,
    石材,
    工艺品,
}

public class BagItemIcon {
    public static Dictionary<ItemType, Sprite> item_Image = new Dictionary<ItemType, Sprite>();

    private Image image;
    private Text text;

    private int index;
    public BagItemIcon(GameObject root, int index, ItemType type) {
        this.index = index - 1;
        image = root.transform.GetChild(0).GetComponent<Image>();
        text = root.transform.GetChild(3).GetComponent<Text>();
        EventTriggerListener.Get(root).onClick += (PointerEventData data) => { Log.Debug("Click:{0}", this.index); };

        Count = 0;
        ItemType = type;
    }

    //设置这个格子对应的物品种类
    private ItemType itemType;
    public ItemType ItemType {
        get { return itemType; }
        set {
            itemType = value;
            if (value == ItemType.空白) {
                image.sprite = null;
            } else {
                if (item_Image.ContainsKey(value)) {
                    image.sprite = item_Image[value];
                }
            }
        }
    }

    private int count;
    //叠加数量
    public int Count {
        get {
            return count;
        }
        set {
            count = value;
            text.gameObject.SetActive(count > 1);
            text.text = value.ToString();
        }
    }
}