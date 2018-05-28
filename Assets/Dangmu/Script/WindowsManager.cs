using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using System;

/// <summary>
/// Windows manager. For windows message controller...
/// 基于消息，not event based...
/// </summary>
public class WindowsManager : SingletonT<WindowsManager> {
	private DefaultMessageHandler message_handler = new DefaultMessageHandler ();
	public DefaultMessageHandler WindowsMessageHdl
	{
		get { return message_handler; }
	}

	//for UIWindowManager..mutex views cache..
	private  Dictionary<string,BaseView> onlyOpenOneWins = new Dictionary<string, BaseView>();

	//state = opened windows...
	private List<Type> currentOpenLists = new List<Type>();
	private List<Type> dontDeleteTypes;

	private List<Type> backList = new List<Type>(); //for jump..Zero
	private float openTime = 0.0f; //
	private float openCD = 1.5f; //forbiden open and close too fast...
	private GameObject root;

	// Use this for initialization...
	// only support one canvas for present..
	public void Init () {
		root = GameObject.FindObjectOfType<Canvas> ().gameObject;
	}

	
	// Update is called once per frame
	public void Update(float time) {
		message_handler.Update ();

		if (openTime > 0)
			openTime -= time;//Time.deltaTime;
	}

	public bool ContainerWin(Type type)
	{
		if(currentOpenLists.Contains(type))
			return true;
		return false;
	}

	/// <summary>
	/// Opens the dested window.
	/// </summary>
	/// <param name="type">Type.</param>
	public void openWin(Type type)
	{
		if( !currentOpenLists.Contains(type) )
			currentOpenLists.Add(type);

		if(getView(type) != null)
		{
			getView(type).Show();
		}
		else
		{
			//Dynamic create...
			//GameObject parent = NGUIManager.Instance.getAnchor(PanelDepth.Box, UIAnchor.Side.Center).gameObject;
			//GameObject loadObject = NGUITools.AddChild(parent);
			//find fro all children object...
			BaseView view = (BaseView)root.GetComponentInChildren(type);

			//register..
			onlyOpenOneWins[type.ToString()] = view;
			view.Show ();
		}
		//SetblackBg(true);
		openTime =1.5f;
	}

	/// <summary>
	/// Closes the window.
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="canForceClose">If set to <c>true</c> can force close.</param>
	public  void closeWin(Type type, bool canForceClose = false)
	{
		if(canForceClose == false)
		{
			//cd not reached..
			if(openTime > 0)
				return;
		}
		Debug.Log ("closeWin:===============1");

		BaseView vb = getView(type);
		if(currentOpenLists.Contains(type)!=null)
			currentOpenLists.Remove(type);
		//		Log.info("======="+type+"==="+currentOpenLists.Count);

		//跳转界面..not used yet...
		if(currentOpenLists.Count < 1 && backList.Count>0)
		{
			Type tempType = backList[backList.Count-1];
			backList.Remove(tempType);
			openWin(tempType);
		}

		if(FilterDontDeleteWin(type))
		{
			//			if(SimpleCamera2.Instance!=null)
			//			SimpleCamera2.Instance.gameObject.SetActive(true);
			if(vb!=null)vb.Hide();
			Debug.Log ("closeWin:===============2:name:" + vb.name);
			return;
		}

		if(vb!=null)
		{
			Debug.Log ("closeWin:===============3:name:" + vb.name);
			vb.close();
		}

		Debug.Log ("closeWin:===============4");
	}

	/// <summary>
	/// Closes all show window..Just for safe..
	/// </summary>
	public void CloseAllShowWin()
	{
		//List<Type> currentOpenLists_temper = new List<Type>();
		for(int k=0; k < currentOpenLists.Count; k++) {
			//currentOpenLists_temper.Add(currentOpenLists[k]);
			closeWin(currentOpenLists[k], true);
		}
	}

	/// <summary>
	/// 跳转界面..transfer windows.
	/// openW  - 需要打开的界   closeW - 关闭自身界面   isBack - 设置是否需要返回...
	/// </summary>
	public void OpenAndCloseWin(Type openW, Type closeW, bool isBack=false)
	{
		if(closeW != null)
			closeWin(closeW, true);
		openWin(openW);
		if(isBack && closeW != null)
		{
			backList.Add(closeW);
		}
	}

