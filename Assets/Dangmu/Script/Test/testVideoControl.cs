using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testVideoControl : MonoBehaviour {
	public VideoController control;

	// Use this for initialization
	void Start () {
		if (null == control)
			control = GetComponentInChildren<VideoController> ();
		control.StartPlay ("file:///C:/Unity3d/dangmu/dangmu/Assets/Dangmu/Textures/WeChat_20180305193437.mp4", null);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
