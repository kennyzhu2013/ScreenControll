using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using UnityEngine.UI;

/// <summary>
/// Control view.
/// </summary>
public class ControlView : BaseView {

	// Use this for initialization
	public override void Start () {
		base.Start ();

		//添加button事件...
		Button[] btnList = gameObject.GetComponentsInChildren<Button>();
		foreach (Button btn in btnList) {
			btn.onClick.AddListener (delegate() {
				this.OnClick( btn.gameObject );
			});
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/// <summary>
	/// Process the click event.
	/// </summary>
	/// <param name="objSender">Object sender.</param>
	public void OnClick(GameObject objSender)
	{
		switch (objSender.name) {
		case "Dangmu":
			//if (true == DangmuTextController.Instance.Displayer.Enable) {
				//DangmuTextController.Instance.Displayer.Enable = false;
				//SetText (objSender, "打 开 弹 幕");
			//} else {
				//DangmuTextController.Instance.Displayer.Enable = true;
				//SetText (objSender, "关 闭 弹 幕");
			//}
			break;
		case "Qipao":
			if (true == VerticalTextController.Instance.Displayer.Enable) {
				VerticalTextController.Instance.Displayer.Enable = false;
				SetText (objSender, "打 开 起 泡");
			} else {
				VerticalTextController.Instance.Displayer.Enable = true;
				SetText (objSender, "关 闭 起 泡");
			}
			break;
		case "Setting":
			break;
		case "MultiScreen":
			break;
		default:
			break;
		}
	}

	private void SetText(GameObject btn, string content)
	{
		Text uiText = btn.GetComponentInChildren<Text> ();
		uiText.text = content;
	}
}
