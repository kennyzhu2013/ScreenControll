using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testVideoprivew : MonoBehaviour {
	public Image previewImage;

	// Use this for initialization
	void Start () {
		//TestLoadPicture ();
		VideoPreviewer.Instance().StartPreview ("file:///C:/Unity3d/dangmu/dangmu/Assets/Dangmu/Textures/WeChat_20180305193437.mp4", PreViewDone);
		//VideoPreviewer.Instance().StartPreview ("file://C:\\Unity3d\\dangmu\\dangmu\\Assets\\Dangmu\\Textures\\WeChat_20180305193437.mp4", PreViewDone);
	}

	// Update is called once per frame
	void Update () {

	}

	public void PreViewDone( VideoPreviewer.PreViewEvent e )
	{
		previewImage.sprite = e.PreViewSprite;
		Debug.Log (e.filePath);
	}

	public void TestLoadPicture()
	{
		Texture2D texture = VideoPreviewer.Instance().LoadTexture ("file:///C:/Unity3d/dangmu/dangmu/Assets/Dangmu/Textures/WeChat_20180305193437.mp4");
		Sprite preViewSprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), 
			new Vector2 (0.5f, 0.5f));
		previewImage.sprite = preViewSprite;

		Debug.Log ("TestLoadPicture##successed:" + preViewSprite.name);
	}
}
