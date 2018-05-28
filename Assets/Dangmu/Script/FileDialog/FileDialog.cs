using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Threading;

/// <summary>
/// File dialog. For local file selection....
/// </summary>
public class FileDialog {
	public FileMaterialDiaLog merialDlg;
	public class ThreadObjectMutex {
		public string UnityFullpath;
		public bool BUnityWorkMutex;
	}
	public ThreadObjectMutex objectThread = new ThreadObjectMutex();

	//filter字段: Video: mov, .mpg, .mpeg, .mp4,.avi, .asf, wmv, webm格式...
	//https://baike.baidu.com/item/OPENFILENAME/1166193?fr=aladdin...
	public void VideoFileSelect()  
	{  
		OpenFileName ofn = new OpenFileName();  

		ofn.structSize = Marshal.SizeOf(ofn);  

		//三菱(*.gxw)\0*.gxw\0西门子(*.mwp)\0*.mwp\0All Files\0*.*\0\0  
		ofn.filter = "All Files\0*.mp4;*.mpg\0\0";  

		ofn.file = new string(new char[256]);  

		ofn.maxFile = ofn.file.Length;  

		ofn.fileTitle = new string(new char[64]);  

		ofn.maxFileTitle = ofn.fileTitle.Length;  

		ofn.initialDir = objectThread.UnityFullpath;//默认路径  

		ofn.title = "Open Video";  

		ofn.defExt = "mp4";//显示文件的类型  
		//注意 一下项目不一定要全选 但是0x00000008项不要缺少  
		ofn.flags=0x00080000|0x00001000|0x00000800|0x00000200|0x00000008;//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR  

		if(DllComdlg.GetOpenFileName( ofn ))  
		{  
			// StartCoroutine(WaitLoad(ofn.file));//加载图片到panle  
			//string filename = "file:///" + Utility.StrictLinuxStyle(ofn.file);
			string filename = Utility.StrictLinuxStyle(ofn.file);
			ofn.file = filename;

			//must not duplicate...
			string shortName = FileListModel.GetFileShortName (filename);
			//Debug.Log( "Selected file with full path: " + filename + " : shortName:" + shortName);
			if (FileListControl.Instance ().GetFileListView (shortName) != null)
				return;
			
			FileItem item = new FileItem ();
			item._fileInfo = new OpenFileNameEx(ofn);
			item._fileType = FileItem.FileType.FileType_Video;

			lock ( objectThread ) {
				FileListControl.Instance ().AddFile (item);
				objectThread.BUnityWorkMutex = true;
				objectThread.UnityFullpath = filename;
			}
			//file path must convert for url path...
			//VideoPreviewer.Instance().StartPreview  ("file:///C:/Unity3d/dangmu/dangmu/Assets/Dangmu/Textures/WeChat_20180305193437.mp4", PreViewDone);
			//StartCoroutine ( WaitLoadTexture(ofn.file) );
			/*VideoPreviewer.Instance().StartPreview  (filename, PreViewDone);
			merialDlg.Hide ();*/
		}
		Thread.CurrentThread.Abort ();
	}  

	public void PreViewDone( VideoPreviewer.PreViewEvent e )
	{
		//previewImage.sprite = e.PreViewSprite;
		Log.debug (this, "PreViewDone: =========" + e.filePath);
		//add sprite...
		//Get file name..
		//videoImage.sprite = e.PreViewSprite;
		FileListControl.Instance ().CreatePreviewSprite (e.filePath, e.PreViewSprite);
	}

}
