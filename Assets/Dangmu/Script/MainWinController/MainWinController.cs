using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

/// <summary>
/// Main window controller.
/// </summary>
public class MainWinController : DMonoSingleton<MainWinController> {
	public LoginModel LoginMgr = new LoginModel();
	public ControlModel CtrlMl = new ControlModel ();
	public LoginView LoginVw;
	public ControlView ContrlVw;
	public MainView MainVw;
	public QrcodeView QrView;
	public string webViewUrl = "" +
	              "http://yxbwx.mmarket.com/weizannew/app/index.php?i=3&c=entry&rid=32&small=0&t=1517658588&do=qd&m=dada_xianchang";

	// Use this for initialization
	void Start () {
		if ( null == instance )
			instance = this;

		LoginMgr.Init();
		CtrlMl.Init ();
		LoginVw = GetComponentInChildren<LoginView> (true);
		ContrlVw = GetComponentInChildren<ControlView> (true);
		MainVw = GetComponentInChildren<MainView> (true);
		QrView = transform.parent.GetComponentInChildren<QrcodeView> (true);

		//Register window...
		WindowsManager.Instance.addWin (typeof(LoginView), LoginVw);
		WindowsManager.Instance.addWin (typeof(ControlView), ContrlVw);
		WindowsManager.Instance.addWin (typeof(MainView), MainVw);
		WindowsManager.Instance.addWin (typeof(QrcodeView), QrView);

		//for test., to delete..
		//StartCoroutine(test());
	}

	/*
	IEnumerator test()
	{
		yield return new WaitForSeconds(5);
		WindowsManager.Instance.openWin( typeof(LoginView) );

		LoginVw.Login();
	}*/
	
	// Update is called once per frame
	void Update () {
		//nothing to do...
	}

	/// <summary>
	/// Exit this instance for x button.
	/// </summary>
	public void Exit()
	{
		Application.Quit ();
	}

	/// <summary>
	/// Minisizes or maxsizes control.
	/// TODO: event based..
	/// </summary>
	public void Minsize()
	{
		//close icon and open
		WindowsManager.Instance.OpenAndCloseWin(typeof(MinIconView), typeof(MainView));
	}

	/// <summary>
	/// Inits the dangmu when logining.
	/// </summary>
	public void InitDangmu()
	{
		//for test
		//发送并处理弹幕初始化消息...
		//先同步获取初始化消息，_lastMessageEventArgs获取结果....
		//NetWorkManager.Instance.socket.IsSyncHttpDone = false;
		DangmuInitReq req = new DangmuInitReq ();
		req.weixinId = LoginMgr.WexinId;
		req.rid = LoginMgr.CurRid;

		//for test...
		Log.info("InitDangmu():" + req.GetUrlString ());
		StartCoroutine ( NetWorkManager.Instance.socket.GetHttpRequest( req.GetUrlString (), InitResponse ) );
	}

	public void InitResponse()
	{
		//wait for response
		//wait for response
		if ( NetWorkManager.Instance.socket.MessageRepsonseEvent == null ) { //wait for all the time...
			InitFailed("网络没响应!");
			return;
		}

		//response get...
		//for to unserizlized
		JsonData _jsonObect = JsonMapper.ToObject( NetWorkManager.Instance.socket.MessageRepsonseEvent.Data );
		NetWorkManager.Instance.socket.IsSyncHttpDone = true;
		if (null == _jsonObect) {
			Log.error (this, "InitDangmu#################null == _jsonObect!");
			InitFailed("服务器返回消息为空!");
			return;
		}

		/*
				Packet packet = decoder.Decode(e);*/
		//直接构造DangmuViewDataConveter...
		//目前无法区分消息类型....
		DangmuInitRsp initRsp = new DangmuInitRsp();
		if ( false == initRsp.ParseJsonObject(_jsonObect) || initRsp.result < 0 ){
			Log.error (this, "InitDangmu#################ParseJsonObject failed!");
			InitFailed ("服务器返回登录失败!");
			return;
		}

		InitSuccess ( initRsp );
	}

