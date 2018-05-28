using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System.IO;
using UnityEngine.UI;
using System;

/// <summary>
/// Video previewer.
/// Must attach to GameObject...
/// </summary>
public class VideoPreviewer : DMonoSingleton<VideoPreviewer> {
	/// <summary>
	/// The dic sprite list.Must construct before star() func...
	/// </summary>
	[SerializeField]
	protected Dictionary<string, Sprite> dicSpriteList = new Dictionary<string, Sprite> ();
	public Sprite GetSprite(string sourceurl)
	{
		if ( dicSpriteList.ContainsKey (sourceurl) )
			return dicSpriteList [sourceurl];

		return null;
	}
	public void AddSprite(string name, Sprite source)
	{
		if ( dicSpriteList.ContainsKey (name) )
			return;
		dicSpriteList.Add (name, source);
	}

	public void Clear()
	{
		dicSpriteList.Clear ();
	}

	public VideoPlayer vpPreivew;
	public int previewWidth = 500;
	public int previewHeight = 500;
	//private Texture2D videoFrameTexture;
	private RenderTexture renderTexture;

	//for test....
	//public Image imageTest; 
	void Start()
	{
		if (null == instance)
			instance = this;
		
		//if ( null == videoFrameTexture )
			//videoFrameTexture = new Texture2D(2, 2);

		if ( null == vpPreivew )
			vpPreivew = GetComponentInChildren<VideoPlayer>();
		vpPreivew.playOnAwake = false;
		vpPreivew.waitForFirstFrame = true;
		vpPreivew.sendFrameReadyEvents = true;
		vpPreivew.frameReady += OnNewFrame;
		//vpPreivew.Play();
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
			Debug.LogWarning (ex.ToString() + " An exception has occurred while OnMessage."); 
		}
	}

	int framesValue = 0;//获得视频第几帧的图片....the firstth picture...
	/// <summary>
	/// Raises the new frame event.
	/// 采集第几帧的视,default 10th frame....
	/// </summary>
	/// <param name="source">Source.</param>
	/// <param name="frameIdx">Frame index.</param>
	void OnNewFrame(VideoPlayer source, long frameIdx)
	{
		framesValue++;
		if ( framesValue == 10 ) {
			//Debug.Log ("OnNewFrame=====================");
			renderTexture = source.texture as RenderTexture;
			Texture2D videoFrameTexture = new Texture2D(renderTexture.width, renderTexture.height);
			if (videoFrameTexture.width != renderTexture.width || videoFrameTexture.height != renderTexture.height) {
				videoFrameTexture.Resize (renderTexture.width, renderTexture.height);
			}

			//To modify...
			RenderTexture.active = renderTexture;
			videoFrameTexture.ReadPixels (new Rect (0, 0, renderTexture.width, renderTexture.height), 0, 0);
			videoFrameTexture.Apply ();
			RenderTexture.active = null;
			//vpPreivew.frameReady -= OnNewFrame;
			//vpPreivew.sendFrameReadyEvents = false;

			//for test...
			ScaleTexture (videoFrameTexture, source.url);

			Sprite preViewSprite = Sprite.Create (videoFrameTexture, new Rect (0, 0, videoFrameTexture.width, videoFrameTexture.height), 
				new Vector2 (0.5f, 0.5f));
			PreViewEvent e = new PreViewEvent (source.url, preViewSprite);
			enqueueToMessageEventQueue (e);
			AddSprite (source.url, preViewSprite);
			Log.debug (this, "add sprite source:" + source.url + " preViewSprite:" + preViewSprite.GetHashCode());
			//dicSpriteList.Add (source.url, preViewSprite);
			framesValue = 0;
			//source.enabled = false;
			//Test:
			//imageTest.sprite = PreViewSprite;
			source.Stop ();
		}
	}

	//for windows....
	private string GetPreviewTextureFullpathOfVideo( string videoFullpath )
	{
		//int filesuffixPos = videoFullpath.IndexOf (".");
		//int filenamePos = videoFullpath.LastIndexOf ("/");

		return FileListModel.GetDataPath() + "/" + FileListModel.GetFileShortName( videoFullpath ) + ".png";
	}

	//TODO: add texture2d cache for system....
	#region FOR_Common_call
	//生成缩略图..and save..
	/// <summary>
	/// Scales the texture.
	/// </summary>
	/// <param name="source">Source.</param>
	/// <param name="saveFileName">Save file name.i.e.saveFileName="temp.png"</param>
	public void ScaleTexture(Texture2D source, string videoFullpath)
	{
		//Debug.Log ("ScaleTexture##" + videoFullpath);
		string picFullpath = GetPreviewTextureFullpathOfVideo( videoFullpath );//FileListModel.GetDataPath () + "/" + saveFileName;
		if ( File.Exists( picFullpath ) ) {
			return;
		}

		//Texture2D result = new Texture2D(videoFrameTexture.width, videoFrameTexture.height, TextureFormat.ARGB32, false);
		Texture2D result = new Texture2D(previewWidth, previewHeight, TextureFormat.ARGB32, false);

		for (int i = 0; i < result.height; ++i)
		{
			for (int j = 0; j < result.width; ++j)
			{
				Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
				result.SetPixel(j, i, newColor);
			}
		}
		result.Apply();
		Debug.Log (picFullpath);
		File.WriteAllBytes(picFullpath, result.EncodeToPNG());
		Debug.Log ("ScaleTexture##Save file:" + picFullpath);
	}
		
	//load from file and cache..
	public Texture2D LoadTexture(string videoFullpath)
	{
		//fileName.LastIndexOf = texture name
		string picFullpath = GetPreviewTextureFullpathOfVideo( videoFullpath );
		if ( !File.Exists( picFullpath ) ) {
			return null;
		}
		Byte[] bytes = File.ReadAllBytes (picFullpath);
		Texture2D result = new Texture2D(4, 4, TextureFormat.ARGB32, false);
		ImageConversion.LoadImage(result, bytes);
		return result;	
	}

	public class PreViewEvent {
		public string filePath;
		public Sprite PreViewSprite;

		public PreViewEvent(string filePath, Sprite data)
		{
			this.filePath = filePath;
			this.PreViewSprite = data;
		}

		public string ToString()
		{
			return string.Format("[SocketIOEvent: filePath={0}, PreViewSprite={1}]", filePath, PreViewSprite.ToString());
		}
	}
	private Queue<PreViewEvent> eventQueue = new Queue<PreViewEvent>();
	private void enqueueToMessageEventQueue (PreViewEvent e)
	{
		//lock (_forMessageEventQueue)
		//coroutine not need lock..
		eventQueue.Enqueue (e);
	}
	private PreViewEvent dequeueFromMessageEventQueue ()
	{
		//lock (_forMessageEventQueue)
		return eventQueue.Count > 0
			? eventQueue.Dequeue ()
				: null;
	}

	private Dictionary<string, List<Action<PreViewEvent>>> handlers = new Dictionary<string, List<Action<PreViewEvent>>>();
	//action = public void demo1(ImageLoadEvent x)..
	public void AddSpriteLoadListener(string filePath, Action<PreViewEvent> action)
	{
		if (!handlers.ContainsKey (filePath))
			handlers.Add (filePath, new List<Action<PreViewEvent>>());

		handlers [filePath].Add (action);
	}
	private void EmitEvent(PreViewEvent ev)
	{
		if (!handlers.ContainsKey(ev.filePath)) { return; }
		foreach (Action<PreViewEvent> handler in this.handlers[ev.filePath]) {
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

	//public delegate void PreviewHandler (PreViewEvent previewSprite);
	public void StartPreview (string filePath, Action<PreViewEvent> callback)
	{
		if ( dicSpriteList.ContainsKey (filePath) ) {
			PreViewEvent messageEventArgs = new PreViewEvent (filePath, dicSpriteList[filePath]);
			//enqueueToMessageEventQueue (messageEventArgs);
			//directly call back..
			callback.Invoke( messageEventArgs );
			return;
		}
		//vpPreivew.enabled = true;
		vpPreivew.url = filePath;
		//vpPreivew.Play ();
		vpPreivew.prepareCompleted += Prepared;
		//vpPreivew.sendFrameReadyEvents = true;
		//vpPreivew.frameReady += OnNewFrame;
		vpPreivew.Prepare();
		AddSpriteLoadListener (filePath, callback);
	}

	void Prepared( VideoPlayer vPlayer )
	{
		Log.debug (this, "VideoPreviewer###VideoPlayer Prepared Ended");
		vPlayer.Play ();
	}
	#endregion
}
