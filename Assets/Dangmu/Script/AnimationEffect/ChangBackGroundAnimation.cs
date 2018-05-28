using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangBackGroundAnimation : MonoBehaviour {

	public List<Sprite> _Pics = new List<Sprite>();                             //替换的图片，是一个图片组  
	private Image _SourceImage;                                                 //需要被替换的目标图片  
	public bool _IsReverse = false;                                             //是否反转，就是看需不需要出现从头到尾，从尾到头的效果  
	public bool _IsAutoPlay = true;                                             //自动播放吗?  
	public float _DelayTime = 0;                                                //延时播放时间  
	public float _DeltaTime=0.1f;                                               //照片切换间隔  
	private int _PicIndex=0;                                                    //照片的编码，就是下标  
	private int _PicLength;                                                     //照片组长度  


	void Start()  
	{  
		_SourceImage = this.GetComponent<Image>();                              //目标图片  
		_PicLength = _Pics.Count;                                               //图片的长度进行存储  
		if (_IsReverse)                                                         //如果需要反转  
		{  
			for (int i = _PicLength - 1; i >= 0; i--)                           //按照目前的长度，反向存一遍  
			{  
				_Pics.Add(_Pics[i]);  
			}  
			_PicLength = _Pics.Count;                                           //重新设置长度  
		}  
		if (_IsAutoPlay)                                                         //是自动播放吗？  
			InvokeRepeating("ChangeBackGround", _DelayTime, _DeltaTime);         //调用切换代码，输入延时时间与间隔时间  
	}

	/// <summary>  
	/// 修改背景方法  
	/// </summary>  
	void ChangeBackGround()  
	{  
		if (_PicIndex >= _PicLength)                                                //如果下标溢出  
			_PicIndex = 0;                                                          //下标清零  
		_SourceImage.sprite = _Pics[_PicIndex];                                     //修改背景  
		_PicIndex++;                                                                //下标自加1  
	}  
}
