using UnityEngine;
using System.Collections;
using System.Text;
using System;
using System.Globalization;
using System.Collections.Generic;
using WebSocketSharp;

namespace HttpIO
{
	public class HttpSocket {
		protected WWWForm header;
		//protected WWW www;

		[SerializeField] public int MaxTimes = 5;
		[SerializeField] public float WaitPerTime = 1.0f;

		private bool isNeedAsync = false; 
		public bool OpenHttpAsync
		{
			get { return isNeedAsync; }
			set { isNeedAsync = value; }
		}

		/// <summary>
		/// The is sync http done.
		/// TODO: 统一整改...
		/// 事务是否异步处理标记...
		/// </summary>
		public bool IsSyncHttpDone = true;

		private WebSocketSharp.MessageEventArgs _lastMessageEventArgs;
		public WebSocketSharp.MessageEventArgs MessageRepsonseEvent
		{
			get { return _lastMessageEventArgs; }
			set { _lastMessageEventArgs = value; }
		}

		private Queue<WebSocketSharp.MessageEventArgs> _messageEventQueue;

		/// <summary>
		/// Occurs when the <see cref="WebSocket"/> gets an error.
		/// </summary>
		public event EventHandler<WebSocketSharp.ErrorEventArgs> OnError;

		/// <summary>
		/// Occurs when the <see cref="WebSocket"/> receives a message.
		/// </summary>
		public event EventHandler<WebSocketSharp.MessageEventArgs> OnMessage;

		public HttpSocket (bool bNeedAsync)
		{
			_lastMessageEventArgs = null;
			isNeedAsync = bNeedAsync;
			IsSyncHttpDone = true;
			MaxTimes = 5;
			WaitPerTime = 1.0f;
			_messageEventQueue = new Queue<MessageEventArgs> ();
		}

		// Use this for initialization
		public void clear () {
			header.headers.Clear ();
			_lastMessageEventArgs = null;
		}

		public void AddHeader(string key, string value)
		{
			Dictionary<string, string> headers = header.headers; 
			headers [key] = value;
		}

		//异步统一接口,....
		public void FrameMove()
		{
			//同步方式...
			if ( OpenHttpAsync ){
				if (IsSyncHttpDone) {
					if (_lastMessageEventArgs != null)
						OnMessage (this, _lastMessageEventArgs);
				}
				return;
			}

			//异步方式...
			try {
				var e = dequeueFromMessageEventQueue ();
				if ( e != null )
					OnMessage.Emit (this, e);
			}
			catch (Exception ex) {
				//acceptException (ex, "An exception has occurred while OnMessage.");
				error (ex.ToString() + " An exception has occurred while OnMessage."); 
			}
		}
		
		public IEnumerator PostJsonData(string url, string data, Action callback = null)
		{
			while ( !isNeedAsync && false == IsSyncHttpDone )
				yield return 0;

			IsSyncHttpDone = false;
			_lastMessageEventArgs = null;
			header = new WWWForm ();
			Dictionary<string, string> headers = header.headers;  
			byte[] rawData = Encoding.UTF8.GetBytes(data);  
			headers["Content-Type"] = "application/json";  
			headers["Accept"] = "application/json"; 

			DateTime date = DateTime.Now;  
			string time =  date.ToString("ddd, yyyy-mm-dd HH':'mm':'ss 'UTC'", DateTimeFormatInfo.InvariantInfo);  
			headers["Date"] = time;

			//www = new WWW(WWW.EscapeURL(url), rawData, headers);  
			WWW www = new WWW(url, rawData, headers);  
			yield return www;

			//throw out events
			if (www.error != null) 
			{ 
				Debug.Log("error is [PostJsonData]:"+ www.error  + "  [Data]:" + rawData );      
				error (www.error); 
			} 
			else 
			{ 
				//最多等5s...
				int maxTimeout = MaxTimes;
				while (!www.isDone) {
					yield return new WaitForSeconds(WaitPerTime);
					maxTimeout--;
					if (maxTimeout <= 0){
						IsSyncHttpDone = true;
						yield break;
					}
				}

				Debug.Log("request ok response: " + www.text);

				_lastMessageEventArgs = new WebSocketSharp.MessageEventArgs (WebSocketSharp.Opcode.Text, www.text);
				IsSyncHttpDone = true;

				if (callback != null)
					callback.Invoke ();

				if ( isNeedAsync )
					enqueueToMessageEventQueue (_lastMessageEventArgs);

				/*
				JsonData jd = JsonMapper.ToObject(www.text);   

				string memberId = jd["memberId"].ToString();  
				string bonusPoint = jd["bonusPoint"].ToString();  
				string nickName = jd["nickName"].ToString();*/ 
			} 
		}

		/// <summary>
		/// The http-get request.
		/// </summary>
		/// <returns>The http request.</returns>
		/// <param name="url">URL.</param>
		public IEnumerator GetHttpRequest(string url, Action callback)
		{
			//Debug.Log ("GetHttpRequest: IsSyncHttpDone" + IsSyncHttpDone + ": isNeedAsync: " + isNeedAsync);
			while ( !isNeedAsync && false == IsSyncHttpDone )
				yield return 0;
			Debug.Log ("GetHttpRequest:" + url);

			IsSyncHttpDone = false;
			_lastMessageEventArgs = null;

			//www = new WWW(WWW.EscapeURL(url), rawData, headers);  
			WWW www = new WWW(url);  
			yield return www;

			//最多等5s...
			int maxTimeout = MaxTimes;
			while ( !www.isDone ) {
				yield return new WaitForSeconds(WaitPerTime);
				Debug.Log ("GetHttpRequest wait 1 second");
				maxTimeout--;
				if (maxTimeout <= 0) {
					IsSyncHttpDone = true;
					yield break;
				}
			}

			//throw out events
			if (www.error != null && www.error != "") 
			{ 
				Debug.Log("error is [GetHttpRequest]:"+ www.error  + "  [url]:" + url );      
				error (www.error); 
			}
			Debug.Log("request ok response: " + www.text);

			//mutex access..
			_lastMessageEventArgs = new WebSocketSharp.MessageEventArgs (WebSocketSharp.Opcode.Text, www.text);
			IsSyncHttpDone = true;

			if (callback != null)
				callback.Invoke ();
			
			if (isNeedAsync)
				enqueueToMessageEventQueue (_lastMessageEventArgs);

			/*
			else
				OnMessage.Emit (this, _lastMessageEventArgs);*/

			/*
			JsonData jd = JsonMapper.ToObject(www.text);   

			string memberId = jd["memberId"].ToString(); */ 
		}

		private void error (string message)
		{
			try {
				OnError.Emit (this, new WebSocketSharp.ErrorEventArgs (message));
				//OnError.Invoke(this, new WebSocketSharp.ErrorEventArgs (message));
			}
			catch (Exception ex) {
				Debug.LogError ("An exception has occurred while OnError:\n" + ex.ToString ());
			}
		}

		private void enqueueToMessageEventQueue (MessageEventArgs e)
		{
			//lock (_forMessageEventQueue)
			//coroutine not need lock..
			_messageEventQueue.Enqueue (e);
		}

		private MessageEventArgs dequeueFromMessageEventQueue ()
		{
			//lock (_forMessageEventQueue)
			return _messageEventQueue.Count > 0
				? _messageEventQueue.Dequeue ()
					: null;
		}
	}
}
