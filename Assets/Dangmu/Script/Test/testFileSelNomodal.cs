using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;  
//using System.Windows.Forms;  
using System.IO;
using UnityEngine.UI;
using System.Threading;

public class testFileSelNomodal : MonoBehaviour {
	public Image videoImage;
	public class ThreadObjectMutex {
		public string UnityFullpath;
		public bool BUnityWorkMutex;
	}
	ThreadObjectMutex objectThread = new ThreadObjectMutex();
	void Start()
	{
		objectThread.UnityFullpath = UnityEngine.Application.dataPath;
		objectThread.BUnityWorkMutex = false;
	}
	//public Texture2D img = null;  
	/*void OnGUI()  
	{  
		if (GUI.Button(new Rect(0, 0, 100, 20), "选择文件"))  
		{  
			OpenFileDialog od = new OpenFileDialog();  
			od.Title = "请选择头像图片";  
			od.Multiselect = false;  
			od.Filter = "图片文件(*.jpg,*.png,*.bmp)|*.jpg;*.png;*.bmp";  
			print("s"+ od.ShowDialog());  
			print("d"+DialogResult.OK);  
			if (od.ShowDialog() == DialogResult.OK)  
			{  
				Debug.Log(od.FileName);  
				StartCoroutine(GetTexture("file://" + od.FileName));  
			}  

		}


		if (img != null)  
		{  
			GUI.DrawTexture(new Rect(0, 20, img.width, img.height), img);  
		}
	}  */

	IEnumerator GetTexture(string url)  
	{  
		WWW www = new WWW(url);  
		yield return www;  
		if (www.isDone && www.error == null)  
		{  
			Texture2D img = www.texture; 
			videoImage.sprite =  Sprite.Create (img, new Rect (0, 0, img.width, img.height), 
				new Vector2 (0.5f, 0.5f));
			Debug.Log(img.width + "  " + img.height);  
			//byte[] data = img.EncodeToPNG();  
			//  File.WriteAllBytes(UnityEngine.Application.streamingAssetsPath + "/Temp/temp.png", data);  
		}  
	}

	//public VideoController control;
	public void VideoFileSelect()  
	{  
		Thread th1 = new Thread( ThreadSelect );    
		//StartCoroutine(CoroutineSelect());
		th1.Start();
	} 

	IEnumerator CoroutineSelect()
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

			lock (VideoPreviewer.Instance()) {
				VideoPreviewer.Instance ().StartPreview (filename, PreViewDone);
			}
			//StartCoroutine ( WaitLoadTexture(ofn.file) );
		}
		yield return 0;
	}


	public void ThreadSelect()
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
			/*lock (VideoPreviewer.Instance()) {
				//VideoPreviewer.Instance ().StartPreview (filename, PreViewDone);
			}*/
			lock ( objectThread ) {
				objectThread.BUnityWorkMutex = true;
				objectThread.UnityFullpath = filename;
			}
			//StartCoroutine ( WaitLoadTexture(ofn.file) );
		} /*
		if (od.ShowDialog() == DialogResult.OK)  
		{  
			Debug.Log(od.FileName);  
			lock (obj) {
			}
			this.StartCoroutine(GetTexture("file://" + od.FileName));  
		}*/
		Thread.CurrentThread.Abort ();
	}

	void Update()
	{
		if (objectThread.BUnityWorkMutex) {
			lock (objectThread) {
				objectThread.BUnityWorkMutex = false;
				VideoPreviewer.Instance ().StartPreview (objectThread.UnityFullpath, PreViewDone);
			}
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
}
