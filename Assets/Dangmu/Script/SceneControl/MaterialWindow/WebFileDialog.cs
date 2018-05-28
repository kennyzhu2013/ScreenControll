using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class WebFileDialog  {
	[Header("网页地址")]
	public InputField url;

	[Header("素材标题")]
	public InputField title;

	[HideInInspector]
	public FileMaterialDiaLog merialDlg;

	//filter字段: Video: mov, .mpg, .mpeg, .mp4,.avi, .asf, wmv, webm格式...
	//https://baike.baidu.com/item/OPENFILENAME/1166193?fr=aladdin...
	public void WebUrlConfirm()
	{
		// StartCoroutine(WaitLoad(ofn.file));//加载图片到panle ..
		string strurl = url.text;
		string strtitle = title.text;
		//string filename = "file:///" + Utility.StrictLinuxStyle(ofn.file);
		Debug.Log( "Selected file with full path: " + strurl );
		//ofn.file = filename;

		//must not duplicate...
		string shortUrl = FileListModel.GetUrlShortName (strurl);
		if (FileListControl.Instance ().GetFileListView (shortUrl) != null)
			return;

		FileItem item = new FileItem ();
		item._fileInfo = null;
		item._title = strtitle;
		item._webUrl = strurl;
		item._fileType = FileItem.FileType.FileType_Web;
		FileListControl.Instance ().AddFile (item);

		//file path must convert for url path...
		//VideoPreviewer.Instance().StartPreview  ("file:///C:/Unity3d/dangmu/dangmu/Assets/Dangmu/Textures/WeChat_20180305193437.mp4", PreViewDone);
		WebPreviewer.Instance().StartPreview  (strurl, PreViewDone);
		//StartCoroutine ( WaitLoadTexture(ofn.file) );

		merialDlg.Hide ();
	}

	public void PreViewDone( WebPreviewer.PreViewEvent e )
	{
		//previewImage.sprite = e.PreViewSprite;
		Log.debug (this, "PreViewDone: =========" + e.webUrl);
		//add sprite...
		//Get file name..
		//videoImage.sprite = e.PreViewSprite;
		FileListControl.Instance ().CreatePreviewWeb (e.webUrl, e.PreViewSprite);
	}
}
