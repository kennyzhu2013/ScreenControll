using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;  
using System.Runtime.InteropServices;  
using System;
using UnityEngine.UI;

public class TestFileDialog : MonoBehaviour {
	public Image plane;  
	public void FileSelect()  
	{  
		OpenFileName ofn = new OpenFileName();  

		ofn.structSize = Marshal.SizeOf(ofn);  

		//三菱(*.gxw)\0*.gxw\0西门子(*.mwp)\0*.mwp\0All Files\0*.*\0\0  
		ofn.filter = "All Files\0*.png;*.jpg\0\0";  

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

			Debug.Log( "Selected file with full path: " + ofn.file );
			StartCoroutine ( WaitLoadTexture(ofn.file) );
		}
	}  

	IEnumerator WaitLoadTexture(string fileName)  
	{  
		string fullPath = "file://" + fileName;
		Debug.Log ("WaitLoadTexture:" + fullPath);
		WWW wwwTexture=new WWW(fullPath);  

		Debug.Log(wwwTexture.url);  

		yield return wwwTexture; 
		if (wwwTexture != null && string.IsNullOrEmpty (wwwTexture.error)) {
			//最多等5s...
			int maxTimeout = 5;
			while (!wwwTexture.isDone) {
				yield return new WaitForSeconds (1);
				maxTimeout--;
				if (maxTimeout <= 0)
					yield break;
			}
			//Debug.Log (wwwTexture.texture.width);
			//Debug.Log (wwwTexture.texture.height);
			Sprite sprite = Sprite.Create (wwwTexture.texture, new Rect (0, 0, wwwTexture.texture.width, wwwTexture.texture.height), 
				                new Vector2 (0.5f, 0.5f));
			plane.sprite = sprite;  
		} else {
			Debug.Log(wwwTexture.error); 
		}
	}  
}