	/// <summary>
	/// Login this instance.
	/// 登录失败处理...
	/// </summary>
	void InitFailed(string strFail)
	{
		LoginVw.UnLockUI ();
		LoginMgr.IsLogined = false;
		if (LoginVw.isActiveAndEnabled)
			LoginVw.LoginFailed (strFail);
	}

	/// <summary>
	/// Login this instance.
	/// 登录成功处理...
	/// </summary>
	void InitSuccess(DangmuInitRsp initRsp)
	{
		LoginMgr.IsLogined = true;

		//start Open qrcodeView..
		WindowsManager.Instance.openWin( typeof(QrcodeView) );

		if (LoginMgr.IndexId < initRsp.lastId)
			LoginMgr.IndexId = initRsp.lastId;

		//Login successed...
		WindowsManager.Instance.OpenAndCloseWin( typeof(ControlView) ,typeof(LoginView) );
		if ( initRsp.dataList.Count <= 0 ){
			Log.debug (this, "InitDangmu#################No new message!");
			return;
		}

		//查找WexinId和IndexId...
		//_model.WexinId = initRsp.dataList [0].weixinId;
		foreach (DangmuInitRsp.DangmuInitData initData in initRsp.dataList) {
			DangmuViewDataConveter dataConv = new DangmuViewDataConveter ( initData );

			//insert derectly....
			//if ( true == DangmuTextController.Instance.Displayer.Enable )
				//DangmuTextController.Instance.enqueueToDataQueue( dataConv );

			//if ( true == VerticalTextController.Instance.Displayer.Enable )
				VerticalTextController.Instance.enqueueToDataQueue( dataConv );
		}

		//插入写测试数据...
		//TODO: To delete... 
		/*
		DangmuViewDataConveter dataTemp = new DangmuViewDataConveter (27, 3, 2, "oYZTz0n7_wbHTNjn9euQCdDjO2p0", "老虎怕小羊", 
			"http://wx.qlogo.cn/mmopen/vi_32/OJcLpmXK9LVicBl9PgMyjQroh2XDPkh6AB6C1W3bxqEtVoOhbKcq546LBU4xlJxlFUhaia7u1rm1Gz1GH90ZIMwA/0", 
			"http://yxbwx.mmarket.com/weizannew/attachment/images/meepo_xianchang/N5kkwL2SvlY2N714sy1knAy7L112zu.jpg", 2, "1513326199"); //"长夜里没法睡男儿无泪..."..
		DangmuTextController.Instance.enqueueToDataQueue (dataTemp);
		VerticalTextController.Instance.enqueueToDataQueue (dataTemp);*/
	}

	/// <summary>
	/// Process querying dangmu.
	/// </summary>
	public void DangmuQuery()
	{
		if ( null == NetWorkManager.Instance.socket.MessageRepsonseEvent ) { //wait for all the time...
			Log.error(this, "DangmuQuery############服务器没有响应!");
			return;
		}

		//response get...
		//for to unserizlized
		JsonData _jsonObect = JsonMapper.ToObject( NetWorkManager.Instance.socket.MessageRepsonseEvent.Data );
		if (null == _jsonObect) {
			Log.error (this, "DangmuQuery#################null == _jsonObect!");
			return;
		}

		/*
		Packet packet = decoder.Decode(e);*/
		//目前无法区分消息类型....
		DangmuTextRsp textRsp = new DangmuTextRsp();
		if ( false == textRsp.ParseJsonObject(_jsonObect) ){
			Log.info (this, "DangmuQuery#################ParseJsonObject no data!");
			return;
		}

		//TODO: lastid更新完内容还是没有....
		if (LoginMgr.IndexId < textRsp.lastId)
			LoginMgr.IndexId = textRsp.lastId;

		//insert data into  dangmu..
		foreach (DangmuTextRsp.DangmuTextData initData in textRsp.dataList) {
			DangmuViewDataConveter dataConv = new DangmuViewDataConveter ( initData );

			//insert derectly....
			//if ( true == DangmuTextController.Instance.Displayer.Enable )
				//DangmuTextController.Instance.enqueueToDataQueue( dataConv );

			if ( true == VerticalTextController.Instance.Displayer.Enable )
				VerticalTextController.Instance.enqueueToDataQueue( dataConv );
		}
	}
}
