using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class CameraController : Singleton<CameraController>{
    public Camera camera { get => Camera.main; }

    public override void Start(EventHelper helper) {
        Debug.Log("Binding Success");
        helper.OnUpdateEvent += Update;
        helper.OnWorldLoadEvent += Init;
    }

    public float minSize = 20;
    public float maxSize = 100;

    private void Init() {
        camera.orthographicSize = GameStaticData.DefaultCameraSize;
    }

    private void Update() {
        if(Input.GetAxis("Mouse ScrollWheel") < -0.01f) {
            camera.orthographicSize -= 1f * GameStaticData.CameraRollSpeed;
            if(camera.orthographicSize < minSize) {
                camera.orthographicSize = minSize;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0.01f) {
            camera.orthographicSize += 1f * GameStaticData.CameraRollSpeed;
            if (camera.orthographicSize > maxSize) {
                camera.orthographicSize = maxSize;
            }
        }
    }

    public void SetPosition(Transform trans) {
        camera.transform.position = new Vector3(trans.position.x, trans.position.y, -100f);
    }

}

