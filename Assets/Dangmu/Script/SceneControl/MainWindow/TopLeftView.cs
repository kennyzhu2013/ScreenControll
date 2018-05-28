using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZenFulcrum.EmbeddedBrowser;
using UnityEngine.Video;
using UnityEngine.UI;

/// <summary>
/// Top left view.
/// </summary>
public class TopLeftView : Common.BaseView {
	public GameObject VideoRoot;
	public GameObject WebRoot;
	public Image shuiYin;
	public Text TitleText;

	private string curUrl = "";

	// Use this for initialization
	void Start () {
		base.Start ();


	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public string GetActivePath()
	{
		if (WebRoot.activeSelf) {
			Browser browser = GetComponentInChildren<Browser> ();
			Log.debug (this, "GetActivePath======url:" + curUrl);
			return curUrl;
		} else if (VideoRoot.activeSelf) {
			VideoPlayer videoPlay = GetComponentInChildren<VideoPlayer> ();
			Log.debug (this, "GetActivePath======url:" + curUrl);
			return curUrl;
		}

		return null;
	}

	//Todo: add title...
	public void DisplayVideo(string videoPath, string title)
	{
		WebRoot.SetActive (false);
		VideoRoot.SetActive (true);
		curUrl = videoPath;
		VideoController videoControl = VideoRoot.GetComponent<VideoController> ();
		videoControl.StartPlay (videoPath, Prepared);
		TitleText.text = title;
	}

	private VideoPlayer curPlayer;
	public long GetVideoFrameCount() {
		if (curPlayer != null)
			return curPlayer.frame;
		return 0;
	}

	void Prepared( VideoPlayer vPlayer )
	{
		Log.debug (this, "VideoPlayer Prepared Ended");
		vPlayer.frame = 1;
		vPlayer.Pause ();
		curPlayer = vPlayer;
		//vPlayer.Play ();
	}

	public void DisplayWeb(string webUrl, string title)
	{
		WebRoot.SetActive (true);
		VideoRoot.SetActive (false);
		curUrl = webUrl;
		Log.debug (this, "DisplayWeb====webUrl:" + webUrl);
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
