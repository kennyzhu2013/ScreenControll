using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System;
using System.IO;
using UnityEngine.UI;
using ZenFulcrum.EmbeddedBrowser;

/// <summary>
/// Web previewer.
/// Just cache sprites of the web rawimage...
/// TODO:...
/// </summary>
public class WebPreviewer : DMonoSingleton<WebPreviewer> {
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

	public Browser browser; //LoadURL...
	public int previewWidth = 500;
	public int previewHeight = 500;
	//private Texture2D webTexture;
	private RawImage webImage;
	//private Texture2D webFrameTexture;
	//private RenderTexture renderTexture;
	//http://blog.csdn.net/bingheliefeng/article/details/51177505..

	//for test....
	//public Image imageTest; 
	void Start()
	{
		if (null == instance)
			instance = this;

		//if ( null == webTexture )
		//webTexture = new Texture2D(2, 2);

		if ( null == browser )
			browser = GetComponentInChildren<Browser>(true);
		browser.onLoad += LoadCallback;

		if ( null == webImage )
			webImage = GetComponentInChildren<RawImage>(true);

		//vpPreivew.frameReady += OnNewFrame;
	}

	// Update is called once per frame
	void Update () {
		
	}

	//for windows....
	private string GetPreviewTextureFullpathOfWeb( string webUrl )
	{
		return FileListModel.GetDataPath() + "/" + FileListModel.GetUrlShortName( webUrl ) + ".png";;
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
		string picFullpath = GetPreviewTextureFullpathOfWeb( videoFullpath );//FileListModel.GetDataPath () + "/" + saveFileName;
		if ( File.Exists( picFullpath ) ) {
			Log.debug (this, "ScaleTexture##Exists:" + picFullpath);
			return;
		}

		//Texture2D result = new Texture2D(videoFrameTexture.width, videoFrameTexture.height, TextureFormat.ARGB32, false);
		Texture2D result = new Texture2D(source.width, source.height, TextureFormat.ARGB32, false);

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
		Log.debug (this, "ScaleTexture##Save file:" + picFullpath);
	}

	//load from file and cache..
	public Texture2D LoadTexture(string webUrl)
	{
		//fileName.LastIndexOf = texture name
		string picFullpath = GetPreviewTextureFullpathOfWeb( webUrl );
		if ( !File.Exists( picFullpath ) ) {
			return null;
		}
		Byte[] bytes = File.ReadAllBytes (picFullpath);
		Texture2D result = new Texture2D(4, 4, TextureFormat.ARGB32, false);
		ImageConversion.LoadImage(result, bytes);
		return result;	
	}

	public class PreViewEvent {
		public string webUrl;
		public Sprite PreViewSprite;

		public PreViewEvent(string webUrl, Sprite data)
		{
			this.webUrl = webUrl;
			this.PreViewSprite = data;
		}

		public string ToString()
		{
			return string.Format("[SocketIOEvent: filePath={0}, PreViewSprite={1}]", webUrl, PreViewSprite.ToString());
		}
	}


	private string previewUrl;
	private Action<PreViewEvent> previewCallback;
	//public delegate void PreviewHandler (PreViewEvent previewSprite);
	public void StartPreview (string webUrl, Action<PreViewEvent> callback)
	{
		if ( dicSpriteList.ContainsKey (webUrl) ) {
			PreViewEvent messageEventArgs = new PreViewEvent (webUrl, dicSpriteList[webUrl]);
			//enqueueToMessageEventQueue (messageEventArgs);
			//directly call back..
			callback.Invoke( messageEventArgs );
			return;
		}
		Log.debug(this, "StartPreview:" + webUrl);

		//vpPreivew.prepareCompleted += Prepared;
		//vpPreivew.frameReady += OnNewFrame;
		//vpPreivew.Prepare();
		//browser.Url = webUrl;
		browser.LoadURL (webUrl, true);
		previewUrl = webUrl;
		previewCallback = callback;
	}

	public void LoadCallback(JSONNode obj) {
		if (previewUrl == null)
			return;

		int statuscode = obj ["status"];
		if (statuscode >= 300) {
			Log.error (this, "LoadCallback==Error statuscode:" + statuscode);
			return;
		}

		//Debug.Log ("LoadCallback==============================:" + statuscode);
		Texture2D tempTexture2D = webImage.texture as Texture2D;
		Texture2D result = new Texture2D(tempTexture2D.width, tempTexture2D.height, TextureFormat.ARGB32, false);

		for (int i = 0; i < result.height; ++i)
		{
			for (int j = 0; j < result.width; ++j)
			{
				Color newColor = tempTexture2D.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
				result.SetPixel(j, i, newColor);
			}
		}
		result.Apply();

		//webFrameTexture.UpdateExternalTexture( tmpTexture.GetNativeTexturePtr() );
		ScaleTexture (result, previewUrl);

		Sprite preViewSprite = Sprite.Create (result, new Rect (0, 0, result.width, result.height), 
			new Vector2 (0.5f, 0.5f));
		PreViewEvent e = new PreViewEvent (previewUrl, preViewSprite);
		previewCallback.Invoke( e );
		AddSprite (previewUrl, preViewSprite);
		//dicSpriteList.Add (webUrl, preViewSprite);
	}

	#endregion
}
