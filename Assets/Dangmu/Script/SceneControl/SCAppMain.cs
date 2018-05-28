using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCAppMain : DMonoSingleton<SCAppMain> {
	[Header("整个界面帧率全局设置")]
	public int frameRate = 30;
	//public ResizableWindow resizeWindow;
	/* 本身资源初始化... */
	void Awake()
	{
		if(instance == null)
			instance =  this;

		//日志开关统一这儿设置....
		//Log.addLevel (Log.ERROR);
		Log.addLevel (Log.ALL);
		Application.targetFrameRate = frameRate; //frame set..

		Log.debug(this, "displays connected: " + Display.displays.Length);
		// Display.displays[0] 是主显示器, 默认显示并始终在主显示器上显示.   ...     
		// 检查其他显示器是否可用并激活.        
		if (Display.displays.Length > 1) {
			Display.displays [1].Activate ();
			/*
			int screenWidth = Display.displays [1].systemWidth;
			int screenHeight = Display.displays [1].systemHeight;
			int fWidth = screenWidth / 2 + screenWidth / 3;
			int fHeight = screenHeight / 2 + screenHeight / 3;
			int posX = screenWidth / 2 - fWidth / 2;
			int poxY = screenHeight / 2 - fHeight / 2;
			Display.displays [1].SetParams (fWidth, fHeight, posX, poxY);*/
		}
		if (Display.displays.Length > 2) {
			Display.displays [2].Activate ();
			//int fWidth = Screen.width / 2 + Screen.width / 3;
			//int fHeight = Screen.height / 2 + Screen.height / 3;
			//int posX = Screen.width / 2 - fWidth / 2;
			//int poxY = Screen.height / 2 - fHeight / 2;
			//Display.displays [1].SetParams (fWidth, fHeight, 0, 0);
		}
		//main connect here..
		//NetworkManager.Instance.Awake ();

		//ResourcePool.Instance.Init ();
		#if USE_LINUX_SERVER
		GameReportManager.Instance.InitEvent ();
		#endif
	}

	// Use this for initialization
	void Start () {
		//network init
		NetWorkManager.Instance.Init();
		SCWindowsManager.Instance.Init ();
	
	}

	// Update is called once per frame
	void Update () {
		//未登录不进入弹幕世界...
		//if ( false == MainWinController.Instance().LoginMgr.IsLogined )
			//return;

		//network update...
		NetWorkManager.Instance.Update();

		//for all win manager..
		SCWindowsManager.Instance.Update (Time.deltaTime);

		//for all model controoler
		//DangmuTextController.Instance.Update ();

		//TODO:
		//time update...
		//TimeScaleManager.Instance.Update ();
	}
}
