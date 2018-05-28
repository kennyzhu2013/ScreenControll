﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragPanelsEx : MonoBehaviour, IPointerDownHandler, IDragHandler {  
	public ResizableWindow resizeWindow;
	// 鼠标起点...
	private Vector2 originalLocalPointerPosition;

	// 面板起点...  
	//private Vector3 originalLocalPanelPosition;  
	// 当前面板...
	private RectTransform panelRectTransform;  
	// 父节点,这个最好是UI父节点，因为它的矩形大小刚好是屏幕大小...
	private RectTransform parentRectTransform;  

	//SetSiblingIndex即可，值越大越靠前...
	private static int siblingIndex = 0;  
	void Awake () {  
		//panelRectTransform = transform.parent as RectTransform;//transform.parent as RectTransform;  da
		//parentRectTransform = panelRectTransform.parent as RectTransform;  //grandfather node...
	}  

	// 鼠标按下  
	public void OnPointerDown (PointerEventData data) {  
		resizeWindow.SetBx (true);

		//siblingIndex++;  
		//panelRectTransform.transform.SetSiblingIndex(siblingIndex);

		// 记录当前面板起点...  
		//originalLocalPanelPosition = panelRectTransform.localPosition;

		// 通过屏幕中的鼠标点，获取在父节点中的鼠标点  
		// parentRectTransform:父节点  
		// data.position:当前鼠标位置  
		// data.pressEventCamera:当前事件的摄像机  
		// originalLocalPointerPosition:获取当前鼠标起点  
		//originalLocalPointerPosition = data.position;
		//RectTransformUtility.ScreenPointToLocalPointInRectangle (parentRectTransform, data.position, data.pressEventCamera, 
			//out originalLocalPointerPosition);  
	}  

	// 拖动  
	public void OnDrag (PointerEventData data) {  
		if (panelRectTransform == null || parentRectTransform == null)  
			return;  

		//Vector2 offsetToOriginal = data.position - originalLocalPointerPosition;
		//resizeWindow.DragWindow ( offsetToOriginal );
		/*
		Vector2 localPointerPosition;  
		// 获取本地鼠标位置  
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle (parentRectTransform, data.position, data.pressEventCamera, 
			out localPointerPosition)) {  
			// 移动位置 = 本地鼠标当前位置 - 本地鼠标起点位置  
			Vector2 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;

			resizeWindow.DragWindow ( offsetToOriginal );
			// 当前面板位置 = 面板起点 + 移动位置  
			//panelRectTransform.localPosition = originalLocalPanelPosition + offsetToOriginal;  
		}  */
		//ClampToWindow ();  
	}  

	// 限制当前面板在父节点中的区域位置  
	/*
	void ClampToWindow () {  
		
	}*/
}  
