using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

//PlayerPrefs to json?...
using Pathfinding.Serialization.JsonFx;
using System.IO;
using System.Security.Cryptography;

[System.Serializable]
public class FileList {
	//public Dictionary<string, FileItem> dicFileItems;
	public List<FileItem> lstFileItems;
	public FileList(){
		lstFileItems = new List<FileItem>();
	}
}

/// <summary>
/// File list model.
/// Init after VideoPreviewer inited
/// </summary>
public class FileListModel {
	private FileList dicFileItems = new FileList ();
	public List<FileItem> FileDicInfo {
		get { return dicFileItems.lstFileItems; }
	}

	//private int countIndex;
	public FileListModel()
	{
		//countIndex = 0;
		InitFileList ();
	}

	/// <summary>
	/// Inits the file list and init textures preview...
	/// </summary>
	public void InitFileList()
	{
		//
		DeserializeAndRead();

		//get texture...
		if (null == dicFileItems)
			dicFileItems = new FileList ();
		List<FileItem> dicFileList = dicFileItems.lstFileItems;
		List<FileItem> deleteList = new List<FileItem>();
		foreach (var item in dicFileList) {
			//if ( countIndex < item.Key )
				//countIndex = item.Key;
			
			//FileItem file 
			if ( item._fileType == FileItem.FileType.FileType_Video ) {
				//string filePath = "file://" + item.Value.
				OpenFileNameEx fileinfo = item._fileInfo;
				string filePath = "file:///" + fileinfo.file;
				Texture2D texture = VideoPreviewer.Instance().LoadTexture ( filePath );
				if (texture == null) {
					deleteList.Add (item);
					continue;
				}

				Sprite sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), new Vector2 (0.5f, 0.5f));
				string key = GetFileShortName (filePath);
				FileListControl.Instance ().AddPreViewSprite (key, sprite);
			}
			else if ( item._fileType == FileItem.FileType.FileType_Web )
			{
				string weburl = item._webUrl;
				Texture2D texture = WebPreviewer.Instance().LoadTexture ( weburl );
				if (texture == null) {
					deleteList.Add (item);
					continue;
				}
					
				Sprite sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), new Vector2 (0.5f, 0.5f));
				string key = GetUrlShortName ( weburl );
				FileListControl.Instance ().AddPreViewWeb (key, sprite);
			}

		}

		foreach (var itemDel in deleteList) {
			dicFileList.Remove (itemDel);
		}

		Save ();
	}

	//remove by 
	public void RemoveByKey(string url)
	{
		foreach (var item in dicFileItems.lstFileItems) {
			if (item._fileType == FileItem.FileType.FileType_Video) {
				string key = GetFileShortName (item._fileInfo.file);
				if (key == url) {
					dicFileItems.lstFileItems.Remove (item);
					return;
				}
			}
			else if (item._fileType == FileItem.FileType.FileType_Web) {
				string key = GetUrlShortName (item._webUrl);
				if (key == url) {
					dicFileItems.lstFileItems.Remove (item);
					return;
				}
			}
		}

		//dicFileItems.dicFileItems.Remove (url);
	}

	/// <summary>
	/// Add the specified item.
	/// Title can be gotten from file name.
	/// </summary>
	/// <param name="item">Item.</param>
	public void Add(FileItem item)
	{
		//++countIndex;
		if (item._title == null || item._title == "") {
			//Debug.Log (item._fileInfo.file);
			if (FileItem.FileType.FileType_Video == item._fileType) {
				item._title = GetFileShortName (item._fileInfo.file);
			}
			else if (FileItem.FileType.FileType_Web == item._fileType) {
				item._title = GetUrlShortName (item._webUrl);
			}
			//Debug.Log ("Add:name: " + item._title );
		}

		//not duplicate..
		foreach( var tempItem in dicFileItems.lstFileItems ) {
			if ( tempItem._fileType == item._fileType ) {
				if (FileItem.FileType.FileType_Video == item._fileType && tempItem._fileInfo.file == item._fileInfo.file ) {
					return;
				}
				else if (FileItem.FileType.FileType_Web == item._fileType && tempItem._webUrl == item._webUrl ) {
					return;
				}
			}
		}

		dicFileItems.lstFileItems.Add (item);
	}

	public void Save()
	{
		SerializeAndSave ();
	}

	public FileItem GetFileInfo(string name)
	{
		//FileItem result = null;
		foreach (var item in dicFileItems.lstFileItems) {
			if (item._fileType == FileItem.FileType.FileType_Video) {
				string key = GetFileShortName (item._fileInfo.file);
				if (key == name) {
					return item;
				}
			}
			else if (item._fileType == FileItem.FileType.FileType_Web) {
				string key = GetUrlShortName (item._webUrl);
				if (key == name) {
					return item;
				}
			}
		}

		Log.error (this, "GetFileInfo cannot find item, name:" + name);
		return null;
	}

	public static string GetShortName(FileItem item)
	{
		if (FileItem.FileType.FileType_Video == item._fileType)
			return GetFileShortName (item._fileInfo.file);
		else if (FileItem.FileType.FileType_Web == item._fileType)
			return GetUrlShortName (item._webUrl);

		return null;
	}

	public static string GetFileShortName(string fullpath)
	{
		Debug.Log ("GetFileShortName:" + fullpath);
		int filesuffixPos = fullpath.IndexOf (".");
		//int filenamePos = fullpath.LastIndexOf ("/") + 1;
		int filenamePos = fullpath.LastIndexOf ("\\") + 1;
		if ( filenamePos <= 0 )
			filenamePos = fullpath.LastIndexOf ("/") + 1;
		return fullpath.Substring (filenamePos, filesuffixPos - filenamePos);
	}

	public static string GetUrlShortName(string strUrl)
	{
		//Debug.Log ("GetUrlShortName:" + strUrl);
		int filenamePos = strUrl.LastIndexOf ("/") + 1;
		if (filenamePos <= 0)
			filenamePos = 0;
		string filename = strUrl.Substring (filenamePos);
		string reuslt = System.Convert.ToString ( strUrl.GetHashCode() );
		if ( filenamePos <= 0 )
			return filename + reuslt;

		int filesuffixPos = filename.IndexOf (".");
		return filename.Substring (0, filesuffixPos) + reuslt;
	}

	public FileItem GetFileInfoByFullPath(string fullpath)
	{
		//int filesuffixPos = fullpath.IndexOf (".");
		//int filenamePos = fullpath.LastIndexOf ("/");
		string name = GetFileShortName(fullpath);
		return GetFileInfo(name);
	}

	static string fileName = "/data.txt";
	public static string FileFullPath {
		get {  return GetDataPath () + fileName; }
	}

	public static string GetDataPath()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			string path = Application.dataPath.Substring (0, Application.dataPath.Length - 5);
			return path + "/Dcoument";
		} else if (Application.platform == RuntimePlatform.Android) {
			return Application.persistentDataPath + "/";
		} else {
			return Application.dataPath + "/";
		}
	}

	private void SerializeAndSave()
	{
		//把数据序列化为string
		string Json_Text = JsonWriter.Serialize(dicFileItems);

		//持久化...
		//var streamWriter = new StreamWriter( FileFullPath );
		//streamWriter.Write(Json_Text);

		//把数据写出到文件
		File.WriteAllText(FileFullPath, Json_Text);
	}

	private void DeserializeAndRead()
	{ 
		//文件是否有效
		if ( !File.Exists(FileFullPath) )
		{
			return;
		}

		//把文件读取成string
		string originText = File.ReadAllText( FileFullPath );

		//string序列化成对象
		dicFileItems = JsonReader.Deserialize<FileList>(originText);
	}
}

[System.Serializable]
public class FileItem {
	public enum FileType {
		FileType_Video,
		FileType_Web,
		FileType_Unkown
	}

	public FileType _fileType;
	public OpenFileNameEx _fileInfo;
	public string _webUrl;
	public string _title;

	/*
	public static void Write(FileItem item)
	{
		string text = JsonWriter.Serialize (item);

		File.WriteAllText (FileListModel.FileFullPath, text);
	}

	public static FileItem Read()
	{
		string str = File.ReadAllText (FileListModel.FileFullPath);

		FileItem item = JsonReader.Deserialize<FileItem> (str);

		Debug.Log (item.ToString());
		return item;
	}*/

}
