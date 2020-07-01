using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class CameraController : Manager<CameraController>{
    public Camera camera { get => Camera.main; }

    public override void Start(EventHelper helper) {
        helper.OnUpdateEvent += Update;
        helper.OnWorldLoadEvent += Init;
    }

    public float minSize = 20;
    public float maxSize = 100;

    private void Init() {
        camera.orthographicSize = GameData.DefaultCameraSize;
    }

    public bool BeginDrive = false;
    private Vector2 beginPos;

    private void Update() {
        if(Input.GetAxis("Mouse ScrollWheel") > 0.01f) {
            camera.orthographicSize -= 1f * GameData.CameraRollSpeed;
            if(camera.orthographicSize < minSize) {
                camera.orthographicSize = minSize;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < -0.01f) {
            camera.orthographicSize += 1f * GameData.CameraRollSpeed;
            if (camera.orthographicSize > maxSize) {
                camera.orthographicSize = maxSize;
            }
        }

        if(Input.GetMouseButtonDown(1)){
            BeginDrive = true;
            beginPos = Input.mousePosition;
        }
        if(Input.GetMouseButtonUp(1)){
            BeginDrive = false;
        }
        if(BeginDrive){
            Vector2 pos = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
            Vector3 dis = (pos - beginPos)*(GameData.CameraDriveSpeed);
            //反转就是+
            camera.transform.position -= dis;
            beginPos = Input.mousePosition;
        }


        
    }

    public void SetPosition(Transform trans) {
        camera.transform.position = new Vector3(trans.position.x, trans.position.y, -100f);
    }
}

