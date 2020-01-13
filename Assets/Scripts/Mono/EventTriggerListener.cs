using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//这段代码是我抄的. 感觉这个写法真挺好

public class EventTriggerListener : UnityEngine.EventSystems.EventTrigger{
	public delegate void VoidDelegate ();
	public VoidDelegate onClick;
	public VoidDelegate onDown;
	public VoidDelegate onEnter;
	public VoidDelegate onExit;
	public VoidDelegate onUp;
	public VoidDelegate onSelect;
	public VoidDelegate onUpdateSelect;
 
	static public EventTriggerListener Get (GameObject go)
	{
		EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
		if (listener == null) listener = go.AddComponent<EventTriggerListener>();
		return listener;
	}
	public override void OnPointerClick(PointerEventData eventData)
	{
		if(onClick != null) {
            onClick();
        }
            //onClick.Invoke(gameObject);
	}
	public override void OnPointerDown (PointerEventData eventData){
		if(onDown != null) onDown();
	}
	public override void OnPointerEnter (PointerEventData eventData){
		if(onEnter != null) onEnter();
	}
	public override void OnPointerExit (PointerEventData eventData){
		if(onExit != null) onExit();
	}
	public override void OnPointerUp (PointerEventData eventData){
		if(onUp != null) onUp();
	}
	public override void OnSelect (BaseEventData eventData){
		if(onSelect != null) onSelect();
	}
	public override void OnUpdateSelected (BaseEventData eventData){
		if(onUpdateSelect != null) onUpdateSelect();
	}
}