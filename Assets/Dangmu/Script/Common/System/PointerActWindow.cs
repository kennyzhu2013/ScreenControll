using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerActWindow : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler {
	//public TransparentWindow cameraWindow;
	// Use this for initialization
	void Start () {
		//if (null == cameraWindow)
			//cameraWindow = GameObject.FindObjectOfType<TransparentWindow> ();
	}

	// 鼠标按下  
	public void OnPointerEnter (PointerEventData eventData) {
		TransparentWindow.SetWindowsMouseAble ();
	}


	public void OnPointerExit (PointerEventData eventData) {
		//TransparentWindow.SetWindowsMouseDisable ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
