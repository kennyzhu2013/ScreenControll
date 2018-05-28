using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class SCLoginView : Common.BaseView {
	public SCMainWinModel _model;
	public InputField passwdField;
	public InputField activeField;
	public Text serverTips;
	public Button btnLogin;
	public Button closeBtn;

	[Header("绑定按钮")]
	public Button bindBtn;

	[Header("绑定后的活动状态")]
	public Text bingText;
	// Use this for initialization
	public override void Start () {
		base.Start ();
		_model = SCMainWinController.Instance ().ScModel;

		//add listener..
		//btnLogin = gameObject.GetComponentInChildren<Button>();
		btnLogin.onClick.AddListener (delegate() {
			this.Login();
		});

		closeBtn.onClick.AddListener (delegate() {
			this.Hide();
		});
		isTweenWin = true;
	}

	// Update is called once per frame
	void Update () {

	}

	/// <summary>
	/// Login this instance.
	/// Proecess Login Request...
	/// </summary>
	public void Login()
	{
		LockUI ();

		//send LoginRequest to server..
		//login process.
		DangmuLoginReq req = new DangmuLoginReq ();
		req.rid = System.Convert.ToInt32( activeField.text );
		req.passwd = passwdField.text;
		_model.Passwd = req.passwd;

		_model.CurRid = req.rid;
		StartCoroutine( NetWorkManager.Instance.socket.GetHttpRequest( req.GetUrlString (), LoginResponse ) );

		//wait for response..
		/*
		yield return new WaitForSeconds(1.0f);
		while ( NetWorkManager.Instance.socket.MessageRepsonseEvent == null ) { //wait for all the time...
			Debug.Log ("[SocketIO] InitDangmu null, no response,wait 1 second! ");
			yield return new WaitForSeconds(1);
		}*/
	}

	/// <summary>
	/// Exit this instance.
	/// </summary>
	public void Exit()
	{
		Application.Quit ();
	}

	/// <summary>
	/// Locks the UI.
	/// </summary>
	public void LockUI()
	{
		btnLogin.enabled = false;
	}

	public void UnLockUI()
	{
		btnLogin.enabled = true;
	}

	//登录响应...
	public void LoginResponse()
	{
		if ( null == NetWorkManager.Instance.socket.MessageRepsonseEvent ) { //wait for all the time...
			LoginFailed("服务器没有响应!");
			return;
		}

		//response get...
		//for to unserizlized
		LitJson.JsonData _jsonObect = JsonMapper.ToObject( NetWorkManager.Instance.socket.MessageRepsonseEvent.Data );
		if (null == _jsonObect) {
			Log.error (this, "InitDangmu#################null == _jsonObect!");
			LoginFailed("服务器返回消息为空!");
			return;
		}

		/*
		Packet packet = decoder.Decode(e);*/
		//目前无法区分消息类型....
		DangmuLoginRsp loginRsp = new DangmuLoginRsp();
		if ( false == loginRsp.ParseJsonObject(_jsonObect) ){
			Log.error (this, "InitDangmu#################ParseJsonObject failed!");
			LoginFailed ("服务器返回登录失败!");
			return;
		}

		LoginSuccess (loginRsp);
	}

	//for test....
	public void Test()
	{
		LoginSuccess (null);
	}

	/// <summary>
	/// Login this instance.
	/// 登录失败处理...
	/// </summary>
	public void LoginFailed(string strFail)
	{
		serverTips.text = strFail;
		_model.IsLogined = false;
		UnLockUI ();
	}

	/// <summary>
	/// Login this instance.
	/// 登录成功处理...
	/// TODO:Realize...
	/// </summary>
	public void LoginSuccess(DangmuLoginRsp loginRsp)
	{
		_model.WexinId = loginRsp.weixinId;
		_model.nickname = loginRsp.nickname;
		_model.qrcode = loginRsp.qrcodeUrl;
		_model.IsMark = loginRsp.bMark;
		_model.actName = loginRsp.actName;

		//_model.IsLogined = true;
		//Start Init dangmu messages...
		//whether to init judge by DangmuStatus_Initing..
		/*
		if (DangmuTextController.DangmuStatus.DangmuStatus_Initing == SCMainWinController.Instance ().DangmuControl.DanmuCurStatus
			|| DangmuTextController.DangmuStatus.DangmuStatus_None == SCMainWinController.Instance ().DangmuControl.DanmuCurStatus )
			SCMainWinController.Instance ().InitDangmu ();
		else {
			SCWindowsManager.Instance.OpenAndCloseWin( typeof(CombBulletScreenDisplayer), typeof(SCLoginView) );
			SCMainWinController.Instance().DangmuControl.DanmuCurStatus = DangmuTextController.DangmuStatus.DangmuStatus_Opening;
		}*/

		//for webview to displayweb
		SCMainWinController.Instance ().InitDangmu ();

		//
		_model.LoginTime = Utility.GetUnixTimeStamp().ToString();

		//set Bind text..
		Image uiImage = bindBtn.GetComponentInChildren<Image> ();
		uiImage.sprite = SCMainWinController.Instance().SCMainVw.buttonSelected;
		bingText.text = @"活动名称：" + _model.actName;
	}

	public void LoginOutView( )
	{
		Image uiImage = bindBtn.GetComponentInChildren<Image> ();
		uiImage.sprite = SCMainWinController.Instance().SCMainVw.buttonUnselected;
		bingText.text = "";
		_model.Init ();
	}
}
