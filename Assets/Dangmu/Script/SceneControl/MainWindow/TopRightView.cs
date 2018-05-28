using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using UnityEngine.UI;
using ZenFulcrum.EmbeddedBrowser;
using UnityEngine.Video;

/// <summary>
/// Top right view.
/// </summary>
public class TopRightView : BaseView {
	public QrcodeView QrView;
	public Text TitleText;

	[Header("水印")]
	public Image shuiYin;

	[Header("退出全屏按钮")]
	public Button quitFullBtn;
	private bool QrEnabled = false;
	public Camera FullCamera;
	public Camera MainCamera;
	public GameObject VideoRoot;
	public GameObject WebRoot;
	private string curUrl = "";
	private Canvas selfCanvas;

	// Use this for initialization
	void Start () {
		base.Start ();

		QrView = GetComponentInChildren<QrcodeView> (true);
		if (null == FullCamera) {
			FullCamera = GameObject.Find ("FullScreenCamera").GetComponent<Camera> ();
			FullCamera.enabled = false;
		}
		if (null == MainCamera) {
			MainCamera = GameObject.Find ("Main Camera").GetComponent<Camera> ();
		}

		selfCanvas = GetComponent<Canvas> ();
		DisplayNone ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ResetQrCode() {
		QrEnabled = !QrEnabled;
		QrView.gameObject.SetActive ( QrEnabled );
	} 

	public void FullWindowSwitch() {
		//Text textTips = objSender.GetComponentInChildren<Text> ();
		if (FullCamera.enabled) {
			FullCamera.enabled = false;
			//textTips.text = "全屏";
			selfCanvas.worldCamera = MainCamera;
			quitFullBtn.gameObject.SetActive (false);
		}
		else {
			FullCamera.enabled = true;
			//textTips.text = "退出全屏";
			selfCanvas.worldCamera = FullCamera;
			quitFullBtn.gameObject.SetActive (true);
		}
	}

	public string GetActivePath()
	{
		if (WebRoot.activeSelf) {
			Browser browser = GetComponentInChildren<Browser> ();
			Log.debug (this, "GetActivePath======url:" + curUrl);
			//return browser.Url;
			return curUrl;
		} else if (VideoRoot.activeSelf) {
			VideoPlayer videoPlay = GetComponentInChildren<VideoPlayer> ();
			Log.debug (this, "GetActivePath======url:" + curUrl);
			//return videoPlay.url;
			return curUrl;
		}

		return null;
	}

	private long curFrame;
	public void DisplayVideo(string videoPath, string title, long lFrame)
	{
		WebRoot.SetActive (false);
		VideoRoot.SetActive (true);
		curUrl = videoPath;
		VideoController videoControl = VideoRoot.GetComponent<VideoController> ();
		videoControl.StartPlay (videoPath, Prepared);
		TitleText.text = title;
		curFrame = lFrame;
	}
		
	void Prepared( VideoPlayer vPlayer )
	{
		Log.debug (this, "VideoPlayer Prepared Ended");
		vPlayer.frame = curFrame;
		vPlayer.Pause ();
	}

	public void DisplayWeb(string webUrl, string title)
	{
		WebRoot.SetActive (true);
		VideoRoot.SetActive (false);
		curUrl = webUrl;
		SimpleController webControl = WebRoot.GetComponentInChildren<SimpleController> ();
		webControl.GoToUrl (webUrl);
		TitleText.text = title;
	}

	public void DisplayNone()
	{
		curUrl = "";
		WebRoot.SetActive (false);
		VideoRoot.SetActive (false);
	}
}
