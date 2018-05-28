using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// File list control.
/// For File List...
/// </summary>
public class FileListControl : DMonoSingleton<FileListControl>
{
	//must set...
	[Header("视频素材预制")]
	public GameObject VideoViewPrefab;

	[Header("网页素材预制")]
	public GameObject WebViewPrefab;

	[Header("素材布局组件")]
	public GridLayoutGroup ScreenRoot;

	[Header("预留预制")]
	public GameObject[] objStayed;

	private Dictionary<string, FileListView> viewDic = new Dictionary<string, FileListView>();
	public void AddFileListView(string name, FileListView source)
	{
		if (viewDic.ContainsKey (name)) {
			Log.error (this, "AddFileListView#######index already exists!");
			return;
		}

		viewDic.Add (name, source);
	}
	public FileListView GetFileListView(string name)
	{
		if ( viewDic.ContainsKey (name) ) {
			return viewDic[name];
		}

		return null;
	}

	private FileListModel fileModel;
	public void AddFile( FileItem item ) {
		fileModel.Add (item);
	}

	/// <summary>
	/// The dic video sprite list to display.
	/// </summary>
	private Dictionary<string, Sprite> dicVideoSpriteList = new Dictionary<string, Sprite>();
	public Sprite GetPreviewSprite(string name)
	{
		if ( dicVideoSpriteList.ContainsKey (name) )
			return dicVideoSpriteList [name];

		return null;
	}
	public void AddPreViewSprite(string name, Sprite source)
	{
		if (dicVideoSpriteList.ContainsKey (name)) {
			Log.error (this, "AddPreViewSprite#######index already exists!");
			return;
		}
			
		dicVideoSpriteList.Add (name, source);
	}

	/// <summary>
	/// The dic video sprite list to display.
	/// </summary>
	private Dictionary<string, Sprite> dicWebSpriteList = new Dictionary<string, Sprite>();
	public Sprite GetPreviewWeb(string name)
	{
		if ( dicWebSpriteList.ContainsKey (name) )
			return dicWebSpriteList [name];

		return null;
	}
	public void AddPreViewWeb(string name, Sprite source)
	{
		if (dicWebSpriteList.ContainsKey (name)) {
			Log.error (this, "AddPreViewSprite#######index already exists!");
			return;
		}

		dicWebSpriteList.Add (name, source);
	}

	//private FileItem.FileType selectedType;
	public FileItem.FileType SelectedType {
		get;
		set;
	}

	void Start()
	{
		if (null == instance)
			instance = this;

		//
		fileModel = new FileListModel();
		fileModel.InitFileList ();

		//
		StartCoroutine( "InitDisPlayedSprite" );
	}

	/// <summary>
	/// Inits the dis played sprite.
	/// To display sprite list in the dicVideoSpriteList..
	/// </summary>
	public void InitDisPlayedSprite()
	{
		//init video sprite default...
		foreach (var item in dicVideoSpriteList) {
			//string name = System.Convert.ToString(item.Key);
			FileItem file = fileModel.GetFileInfo( item.Key );
			FileListView instance = FileListView.Create(item.Key, file._title, item.Value);
			instance.fileListModel = file;
			AddFileListView (item.Key, instance);
			//viewDic.Add (item.Key, instance);
		}

		//init web sprite preview...
		foreach (var item in dicWebSpriteList) {
			//string name = System.Convert.ToString(item.Key);
			FileItem file = fileModel.GetFileInfo( item.Key );
			FileListView instance = FileListView.Create(item.Key, file._title, item.Value);
			instance.fileListModel = file;
			AddFileListView (item.Key, instance);
			//viewDic.Add (item.Key, instance);
		}

		RefreshFileList ();
	}

	/// <summary>
	/// Inits the dis played sprite.
	/// To display sprite list in the dicVideoSpriteList..
	/// </summary>
	public void FreshPreViewByType(List<FileItem> itemList)
	{
		//init video sprite first..
		foreach (var item in itemList) {
			//string name = System.Convert.ToString(item.Key);
			string shortname = FileListModel.GetShortName( item );
			FileListView instance = FileListView.Create(name, item._title, dicVideoSpriteList[shortname]);
			instance.fileListModel = item;
			AddFileListView (shortname, instance);
			//viewDic.Add (shortname, instance);
		}

		//init web sprite preview...
	}

