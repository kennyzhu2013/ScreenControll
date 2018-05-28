using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testWebPreview : MonoBehaviour {
	public Image previewImage;
	public Shader outShader;

	// Use this for initialization
	void Start () {
		
		//TestLoadPicture ();
		WebPreviewer.Instance().StartPreview ("www.baidu.com", PreViewDone);
		//VideoPreviewer.Instance().StartPreview ("file://C:\\Unity3d\\dangmu\\dangmu\\Assets\\Dangmu\\Textures\\WeChat_20180305193437.mp4", PreViewDone);
	}

	// Update is called once per frame
	void Update () {

	}

	public void PreViewDone( WebPreviewer.PreViewEvent e )
	{
		previewImage.sprite = e.PreViewSprite;
		Debug.Log (e.webUrl);
	}
}
