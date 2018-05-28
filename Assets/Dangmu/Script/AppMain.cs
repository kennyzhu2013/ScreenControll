using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// App main.
/// LoginManager登录后的主程序入口...
/// 登录后激活.....
/// </summary>
public class AppMain : DMonoSingleton<AppMain> {

	/* 本身资源初始化... */
	void Awake()
	{
		if(instance == null)
			instance =  this;

		//日志开关统一这儿设置....
		//Log.addLevel (Log.ERROR);
		Log.addLevel (Log.ALL);
		Application.targetFrameRate = 30; //frame set..

		//main connect here..
		//NetworkManager.Instance.Awake ();

		//PatchManager.Instance.Init ();
		//AssetBundleManager.Instance.Init ();
		//ResourcePool.Instance.Init ();
		#if USE_LINUX_SERVER
		GameReportManager.Instance.InitEvent ();
		SystemUnlockManager.Instance.RegisterSystem ();
		#endif
	}

	// Use this for initialization
	void Start () {
		//network init
		NetWorkManager.Instance.Init();
		WindowsManager.Instance.Init ();
		//DangmuTextController.Instance.Init ();
		VerticalTextController.Instance.Init ();
	}

	// Update is called once per frame
	void Update () {
		//未登录不进入弹幕世界...
		if ( false == MainWinController.Instance().LoginMgr.IsLogined )
			return;
		
		//network update...
		NetWorkManager.Instance.Update();

		//for all win manager..
		WindowsManager.Instance.Update (Time.deltaTime);

		//for all model controoler
		//DangmuTextController.Instance.Update ();
		VerticalTextController.Instance.Update ();

		//TODO:
		//time update...
		//TimeScaleManager.Instance.Update ();
	}
}
