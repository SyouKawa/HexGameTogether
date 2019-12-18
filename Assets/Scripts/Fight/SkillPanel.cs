using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[AddPool("Prefabs/Fight/ImageHUD")]
public class SkillPanelIcon {
    public GameObject source;
    public Image image;
    public SkillPanelIcon() {
        source = ObjectManager.GetInstantiate(this);
        image = source.GetComponent<Image>();
    }
    
    ~SkillPanelIcon() {
        ObjectManager.ReturnInstantiate<SkillPanelIcon>(source);
    }
}

[AddPool("Prefabs/Fight/SkillHUD")]
public class SkillObj {
    public GameObject source;
    public Text text;
    public Transform abiliTrans;

    public SkillObj() {
        source = ObjectManager.GetInstantiate(this);

        text = ObjectManager.GetInstantiate(this).GetComponent<Text>();

    }

    ~SkillObj() {

    }
}
