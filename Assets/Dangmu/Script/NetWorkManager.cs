using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HttpIO;
using System;
using LitJson;
using Common;

/// <summary>
/// Net work manager.
/// 参考HttpIOComponent实现,这里为异步实现....
/// update函数必须在appmain后进行...
/// </summary>
public class NetWorkManager : SingletonT<NetWorkManager> {
	#region Public Properties
	public string url = "http://120.197.235.92/mssp_pps/";
	public string path = "draw/addOpportunity.do"; //prize url...


	public HttpSocket socket { get { return hs; } }
	public float QueryGap { get { return queryTicks; } }
	#endregion

	#region Private Properties

	//private Thread socketThread;
	private HttpSocket hs;
	public bool NetWorkAsync
	{
		get { return hs.OpenHttpAsync; }
		set { hs.OpenHttpAsync = value; }
	}

	private float timeCacl;
	private float queryTicks;

	private Encoder encoder;
	private Decoder decoder;
	//private Parser parser;

	#endregion

	#if SOCKET_IO_DEBUG
	public Action<string> debugMethod;
	#endif

	#region Unity interface

	public void Init()
	{
		
		encoder = new Encoder();
		decoder = new Decoder();
		//parser = new Parser();
		//handlers = new Dictionary<string, List<Action<SocketIOEvent>>>();

		//
		//sync,改用同步方式.......
		hs = new HttpSocket(false);
		hs.OnMessage += OnMessage;
		hs.OnError += OnError;

		timeCacl = 0.0f;
		queryTicks = 1.0f;
		#if SOCKET_IO_DEBUG
		if(debugMethod == null) { debugMethod = Debug.Log; };
		#endif
	}

	public void Update()
	{
		//not framemove for present...
		hs.FrameMove ();

		timeCacl += Time.deltaTime;
		if ( timeCacl >= QueryGap ) {
			//TicksQuery();
			timeCacl = 0.0f;
		}

		//else to do?...
	}


	public JsonData GetResponseJsonData()
	{
		WebSocketSharp.MessageEventArgs e = hs.MessageRepsonseEvent;
		if ( e == null ) 
			return null;

		JsonData jsonData = JsonMapper.ToObject( e.Data );
		//jsonObject.bClassNameExist = false;
		return jsonData;
	}
	#endregion

	#region Private Methods
	private void EmitMessage(string raw)
	{
		EmitPacket(new Packet(new JSONObject(raw)));
	}

	//same as EmitRawMessage..
	private void EmitPacket(Packet packet)
	{
		#if SOCKET_IO_DEBUG
		debugMethod.Invoke("[SocketIO] " + packet);
		#endif

		//directly coroutine..
		//TODO:Synchronize better?not realize...
		string urlPath = url + path;
		hs.IsSyncHttpDone = false;
		SCAppMain.Instance().StartCoroutine( hs.PostJsonData( urlPath, encoder.Encode(packet) ) );

		Debug.Log ("Data sended,[url]:" + urlPath);
		Debug.Log (" [JsonPayLoad]:"+ encoder.Encode(packet));
	}


	//betMsg
	public void EmitRawMessage(string rawpacket)
	{
		#if SOCKET_IO_DEBUG
		debugMethod.Invoke("[SocketIO] " + packet);
		#endif

		//directly coroutine..
		//TODO:Synchronize better?not realize...
		string urlPath = url + path;
		hs.IsSyncHttpDone = false;
		SCAppMain.Instance().StartCoroutine( hs.PostJsonData( urlPath, rawpacket ) );

		Debug.Log ("Data sended,[url]:"+urlPath);
		Debug.Log (" [JsonPayLoad]:"+rawpacket);
	}

	/// <summary>
	/// Gets the raw message.
	/// Http-Get
	/// </summary>
	/// <param name="httpurl">Httpurl.</param>
	public void GetRawMessage(string httpurl)
	{
		#if SOCKET_IO_DEBUG
		debugMethod.Invoke("[SocketIO] " + packet);
		#endif

		//directly coroutine..
		//TODO:Synchronize better?not realize..
		hs.IsSyncHttpDone = false;
		SCAppMain.Instance().StartCoroutine( hs.GetHttpRequest( httpurl, null ) );

		Log.info ( this, "Data sended,[url]:" + httpurl );
	}

	/// <summary>
	/// Raises the message event.
	/// uniform asyncronzilly interface, call-back...
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	protected void OnMessage(object sender, WebSocketSharp.MessageEventArgs e)
	{
		#if SOCKET_IO_DEBUG
		debugMethod.Invoke("[SocketIO] Raw message: " + e.Data);
		#endif
		Debug.Log ("[SocketIO] OnMessage e.Data: " + e.Data);
		hs.MessageRepsonseEvent = null;
		hs.IsSyncHttpDone = true;

		//for to unserizlized
		JsonData _jsonObect = JsonMapper.ToObject( e.Data );
		if (null == _jsonObect) {
			Log.error ("NetWorkManager", "OnMessage#################null == _jsonObect!");
			return;
		}

		/*
				Packet packet = decoder.Decode(e);*/
		//直接构造DangmuViewDataConveter...
		//目前无法区分消息类型....
		DangmuTextRsp textRsp = new DangmuTextRsp();
		if ( false == textRsp.ParseJsonObject(_jsonObect) ){
			Log.error ("NetWorkManager", "OnMessage#################ParseJsonObject failed!");
			return;
		}

		if ( textRsp.dataList.Count <= 0 ){
			Log.debug ("NetWorkManager", "OnMessage#################No new message!");
			return;
		}

		foreach (DangmuTextRsp.DangmuTextData textData in textRsp.dataList) {
			DangmuViewDataConveter dataConv = new DangmuViewDataConveter ( textData );

			//insert derectly....
			//SCMainWinController.Instance().DangmuControl.enqueueToDataQueue( dataConv );
		}

		//null to do...
	}

	/// <summary>
	/// Tickses the query.
	/// 定时轮询服务器....
	/// </summary>
	/*private void TicksQuery()
	{
		//response not receive...
		if ( false == SCMainWinController.Instance().ScModel.IsLogined ) {
			return;
		}
		
		//定时轮询,send DangmuTextReq...
		// if rid not change...
		//hs.IsSyncHttpDone = false;
		DangmuTextReq req = new DangmuTextReq ();
		req.rid = SCMainWinController.Instance().ScModel.CurRid;
		req.weixinId = SCMainWinController.Instance().ScModel.WexinId;
		req.lastid = SCMainWinController.Instance ().ScModel.IndexId;

		SCMainWinController.Instance().StartCoroutine( 
			NetWorkManager.Instance.socket.GetHttpRequest( req.GetUrlString (), SCMainWinController.Instance().DangmuQuery ) );
		//MainWinController.Instance().StartCoroutine( 
			//NetWorkManager.Instance.socket.GetHttpRequest( req.GetUrlString (), MainWinController.Instance().DangmuQuery ) );
	}*/

	private void OnError(object sender, WebSocketSharp.ErrorEventArgs e)
	{
		Log.error ("NetWorkManager", "OnError: " + e.Message);
		hs.MessageRepsonseEvent = null;

		//EmitEvent("error");
	}
		
	#endregion
}
