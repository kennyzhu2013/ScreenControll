using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using UnityEngine.UI;

public class QrcodeView : BaseView {
	private Text name;
	private Image qrCode;

	// Use this for initialization
	public override void Start () {
		base.Start ();
		base.isTweenWin = true;

		name = GetComponentInChildren<Text> (true);
		qrCode = GetComponentInChildren<Image> (true);
		qrCode.enabled = true;

		//to load sprite..
		TextureLoader.Instance().StartSpriteLoad(SCMainWinController.Instance().ScModel.qrcode, 0, 
			new TextureLoader.DelegateSpriteCallMethod(this.Display) );
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/// <summary>
	/// Display the specified headSprite and index.
	/// </summary>
	/// <param name="headSprite">Head sprite.</param>
	/// <param name="index">Index.</param>
	public void Display(Sprite qrSprite, uint index)
	{
		qrCode.enabled = true;
		name.text = SCMainWinController.Instance ().ScModel.nickname;
		qrCode.sprite = qrSprite;
	}
}