	//使用方式: btn.onClick.AddListener (delegate { ClickRemovePreview(name); });...
	public void ClickRemovePreview(string key)
	{
		if ( viewDic.ContainsKey (key) ) {
			FileListView view = viewDic [key];
			viewDic.Remove (key);
			DestroyObject (view.gameObject);
		}

		if ( dicVideoSpriteList.ContainsKey (key) ) {
			//Sprite sprite = dicVideoSpriteList [key];
			dicVideoSpriteList.Remove (key);
		}

		if ( dicWebSpriteList.ContainsKey (key) ) {
			//Sprite sprite = dicVideoSpriteList [key];
			dicWebSpriteList.Remove (key);
		}

		fileModel.RemoveByKey (key);
		fileModel.Save ();
		RefreshFileList ();
	}

	/// <summary>
	/// Switchs the type of the preview list by.
	/// </summary>
	public void SwitchPreviewListByType(FileItem.FileType filetype)
	{
		int childcount = ScreenRoot.transform.childCount;
		List<Transform> transList = new List<Transform> ();
		for (int i = 0; i < childcount; ++i) {
			Transform trans = ScreenRoot.transform.GetChild (i);
			transList.Add (trans);
		}

		foreach (Transform t in transList) {
			Destroy ( t.gameObject );
		}
		transList.Clear ();

		//Add new List Type...
		List<FileItem> itemList = GetFileListByType( filetype );
		FreshPreViewByType (itemList);

		RefreshFileList ();
	}

	//not cache yet...
	public List<FileItem> GetFileListByType(FileItem.FileType filetype)
	{
		List<FileItem> result = new List<FileItem> ();
		foreach (var item in fileModel.FileDicInfo) {
			if (item._fileType == filetype) {
				result.Add (item);
			}
		}
		return result;
	}

	/// <summary>
	/// Creates the preview sprite for video.
	/// </summary>
	/// <param name="filePath">File path.</param>
	/// <param name="sprite">Sprite.</param>
	public void CreatePreviewSprite(string filePath, Sprite sprite)
	{
		string filename = FileListModel.GetFileShortName (filePath);
		FileItem file = fileModel.GetFileInfo (filename);
		Debug.Log ("CreatePreviewSprite: name:" + filename + "###file:" + file + "###fileinfo.file:" + file._fileInfo.file);
		//dicVideoSpriteList.Add (filename, sprite);
		AddPreViewSprite (filename, sprite);
		FileListView instance = FileListView.Create(filename, file._title, sprite);
		instance.fileListModel = file;
		AddFileListView(filename, instance);
		//viewDic.Add (filename, instance);
		fileModel.Save ();
		RefreshFileList ();
	}

	/// <summary>
	/// Creates the preview web for web.
	/// </summary>
	/// <param name="strUrl">String URL.</param>
	/// <param name="sprite">Sprite.</param>
	public void CreatePreviewWeb(string strUrl, Sprite sprite)
	{
		string key = FileListModel.GetUrlShortName (strUrl);
		FileItem file = fileModel.GetFileInfo (key);
		Debug.Log ("CreatePreviewWeb: name:" + strUrl + "###file:" + file + "###weburl:" + file._webUrl);
		//dicWebSpriteList.Add ();
		AddPreViewWeb (key, sprite);
		FileListView instance = FileListView.Create(key, file._title, sprite);
		instance.fileListModel = file;
		Debug.Log ( "CreatePreviewWeb:GameObjectName: " + instance.gameObject.name + "_model._webUrl:"+ instance.fileListModel._webUrl 
			+ ":_model._filetype:" + instance.fileListModel._fileType);
		AddFileListView(key, instance);
		//viewDic.Add (key, instance);
		fileModel.Save ();
		RefreshFileList ();
	}

	//refresh number of file lists.
	public void RefreshFileList() {
		if ( viewDic.Count > 0 ) {
			int remainHide = viewDic.Count >= objStayed.Length ? objStayed.Length : (objStayed.Length - viewDic.Count);
			int counter = 0;
			foreach (var temp in objStayed) {
				if (counter < remainHide) {
					temp.SetActive (false);
				} else {
					temp.SetActive (true);
				}

				++counter;
			}
		}

		//
		int indexTemp = 1;
		foreach (var temp in viewDic) {
			FileListView viewtemp = temp.Value;
			viewtemp.SetViewNunmber ( indexTemp );
			++indexTemp;
		}
	}

	public Transform GetTempRoot()
	{
		return ScreenRoot.transform;
	}
}
