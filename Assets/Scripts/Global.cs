using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{
    public static Global Instance { get; private set; }
    public ObjectPoolData objectManager;
    private void Awake()
    {
        Instance = this;
        objectManager = new ObjectPoolData();
        objectManager.poolRootTransform = transform;
        objectManager.InitPool();
    }
}
