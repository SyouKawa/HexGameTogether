using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceControl : MonoBehaviour
{
    public Sprite sprite;

    // Start is called before the first frame update
    void Start()
    {
        sprite = Resources.Load<Sprite>("hexGameIcon/0");

        GetComponent<SpriteRenderer>().sprite = sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
