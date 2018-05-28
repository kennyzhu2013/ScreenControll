using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using ZenFulcrum.EmbeddedBrowser;

public class WebBulletScreenView : BaseView {
	public string BulletUrl = "";
	public Browser browser;
	public ResizableWindow RWindow;

	// Use this for initialization
	void Start () {
		
		base.Start ();
		if ( "" == BulletUrl )
			BulletUrl = "http://yxbwx.mmarket.com/weizannew/app/index.php?i=3&c=entry&rid=32&small=0&t=1524731905&do=barwall&m=dada_xianchang";

		Debug.Log ("WebBulletScreenView: BulletUrl ======" + BulletUrl);
		browser.LoadURL (BulletUrl, true);
		RWindow.StartCoroutine ( RWindow.SetSecondWindowTopMost() );
		//RWindow.SetSecondWindowTopMost ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
