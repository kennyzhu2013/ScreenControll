using System;
using UnityEngine;
using UnityEngine.UI;
using Common;

/// <summary>
/// File list view.
/// For test....
/// </summary>
public class FileListView : BaseView {

	//for test...
	//public Image[] images;

	[HideInInspector]
	public string title;
	public int number;

	//public Transform textObj;
	private Text textTitle;
	private Text textNumber;

	[HideInInspector]
	public int index;

	[HideInInspector]
	public Sprite preview;
	private Image previewImage;

	[HideInInspector]
	public FileItem fileListModel;

	//private Button btnRmv;
	public override void Start () {
		base.Start ();
		base.isTweenWin = false;

		if (null == previewImage) {
			previewImage = transform.Find("Image").GetComponent<Image>();
			if (preview != null) {
				previewImage.sprite = preview;
				Log.debug (this, "previewImage.sprite========:" + previewImage.sprite.name);
			}
		}

		if (null == textTitle) {
			//textObj = ;
			//Debug.Log ("=============:" + textObj);
			Text[] texts = transform.Find ("TitleBanner").GetComponentsInChildren<Text>(true);
			//textTitle = transform.Find ("TitleBanner").GetComponentInChildren<Text>(true);
			foreach (Text temp in texts) {
				if ("Name" == temp.name) {
					textTitle = temp;
					textTitle.text = title;
				}
				else if ("Number" == temp.name) {
					textNumber = temp;
					textNumber.text = System.Convert.ToString(number);
				}
				//
			}
		}

		//添加button事件...
		Button[] btnList = gameObject.GetComponentsInChildren<Button>();
		foreach (Button btn in btnList) {
			btn.onClick.AddListener (delegate() {
				this.OnClick( btn.gameObject );
			});
		}

		//btnRmv.onClick.AddListener (delegate { ClickRemovePreview(name); });
	}

	public void SetViewNunmber(int iIndex) {
		number = iIndex;
		if ( textNumber != null )
			textNumber.text = System.Convert.ToString(number);
	}

	//init to set ..
	/*
	public void SetSprite(int index, Sprite previewSprite)
	{
		this.index = index;
		previewImage.sprite = previewSprite;
	}*/

	public void OnClick(GameObject objSender)
	{
		switch (objSender.name) {
		case "PreviewBtn": //Open Material Window..
			if (fileListModel._fileType == FileItem.FileType.FileType_Video) {
				SCMainWinController.Instance ().TopLeftVw.DisplayVideo (fileListModel._fileInfo.file, title);
			}
			else if ( fileListModel._fileType == FileItem.FileType.FileType_Web )
				SCMainWinController.Instance ().TopLeftVw.DisplayWeb( fileListModel._webUrl, title);
			break;
		case "ProjectBtn": //..
			if ( fileListModel._fileType == FileItem.FileType.FileType_Video )
				SCMainWinController.Instance ().TopRhtVw.DisplayVideo( fileListModel._fileInfo.file, title, 
															SCMainWinController.Instance ().TopLeftVw.GetVideoFrameCount() );
			else if ( fileListModel._fileType == FileItem.FileType.FileType_Web )
				SCMainWinController.Instance ().TopRhtVw.DisplayWeb( fileListModel._webUrl, title);
			break;
		case "RemoveBtn": //移除...
			string key = "";
			if (fileListModel._fileType == FileItem.FileType.FileType_Video)
				key = FileListModel.GetFileShortName (fileListModel._fileInfo.file);
			else if (fileListModel._fileType == FileItem.FileType.FileType_Web)
				key = FileListModel.GetUrlShortName (fileListModel._webUrl);
			FileListControl.Instance ().ClickRemovePreview (key);
			break;
		default:
			break;
		}
	}

	//
	public static FileListView Create( string name, string title, Sprite sprite,  FileItem.FileType filetype = FileItem.FileType.FileType_Video )
	{
		if (sprite == null) {
			Debug.Log("FileListView.Create(), sprite can not be null !");
			return null;
		}

		GameObject go;
		if (FileItem.FileType.FileType_Video == filetype)
			go = Instantiate (FileListControl.Instance ().VideoViewPrefab) as GameObject;
		else if (FileItem.FileType.FileType_Web == filetype)
			go = Instantiate (FileListControl.Instance ().WebViewPrefab) as GameObject;
		else {
			Log.error ("FileListView", "Unknown file type:" + filetype);
			return null;
		}

		go.transform.SetParent(FileListControl.Instance().GetTempRoot());
		go.transform.localPosition = new Vector3 (go.transform.localPosition.x, go.transform.localPosition.y, 0);
		go.transform.localScale = Vector3.one;
		go.name = name;

		//set title and sprite
		//RectTransform rect = go.GetComponent<RectTransform> ();
		//rect.rect.height = 100;
		//rect.rect.width = 100;
		FileListView instance = go.GetComponent<FileListView>();
		if (null == instance)
			instance = go.AddComponent<FileListView>();
		instance.title = title;
		instance.preview = sprite;
		instance.fileListModel = null;
		return instance;
	}
}
