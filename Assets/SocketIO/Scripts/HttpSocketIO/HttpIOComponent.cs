using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using LitJson;

namespace HttpIO
{
	public class HttpIOComponent : MonoBehaviour {
		#region Public Properties

		public string url = "http://120.197.235.92/mssp_pps/";
		public string path = "draw/addOpportunity.do"; //prize url...

		//public bool autoConnect = true;
		//public int timeoutDelay = 5;

		public HttpSocket socket { get { return hs; } }

		#endregion

		#region Private Properties

		//private Thread socketThread;
		private HttpSocket hs;

		private Encoder encoder;
		private Decoder decoder;
		//private Parser parser;

		private Dictionary<string, List<Action<SocketIOEvent>>> handlers;

		//private object eventQueueLock; //not need present..
		private Queue<SocketIOEvent> eventQueue;
		#endregion

		#if SOCKET_IO_DEBUG
		public Action<string> debugMethod;
		#endif

		#region Unity interface

		public void Awake()
		{
			encoder = new Encoder();
			decoder = new Decoder();
			//parser = new Parser();
			handlers = new Dictionary<string, List<Action<SocketIOEvent>>>();

			hs = new HttpSocket(false);
			hs.OnMessage += OnMessage;
			hs.OnError += OnError;

			eventQueue = new Queue<SocketIOEvent>();

			#if SOCKET_IO_DEBUG
			if(debugMethod == null) { debugMethod = Debug.Log; };
			#endif
		}

		public void Update()
		{
			hs.FrameMove ();

			while(eventQueue.Count > 0){
				EmitEvent(eventQueue.Dequeue());
			}

			//else to do?...
		}

		//must done.
		public JSONObject GetResponseJsonObject()
		{
			WebSocketSharp.MessageEventArgs e = hs.MessageRepsonseEvent;
			if ( e == null ) 
				return null;

			JSONObject jsonObject = new JSONObject( e.Data );
			jsonObject.bClassNameExist = false;
			return jsonObject;
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
			StartCoroutine( hs.PostJsonData( urlPath, encoder.Encode(packet) ) );

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
			StartCoroutine( hs.PostJsonData( urlPath, rawpacket ) );

			Debug.Log ("Data sended,[url]:"+urlPath);
			Debug.Log (" [JsonPayLoad]:"+rawpacket);
		}

		//uniform asyncronzilly interface...
		protected void OnMessage(object sender, WebSocketSharp.MessageEventArgs e)
		{
			#if SOCKET_IO_DEBUG
			debugMethod.Invoke("[SocketIO] Raw message: " + e.Data);
			#endif
			Debug.Log ("[SocketIO] OnMessage e.Data: " + e.Data);

			//for to unserizlized
			JSONObject jsonObject = new JSONObject( e.Data );
			jsonObject.bClassNameExist = false;

			/*
				Packet packet = decoder.Decode(e);*/
			/*

			switch (jsonObject.className) {
			case GameAccountRegisterRsp.CLASS_NAME: 	HandJsonObject(jsonObject);		break;
			case GameAccountLoginRsp.CLASS_NAME: 	HandLoginJsonObject(jsonObject);		break;
			case GameBetRsp.CLASS_NAME: 	HandBetJsonObject(jsonObject);		break;
				//case EnginePacketType.CLOSE: 	EmitEvent("close");		break;
				//case EnginePacketType.PING:		HandlePing();	   		break;
				//case EnginePacketType.PONG:		HandlePong();	   		break;
				//case EnginePacketType.MESSAGE: 	HandleMessage(packet);	break;
			}*/
			HandSynChronizeJsonObject (jsonObject);
		}
			
		//直接响应处理...
		private void HandSynChronizeJsonObject(JSONObject jsonObject)
		{
			//根据classname的map找到类型是GameAccountRegisterRsp....
//			GameAccountRegisterRsp rsp = new GameAccountRegisterRsp();
//			rsp.ParseJsonObject (jsonObject);
			Debug.Log ("[SocketIO] HandSynChronizeJsonObject not implented yet! ");
		}

		private void OnError(object sender, WebSocketSharp.ErrorEventArgs e)
		{
			Debug.Log ("OnError: " + e.Message);
			//EmitEvent("error");
		}


		private void EmitEvent(string type)
		{
			EmitEvent(new SocketIOEvent(type));
		}

		private void EmitEvent(SocketIOEvent ev)
		{
			if (!handlers.ContainsKey(ev.name)) { return; }
			foreach (Action<SocketIOEvent> handler in this.handlers[ev.name]) {
				try{
					handler(ev);
				} catch(Exception ex){
					#if SOCKET_IO_DEBUG
					debugMethod.Invoke(ex.ToString());
					#endif
					Log.info (ex.ToString());
				}
			}
		}
		#endregion
	}
}