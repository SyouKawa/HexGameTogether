using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public static PathManager Instance { get; private set; }

    private void Awake(){
        Instance = this;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            Debug.Log("Target Position: " + hit.collider.gameObject.transform.position);
        }
    }
}
