using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DiceControl : MonoBehaviour
{
    public DiceSides thisSide = DiceSides.蓝观察;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = BattleMode.Instance.LoadSprite(thisSide);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
