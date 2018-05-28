using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using UnityEngine.UI;

/// <summary>
/// SC main view.
/// for canvas-main and canvas-bottom...
/// </summary>
public class SCMainView : BaseView {
	//other function view..
	AudioSource audioSource;
	public RawImage projectImage;
	private Dropdown windowSizeDrop;
	private int iDropValue;

	//for test..
	//private Sprite dangmuGuanbi;
	//private Sprite qrcodeGuanbi;
	//private Sprite voiceGuanbi;
	//private Sprite projectGuanbi;
	[Header("按钮选中时候的背景图片")]
	public Sprite buttonSelected;

	[Header("按钮未选中时候的背景图片")]
	public Sprite buttonUnselected;
	// Use this for initialization
	void Start () {
		base.Start ();
		base.isTweenWin = true;

		//添加button事件...
		Button[] btnList = gameObject.GetComponentsInChildren<Button>();
		foreach (Button btn in btnList) {
			btn.onClick.AddListener (delegate() {
				this.OnClick( btn.gameObject );
			});
		}

		if (null == windowSizeDrop) {
			windowSizeDrop = GameObject.Find ("Dropdown").GetComponent<Dropdown> ();
		}
		iDropValue = windowSizeDrop.value;

		audioSource = GameObject.Find ("Audio Source").GetComponent<AudioSource> ();
		if (null == projectImage) {
			projectImage = GameObject.Find ("ProjectImage").GetComponent<RawImage> ();
		}

		projectImage.enabled = false;

		//buttonUnselected = Resources.Load(@"button_0.png", typeof(Sprite)) as Sprite;
		//buttonSelected = Resources.Load(@"button_1.png", typeof(Sprite)) as Sprite;
		//qrcodeGuanbi = Resources.Load(@"Image/icon/二维码_不可点击_icon.png", typeof(Sprite)) as Sprite;
		//voiceGuanbi = Resources.Load(@"Image/icon/声音_不可点击_icon.png", typeof(Sprite)) as Sprite;
		//projectGuanbi = Resources.Load(@"Image/icon/停止投影_icon.png", typeof(Sprite)) as Sprite;
	}
	
	// Update is called once per frame
	void Update () {
		if (windowSizeDrop.value == iDropValue)
			return;
		
		if (0 == windowSizeDrop.value) {
			ResizableWindow.Instance ().SetWindowSize (1920, 1080);
		}
		else if (1 == windowSizeDrop.value) {
			ResizableWindow.Instance ().SetWindowSize (1366, 768);
		}
		else if (2 == windowSizeDrop.value) {
			//ResizableWindow.Instance ().SetWindowSize (1280, 1024);
			ResizableWindow.Instance ().SetWindowSize (1280, 724);
		}
		windowSizeDrop.value = iDropValue;
	}

	/// <summary>
	/// Process the click event of the main window.
	/// TODO:..
	/// </summary>
	/// <param name="objSender">Object sender.</param>
	public void OnClick(GameObject objSender)
	{
		switch (objSender.name) {
		case "AddMaterialBtn": //Open Material Window..
			SCWindowsManager.Instance.openWin( typeof(FileMaterialDiaLog) );
			break;
		case "DanMuBtn":
			if ( SCMainWinController.Instance ().ScModel.IsLogined ) {
				//&& SCMainWinController.Instance ().DangmuControl.DanmuCurStatus != DangmuTextController.DangmuStatus.DangmuStatus_None ) {
				if ( true == SCMainWinController.Instance().DangmuVw.isActiveAndEnabled ) {
					//SCMainWinController.Instance().DangmuVw.enabled = false;
					SCWindowsManager.Instance.closeWin( typeof(WebBulletScreenView) );
					//SetButtonText (objSender, "打 开 弹 幕");
					SetButtonSprite (objSender, buttonUnselected);
					//SCMainWinController.Instance ().DangmuControl.DanmuCurStatus = DangmuTextController.DangmuStatus.DangmuStatus_Hiding;
				} else {
					//SCMainWinController.Instance().DangmuVw.enabled = true;
					SCMainWinController.Instance().InitDangmu();
					SetButtonSprite (objSender, buttonSelected);
					//SetButtonText (objSender, "关 闭 弹 幕");
					//SCMainWinController.Instance ().DangmuControl.DanmuCurStatus = DangmuTextController.DangmuStatus.DangmuStatus_Opening;
				}
			} 
			else if ( SCMainWinController.Instance ().ScModel.IsLogined ) { //Logined but not intialed..
				if ( true == SCMainWinController.Instance().DangmuVw.isActiveAndEnabled ) {
					SCWindowsManager.Instance.closeWin( typeof(WebBulletScreenView) );
					//SetButtonText (objSender, "打 开 弹 幕");
					SetButtonSprite (objSender, buttonUnselected);
					//SCMainWinController.Instance ().DangmuControl.DanmuCurStatus = DangmuTextController.DangmuStatus.DangmuStatus_Hiding;
				} else {
					//SCMainWinController.Instance().DangmuVw.enabled = true;
					SetButtonSprite (objSender, buttonSelected);
					SCMainWinController.Instance().InitDangmu();
					//SetButtonText (objSender, "关 闭 弹 幕");
				}
			}
			else {
				//SCMainWinController.Instance ().DangmuControl.DanmuCurStatus = DangmuTextController.DangmuStatus.DangmuStatus_Initing;
				SCWindowsManager.Instance.openWin( typeof(SCLoginView) );
			}
			break;
		case "SwitchBtn"://qiehuan
			SwitchProject();
			break;
		case "QRCodeBtn":
			SCMainWinController.Instance().TopRhtVw.ResetQrCode ();
			break;
		case "FullScreenBtn":
			//SCWindowsManager.Instance.openWin (typeof(FullWindowView));
			SCMainWinController.Instance().TopRhtVw.FullWindowSwitch( );
			break;
		case "StartBtn": //Preject...
			//
			ProjectProcess( objSender );
			break;
		case "SoundBtn":
			audioSource.mute = !audioSource.mute;
			if ( false == audioSource.mute )
				SetButtonSprite (objSender, buttonSelected);
			else
				SetButtonSprite (objSender, buttonUnselected);
			break;

		case "BindBtn":
			ProcessBingding (objSender);
			break;

		case "MarkBtn":
			if ( !SCMainWinController.Instance ().ScModel.IsLogined ) {
				SCWindowsManager.Instance.openWin ( typeof(SCLoginView) );
			}
			break;

		default:
			break;
		}
	}

