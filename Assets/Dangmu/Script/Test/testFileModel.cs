using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class testFileModel : MonoBehaviour {
	public Image videoImage;
	//public VideoController control;
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

		ofn.initialDir =UnityEngine.Application.dataPath;//默认路径  

		ofn.title = "Open Picture";  

		ofn.defExt = "mp4";//显示文件的类型  
		//注意 一下项目不一定要全选 但是0x00000008项不要缺少  
		ofn.flags=0x00080000|0x00001000|0x00000800|0x00000200|0x00000008;//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR  


		if(DllComdlg.GetOpenFileName( ofn ))  
		{  
			// StartCoroutine(WaitLoad(ofn.file));//加载图片到panle  
			string filename = "file:///" + Utility.StrictLinuxStyle(ofn.file);
			Debug.Log( "Selected file with full path========: " + filename );
			ofn.file = filename;

			FileItem item = new FileItem ();
			item._fileInfo = new OpenFileNameEx(ofn);
			item._fileType = FileItem.FileType.FileType_Video;
			//FileListControl.Instance ().AddFile (item);

			//file path must convert for url path...
			//VideoPreviewer.Instance().StartPreview  ("file:///C:/Unity3d/dangmu/dangmu/Assets/Dangmu/Textures/WeChat_20180305193437.mp4", PreViewDone);
			VideoPreviewer.Instance().StartPreview  (filename, PreViewDone);
			//StartCoroutine ( WaitLoadTexture(ofn.file) );
		}
	}  

	public void PreViewDone( VideoPreviewer.PreViewEvent e )
	{
		//previewImage.sprite = e.PreViewSprite;
		Debug.Log ("PreViewDone: =========" + e.filePath);
		//add sprite...
		//Get file name..
		videoImage.sprite = e.PreViewSprite;
		//FileListControl.Instance ().CreatePreviewSprite (e.filePath, e.PreViewSprite);
	}  

	//test file position
	public void TestOperation() {
		string file1 = "file:///C:/Unity3d/dangmu/dangmu/Assets/Dangmu/Textures/WeChat_20180305193437.mp4";
		string file2 = "file:///C:/Unity3d/dangmu/dangmu/Assets/Dangmu/Textures/Wechat_20130312.mp4";

		//
	}
}
