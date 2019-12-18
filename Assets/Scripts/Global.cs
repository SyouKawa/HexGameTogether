using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{
    public static Global Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
        ObjectManager.poolRootTransform = transform;
        ObjectManager.InitPool();
    }
}
