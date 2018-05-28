using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;
//using System.Drawing.Imaging;
using System.IO;
using System;
using System.Drawing.Imaging;

/// <summary>
/// Animated GIF drawer.
/// </summary>
public class AnimatedGifDrawer : MonoBehaviour
{
	public bool Enable = false;
	public UnityEngine.UI.Image displayOnImage;                           // UGUI上显示
	public GameObject[] displayOnObjs;                                    // 对象上显示
	public string gifAssetString;
	[SerializeField]
	private float fps = 24;                                               // 帧数    
	private List<Texture2D> tex2DList = new List<Texture2D>();            // gif拆解图集
	private float time = 0f;
	private List<Sprite> spriteList = new List<Sprite> ();
	Bitmap myBitmap;
	void Start()
	{
		//TODO: Slow load gif, so you need to load in a corotine...
		// 根据指定文件创建image对象
		System.Drawing.Image image = System.Drawing.Image.FromFile(Application.streamingAssetsPath + "/GIF/" + gifAssetString);//ColorRibbon.gif");
		Gif2Texture2D(image);
		Debug.Log (tex2DList);
		//Debug.Log (tex2DList.Count);

		foreach (Texture2D text2D in tex2DList) {
			Sprite spriteTmp = 
				Sprite.Create (text2D, new Rect (0, 0, text2D.width, text2D.height), new Vector2 (0.5f, 0.5f));
			spriteList.Add (spriteTmp);
		}
	}

	// Update is called once per frame  
	void Update()
	{
		if ( !Enable )
			return;
		
		if (tex2DList != null && tex2DList.Count > 0)
		{
			time += Time.deltaTime;
			int index = (int)( time * fps ) % tex2DList.Count;
			if (displayOnImage != null)
			{
				//displayOnImage.sprite = Sprite.Create(tex2DList[index], new Rect(0, 0, tex2DList[index].width, tex2DList[index].height), new Vector2(0.5f, 0.5f));
				displayOnImage.sprite = spriteList[index];
			}

			if (displayOnObjs.Length != 0)
			{
				for (int i = 0; i < displayOnObjs.Length; i++)
					displayOnObjs[i].GetComponent<Renderer>().material.mainTexture = tex2DList[index];
			}
		}
	}
	/// <summary>
	/// Gif转Texture2D
	/// Crash fixed...
	/// </summary>
	/// <param name="image"> System.Image</param>
	/// <returns>Texture2D集合</returns>
	private void Gif2Texture2D(System.Drawing.Image image)
	{
		//List<Texture2D> tex = new List<Texture2D>();
		if (image != null)
		{
			// 图片构成有两种形式： 1、多页(.gif)  2、多分辨率..
			// 获取image对象的dimenson数，打印结果是1。..
			Debug.Log("image对象的dimenson数:"+image.FrameDimensionsList.Length);
			// image.FrameDimensionsList[0]-->获取image对象第一个dimension的 Guid（全局唯一标识符）....
			// 根据指定的GUID创建一个提供获取图像框架维度信息的实例..
			FrameDimension frameDimension = new FrameDimension(image.FrameDimensionsList[0]);
			// 获取指定维度的帧数
			int framCount = image.GetFrameCount(frameDimension);
			// 遍历图像帧
			for (int i = 0; i < framCount; i++)
			{
				// 选择由维度和索引指定的帧（激活图像帧）;  
				image.SelectActiveFrame(frameDimension, i);
				// 创建指定大小的 Bitmap 的实例。
				Bitmap framBitmap = new Bitmap(image.Width, image.Height);
				// Test:將Bitmap轉成PNG
				// framBitmap.Save("E:/Desktop/Saved_"+i+".png", System.Drawing.Imaging.ImageFormat.Png);

				// 从指定的Image 创建新的Graphics,并在指定的位置使用原始物理大小绘制指定的 Image,将当前激活帧的图形绘制到framBitmap上; 
				// 简单点就是从 frameBitmap（里面什么都没画，是张白纸）创建一个Graphics，然后执行画画DrawImage
				// System.Drawing.Graphics.FromImage(framBitmap).DrawImage(image, Point.Empty);
				/*using (System.Drawing.Graphics newGraphics = System.Drawing.Graphics.FromImage(framBitmap))
				{
					newGraphics.DrawImage(image, Point.Empty);
				}*/
				System.Drawing.Graphics.FromImage(framBitmap).DrawImage(image, Point.Empty);

				/*
				// 创建一个指定大小的 Texture2D 的实例
				Texture2D frameTexture2D = new Texture2D(framBitmap.Width, framBitmap.Height, TextureFormat.ARGB32, true);
				// 执行Bitmap转Texture2D
				frameTexture2D.LoadImage(Bitmap2Byte(framBitmap));*/
				var frameTexture = new Texture2D(framBitmap.Width, framBitmap.Height);  
				for (int x = 0; x < framBitmap.Width; x++) {
					for (int y = 0; y < framBitmap.Height; y++) {  
						System.Drawing.Color sourceColor = framBitmap.GetPixel (x, y);  
						frameTexture.SetPixel (framBitmap.Width - 1 - x, y, new Color32 (sourceColor.R, sourceColor.G, sourceColor.B, sourceColor.A)); // for some reason, x is flipped  
					}  
				}
				frameTexture.Apply();  

				// 添加到列表中...
				tex2DList.Add(frameTexture);
				//Debug.Log (frameTexture);

				// Test:將Texture2D轉成PNG 
				// var bytes = frameTexture2D.EncodeToPNG();
				// Destroy(frameTexture2D);
				// File.WriteAllBytes("E:/Desktop/Saved_" + i + ".png", bytes);

			}
		}
	}
	/// <summary>
	/// Bitmap转Byte
	/// </summary>
	/// <param name="bitmap">Bitmap</param>
	/// <returns>byte数组</returns>
	private byte[] Bitmap2Byte(Bitmap bitmap)
	{
		using (MemoryStream stream = new MemoryStream())
		{
			// 将bitmap 以png格式保存到流中
			bitmap.Save(stream, ImageFormat.Png);
			// 创建一个字节数组，长度为流的长度
			byte[] data = new byte[stream.Length];
			// 重置指针
			stream.Seek(0, SeekOrigin.Begin);
			// 从流读取字节块存入data中
			stream.Read(data, 0, Convert.ToInt32(stream.Length));
			return data;
		}
	}
}