	/// <summary>
	/// Closes all and delete...
	/// </summary>
	public void CloseAll ()
	{
		backList.Clear();
		CloseAllShowWin ();

		Dictionary<string, BaseView> temp = new Dictionary<string, BaseView>();
		foreach (KeyValuePair<string, BaseView> kv in onlyOpenOneWins)
		{
			//not delete main control and system message...
			if (kv.Key == "MainWinController")
			{
				continue;
			}
			if (kv.Value != null)
				temp.Add(kv.Key, kv.Value);
		}

		//clear only open...
		onlyOpenOneWins.Clear();
		foreach(BaseView v in temp.Values)
		{	
			v.close();
		}
	}

	/// <summary>
	/// Closes the child window and delete.
	/// </summary>
	/// <param name="type">Type.</param>
	public void CloseChildWin(Type type)
	{
		BaseView vb = getView(type);
		if(vb != null)
		{
			vb.close();
		}

		if(currentOpenLists.Contains(type) != null)
			currentOpenLists.Remove(type);
	}

	/**每个load里面保存到type对应的view**/
	/// <summary>
	/// Adds the window but not open.
	/// </summary>
	/// <returns><c>true</c>, if window was added, <c>false</c> otherwise.</returns>
	/// <param name="type">Type.</param>
	/// <param name="vb">Vb.</param>
	public bool addWin(Type type, BaseView vb)
	{
		//not realized yet...
		/*
		if(type == typeof(Loading_Load))
			return false;*/
		if( onlyOpenOneWins.ContainsKey(type.ToString()) )
			return false;
		else
		{
			onlyOpenOneWins.Add(type.ToString(),vb);
			return true;
		}

		//SetblackBg(true);
	}

	/**过滤掉不删除的界面**/
	/// <summary>
	/// Filters the dont delete window.
	/// Just hide not delete..
	/// </summary>
	/// <returns><c>true</c>, if dont delete window was filtered, <c>false</c> otherwise.</returns>
	/// <param name="type">Type.</param>
	private bool FilterDontDeleteWin(Type type)
	{
		if(dontDeleteTypes==null)
		{
			dontDeleteTypes = new List<Type>();
			dontDeleteTypes.Add( typeof(ControlView) );
			dontDeleteTypes.Add ( typeof(LoginView) );
			dontDeleteTypes.Add ( typeof(MinIconView) );
			dontDeleteTypes.Add ( typeof(MainView) );
			dontDeleteTypes.Add ( typeof(QrcodeView) );
		}

		foreach( Type tempType in dontDeleteTypes )
		{
			if(type == tempType)
				return true;
		}
		return false;
	}

	public int GetListNum()
	{
		if(currentOpenLists == null)
			return 0;
		return currentOpenLists.Count;
	}

	/// <summary>
	/// 获取打开过的界面的长度.
	/// </summary>
	/// <returns></returns>
	public int GetBackList()
	{
		if (backList != null)
		{
			return backList.Count;
		}
		return 0;
	}

	public  BaseView getView(Type type)
	{
		if(onlyOpenOneWins.ContainsKey(type.ToString()))
		{
			BaseView vb = onlyOpenOneWins[type.ToString()];
			return vb;
		}
		return null;
	}

	/// <summary>
	/// 获取最后打的界面.
	/// </summary>
	/// <returns></returns>
	public Type GetCurrentOpenList()
	{
		Type _type = null;

		if (currentOpenLists != null && currentOpenLists.Count > 0)
		{
			_type = currentOpenLists[currentOpenLists.Count - 1];
		}
		return _type;
	}

	public void ClearBackList()
	{
		backList.Clear();
	}

	//system icon remove.
	/**TODO://播放打开界面声音. Need AudioManager....**/
}

//消息ID for windows
public enum UIMessageId
{
	None,
	//
	Login,
	WinMinize, //icon 图标最小化....
	WinResumt,

	DangmuOpen, //default open?..
	DangmuClose, 

	HengSwitchOpen,
	ShuSwitchOpen,

	SetOpen,
	SetConfirm,
	SetCancel
}

/** UI系统 ,目前只有一个UI窗口没啥实际作用**/
/// <summary>
/// User interface sys type.
/// TODO: future, not used...
/// </summary>
public enum UISysType
{
	/** Shezhi**/
	UI_Login,
	UI_Control,
	UI_Set,
}
