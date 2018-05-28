using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Texture loader.
/// </summary>
public class TextureLoader : DMonoSingleton<TextureLoader> {
	/// <summary>
	/// The dic sprite list.Must construct before star() func...
	/// </summary>
	[SerializeField]
	public Dictionary<string, Sprite> dicSpriteList = new Dictionary<string, Sprite> ();

	public Sprite GetSprite(string imageurl)
	{
		if ( dicSpriteList.ContainsKey (imageurl) )
			return dicSpriteList [imageurl];

		return null;
	}


	//TODO: add index....
	public class ImageLoadEvent
	{
		public string name { get; set; }

		public Sprite image { get; set; }

		public ImageLoadEvent(string name) : this(name, null) { }

		public ImageLoadEvent(string name, Sprite data)
		{
			this.name = name;
			this.image = data;
		}

		public override string ToString()
		{
			return string.Format("[SocketIOEvent: name={0}, image={1}]", name, image.ToString());
		}
	}
	private Queue<ImageLoadEvent> eventQueue;
	private void enqueueToMessageEventQueue (ImageLoadEvent e)
	{
		//lock (_forMessageEventQueue)
		//coroutine not need lock..
		eventQueue.Enqueue (e);
	}
	private ImageLoadEvent dequeueFromMessageEventQueue ()
	{
		//lock (_forMessageEventQueue)
		return eventQueue.Count > 0
			? eventQueue.Dequeue ()
				: null;
	}

	//需要url判断数据是否完整...
	public delegate void DelegateSpriteCallMethod(Sprite sprite, uint textIndex);
	private Dictionary<string, List<Action<ImageLoadEvent>>> handlers;
	//action = public void demo1(ImageLoadEvent x)..
	public void AddSpriteLoadListener(string imageurl, Action<ImageLoadEvent> action)
	{
		if (!handlers.ContainsKey (imageurl))
			handlers.Add (imageurl, new List<Action<ImageLoadEvent>>());

		handlers [imageurl].Add (action);
	}
	/*
	private ImageLoadEvent _lastMessageEventArgs;
	public ImageLoadEvent MessageRepsonseEvent
	{
		get { return _lastMessageEventArgs; }
	}*/

	public void Clear()
	{
		dicSpriteList.Clear ();
	}
	public void Dispose()
	{
		eventQueue.Clear ();
		//_lastMessageEventArgs = null;
	}
	public void ReleaseAll()
	{
		Clear ();
		Dispose ();
		handlers.Clear ();
	}

	// Use this for initialization
	void Start () {
		//dicSpriteList = new Dictionary<string, Sprite> ();
		eventQueue = new Queue<ImageLoadEvent> ();
		handlers = new Dictionary<string, List<Action<ImageLoadEvent>>> ();
	}
	
	// Update is called once per frame
	void Update () {
		//异步统一接口,....
		try {
			var e = dequeueFromMessageEventQueue ();
			if ( e != null )
				EmitEvent (e);
		}
		catch (Exception ex) {
			//acceptException (ex, "An exception has occurred while OnMessage.");
			Debug.LogError (ex.ToString() + " An exception has occurred while OnMessage."); 
		}
	}

	private void EmitEvent(ImageLoadEvent ev)
	{
		if (!handlers.ContainsKey(ev.name)) { return; }
		foreach (Action<ImageLoadEvent> handler in this.handlers[ev.name]) {
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

	/// <summary>
	/// 以WWW方式进行加载,add cache..
	/// </summary>
	public void StartSpriteLoad(string imageurl, uint textIndex, DelegateSpriteCallMethod callbackDelegate)
	{
		if (dicSpriteList.ContainsKey (imageurl)) {
			if (callbackDelegate != null) {
				callbackDelegate (dicSpriteList[imageurl], textIndex);
			} else {
				ImageLoadEvent messageEventArgs = new ImageLoadEvent (imageurl, dicSpriteList[imageurl]);
				enqueueToMessageEventQueue (messageEventArgs);
			}
			return;
		}
			
		StartCoroutine( Load(imageurl, textIndex, callbackDelegate) );
	}

	/// <summary>
	/// Load the specified imageurl, textIndex and callbackDelegate.
	/// load for nocache
	/// </summary>
	/// <param name="imageurl">Imageurl.</param>
	/// <param name="textIndex">Text index.</param>
	/// <param name="callbackDelegate">Callback delegate.</param>
	public IEnumerator Load(string imageurl, uint textIndex, DelegateSpriteCallMethod callbackDelegate)
	{
		double startTime = (double)Time.time;
		//请求WWW
		WWW www = new WWW(imageurl);
		yield return www;        
		if(www != null && string.IsNullOrEmpty(www.error) )
		{
			//最多等5s...
			int maxTimeout = 5;
			while (!www.isDone) {
				yield return new WaitForSeconds(1);
				maxTimeout--;
				if (maxTimeout <= 0)
					yield break;
			}

			//获取Texture
			Texture2D texture = www.texture;

			//创建Sprite
			Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

			//image.sprite = sprite;,for cache...
			if ( false == dicSpriteList.ContainsKey(imageurl) )
				dicSpriteList.Add(imageurl, sprite);

			//?...为啥不能正常回调啊啊啊...
			if (callbackDelegate != null) {
				callbackDelegate (sprite, textIndex);
			} else {
				ImageLoadEvent messageEventArgs = new ImageLoadEvent (imageurl, sprite);
				enqueueToMessageEventQueue (messageEventArgs);
			}

			startTime = (double)Time.time - startTime;
			Debug.Log("WWW加载用时:" + startTime);
		}
	}
}
