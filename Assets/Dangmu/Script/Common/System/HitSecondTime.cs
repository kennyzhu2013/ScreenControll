using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class HitSecondTime : MonoBehaviour, IPointerDownHandler/*IPointerEnterHandler*/  {
	public Action DoubleClick;

	// 鼠标按下 ...
	public void OnPointerClick (PointerEventData data) {
		Debug.LogWarning ("HitSecondTime:" + data);
		if (data.clickCount == 2) {
			if (DoubleClick != null) {
				DoubleClick.Invoke ();
			}
		}
	}

	// 鼠标按下  
	public void OnPointerDown (PointerEventData data) { 
	}

	/*
	public void OnPointerEnter(PointerEventData eventData) {
		Debug.Log ("OnPointerEnter=============");
	}*/

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
