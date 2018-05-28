using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class SCMainWinController : DMonoSingleton<SCMainWinController> {
	public SCMainWinModel ScModel = new SCMainWinModel();

	public MainView MainVw;

	//public ControlModel CtrlMl = new ControlModel ();
	//public LoginView LoginVw;
	public SCMainView SCMainVw;
	public TopLeftView TopLeftVw;
	public TopRightView TopRhtVw;
	public SCLoginView LoginVw;
	public WebBulletScreenView DangmuVw;

	//public FullWindowView FullWinVw;
	public FileMaterialDiaLog FileMatVw;
	//public CombBulletScreenDisplayer DangmuVw;
	//public DangmuTextController DangmuControl;

	// Use this for initialization
	void Start () {
		if ( null == instance )
			instance = this;

		ScModel.Init();

		//init view...
		if (null == SCMainVw)
			SCMainVw = GameObject.Find ("MainWindow").GetComponent<SCMainView> ();

		if (null == FileMatVw)
			FileMatVw = GameObject.Find ("MaterialWindow").GetComponent<FileMaterialDiaLog> ();
		
		if (null == TopLeftVw)
			TopLeftVw = GameObject.Find ("Canvas-TopLeft").GetComponent<TopLeftView> ();

		if (null == TopRhtVw)
			TopRhtVw = GameObject.Find ("Canvas-TopRight").GetComponent<TopRightView> ();

		if (null == LoginVw)
			LoginVw = GameObject.Find ("LoginWindow").GetComponent<SCLoginView> ();

		//if (null == MainVw)
			//MainVw = GameObject.Find ("MainWindow").GetComponent<MainView> ();

		//
		SCWindowsManager.Instance.addWin (typeof(SCMainView), SCMainVw);
		SCWindowsManager.Instance.addWin (typeof(FileMaterialDiaLog), FileMatVw);
		SCWindowsManager.Instance.addWin (typeof(SCLoginView), LoginVw);
		SCWindowsManager.Instance.addWin (typeof(WebBulletScreenView), DangmuVw);
		SCWindowsManager.Instance.addWin (typeof(MainView), MainVw);

		//SCWindowsManager.Instance.addWin (typeof(FullWindowView), FullWinVw);

		//DangmuControl = new DangmuTextController ();
		//DangmuControl.Init ();
	}

	/// <summary>
	/// Inits the Scene view when logining.
	/// </summary>
	public void InitSceneControlAfterLogined()
	{
	}
	
	// Update is called once per frame
	void Update () {
		//Dangmu control process...
		if ( false == ScModel.IsLogined )
			return;

		//DangmuControl.Update ();
	}

	/// <summary>
	/// Minisizes or maxsizes control.
	/// TODO: event based..
	/// </summary>
	public void Minsize()
	{
		//close icon and open
		//WindowsManager.Instance.closeWin( typeof(MainView), true );
		//MainVw.Hide ();
		SCWindowsManager.Instance.OpenAndCloseWin(typeof(MinIconView), typeof(MainView));
	}

	/// <summary>
	/// Exit this instance for x button.
	/// </summary>
	public void Exit()
	{
		Application.Quit ();
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
		/*
		DangmuInitReq req = new DangmuInitReq ();
		req.weixinId = ScModel.WexinId;
		req.rid = ScModel.CurRid;

		//for test...
		Log.info("InitDangmu():" + req.GetUrlString ());
		StartCoroutine ( NetWorkManager.Instance.socket.GetHttpRequest( req.GetUrlString (), InitResponse ) );
		*/
		//string webUrl = "http://yxbwx.mmarket.com/weizannew/app/index.php?i=3&c=entry&rid=32&small=0&t=1524731905&do=barwall&m=dada_xianchang";
		string webUrl = "http://yxbwx.mmarket.com/weizannew/app/index.php?i=" + ScModel.WexinId + "&c=entry&rid=" + ScModel.CurRid 
			+ "&pw=" + ScModel.Passwd + "&small=0&t=1524731905&do=barwall&m=dada_xianchang&dm=1";// + ScModel.LoginTime?
		StartCoroutine ( InitWebDangmuView ( webUrl ) );

		//qu shui yin
		if (false == SCMainWinController.Instance ().ScModel.IsMark)
			DisableShuiyin ();
	}

	private IEnumerator InitWebDangmuView( string webUrl )
	{
		ScModel.IsLogined = true;

		//start Open qrcodeView..Not need 
		//SCWindowsManager.Instance.openWin( typeof(QrcodeView) );

		DangmuVw.BulletUrl = webUrl;
		//TransparentWindow.SetWindowsMouseDisable ();
		SCWindowsManager.Instance.OpenAndCloseWin( typeof(WebBulletScreenView), typeof(SCLoginView) );

		yield return 0;
	}

	private void DisableShuiyin()
	{
		TopRhtVw.shuiYin.enabled = false;
		TopLeftVw.shuiYin.enabled = false;
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
		ScModel.IsLogined = false;
		if (LoginVw.isActiveAndEnabled)
			LoginVw.LoginFailed (strFail);
	}

	/// <summary>
	/// Login this instance.
	/// 登录成功处理...
	/// </summary>
	void InitSuccess(DangmuInitRsp initRsp)
	{
		ScModel.IsLogined = true;

		//start Open qrcodeView..Not need 
		//SCWindowsManager.Instance.openWin( typeof(QrcodeView) );

		if (ScModel.IndexId < initRsp.lastId)
			ScModel.IndexId = initRsp.lastId;
		//DangmuControl.DanmuCurStatus = DangmuTextController.DangmuStatus.DangmuStatus_Inited;

		//TransparentWindow.SetWindowsMouseDisable ();
		//Login successed...
		//ControlView not used any more
		//WindowsManager.Instance.OpenAndCloseWin( typeof(ControlView) ,typeof(SCLoginView) );
		SCWindowsManager.Instance.OpenAndCloseWin( typeof(WebBulletScreenView), typeof(SCLoginView) );
		if ( initRsp.dataList.Count <= 0 ){
			Log.debug (this, "InitDangmu#################No new message!");
			return;
		}

		//Log.debug (this, "InitSuccess: count:" + initRsp.dataList.Count);
		//查找WexinId和IndexId...
		//_model.WexinId = initRsp.dataList [0].weixinId;
		foreach (DangmuInitRsp.DangmuInitData initData in initRsp.dataList) {
			DangmuViewDataConveter dataConv = new DangmuViewDataConveter ( initData );

			//insert derectly....
			//if ( true == DangmuTextController.Instance.Displayer.Enable )
			//DangmuControl.enqueueToDataQueue( dataConv );

			//if ( true == VerticalTextController.Instance.Displayer.Enable )
			//VerticalTextController.Instance.enqueueToDataQueue( dataConv );
		}
		//DangmuControl.DanmuCurStatus = DangmuTextController.DangmuStatus.DangmuStatus_Opening;
	}

	/// <summary>
	/// Process querying dangmu.
	/// </summary>
	/*
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

		//目前无法区分消息类型....
		DangmuTextRsp textRsp = new DangmuTextRsp();
		if ( false == textRsp.ParseJsonObject(_jsonObect) ){
			Log.info (this, "DangmuQuery#################ParseJsonObject no data!");
			return;
		}

		//TODO: lastid更新完内容还是没有....
		if (ScModel.IndexId < textRsp.lastId)
			ScModel.IndexId = textRsp.lastId;

		//insert data into  dangmu..
		foreach (DangmuTextRsp.DangmuTextData initData in textRsp.dataList) {
			DangmuViewDataConveter dataConv = new DangmuViewDataConveter ( initData );

			//insert derectly....
			if ( true == DangmuVw.Enable )
				DangmuControl.enqueueToDataQueue( dataConv );

			//if ( true == VerticalTextController.Instance.Displayer.Enable )
			//VerticalTextController.Instance.enqueueToDataQueue( dataConv );
		}
	}
    */
	//
	public void LoginOut()
	{
		SCWindowsManager.Instance.closeWin( typeof(WebBulletScreenView) );
		SCWindowsManager.Instance.closeWin ( typeof(SCLoginView) );
		LoginVw.LoginOutView ( );
	}
}