	/// <summary>
	/// Process the project.
	/// </summary>
	/// <param name="btnObj">Button object.</param>
	public void ProjectProcess(GameObject btnObj) {
		Text textTips = btnObj.GetComponentInChildren<Text> ();
		if (projectImage.enabled) {
			projectImage.enabled = false;
			textTips.text = "开始投影";
			SetButtonSprite (btnObj, buttonUnselected);
		}
		else {
			projectImage.enabled = true;
			textTips.text = "停止投影";
			SetButtonSprite (btnObj, buttonSelected);
		}
	}

	public void SwitchProject () {
		string leftString = SCMainWinController.Instance ().TopLeftVw.GetActivePath ();
		string leftTitle = SCMainWinController.Instance ().TopLeftVw.TitleText.text;
		string rightString = SCMainWinController.Instance ().TopRhtVw.GetActivePath ();
		string rightTitle = SCMainWinController.Instance ().TopRhtVw.TitleText.text;

		bool rightBVideo = SCMainWinController.Instance ().TopRhtVw.VideoRoot.activeSelf;
		bool rightBWeb = SCMainWinController.Instance ().TopRhtVw.WebRoot.activeSelf;
		if ( leftString == null )
			SCMainWinController.Instance ().TopRhtVw.DisplayNone ();
		else if ( SCMainWinController.Instance ().TopLeftVw.WebRoot.activeSelf )
			SCMainWinController.Instance ().TopRhtVw.DisplayWeb(leftString, leftTitle);
		else if ( SCMainWinController.Instance ().TopLeftVw.VideoRoot.activeSelf )
			SCMainWinController.Instance ().TopRhtVw.DisplayVideo(leftString, leftTitle, SCMainWinController.Instance ().TopLeftVw.GetVideoFrameCount());

		if ( rightString == null )
			SCMainWinController.Instance ().TopLeftVw.DisplayNone ();
		else if ( rightBWeb )
			SCMainWinController.Instance ().TopLeftVw.DisplayWeb(rightString, rightTitle);
		else if ( rightBVideo )
			SCMainWinController.Instance ().TopLeftVw.DisplayVideo(rightString, rightTitle);
	}

	/*
	private void SetButtonText(GameObject btn, string content)
	{
		Text uiText = btn.GetComponentInChildren<Text> ();
		uiText.text = content;
	}*/

	private void ProcessBingding(GameObject btn)
	{
		//SCWindowsManager.Instance.openWin( typeof(SCLoginView) );
		if ( SCMainWinController.Instance ().ScModel.IsLogined ) {
			SCMainWinController.Instance ().LoginOut ();
			//SetButtonSprite (btn, buttonUnselected);
		}
		else {
			if (true == SCWindowsManager.Instance.ContainerWin (typeof(SCLoginView))) {
				SCWindowsManager.Instance.closeWin (typeof(SCLoginView));
			} else {
				SCWindowsManager.Instance.openWin (typeof(SCLoginView));
			}
		}
	}

	private void SetButtonSprite(GameObject btn, Sprite uiSprite)
	{
		Image uiImage = btn.GetComponentInChildren<Image> ();
		uiImage.sprite = uiSprite;
	}
}
