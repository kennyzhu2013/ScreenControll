using System;
using Common;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Vertical text controller.
/// Same as DangmuTextController..
/// </summary>
public class VerticalTextController: SingletonT<VerticalTextController> {
	public VerticalScreenDisplayer Displayer;

	//public CombBulletScreenDisplayer Displayer;
	public float displayGapTime = 0.25f;
	public bool  IsCycle = false; //TODO:控制是否循环显示...
	private Queue<DangmuViewDataConveter> _textQueue;
	private Dictionary<uint, DangmuViewDataConveter> _textDicCache; //index-for message reach
	public void enqueueToDataQueue (DangmuViewDataConveter eData)
	{
		//lock (_forMessageEventQueue)
		//coroutine not need lock...
		_textQueue.Enqueue (eData);
	}

	//被网络层调用处理显示...
	private DangmuViewDataConveter dequeueFromDataQueue ()
	{
		//lock (_forMessageEventQueue)
		return _textQueue.Count > 0
			? _textQueue.Dequeue ()
				: null;
	}

	//测试解析数据...
	//List<DangmuInitRsp.DangmuInitData> dataList;
	private uint counter = 0; 
	private float timeCacl;

	/// <summary>
	/// Init this instance.
	/// control start with displayer enabled....
	/// </summary>
	public void Init() {
		Displayer = GameObject.FindObjectOfType<VerticalScreenDisplayer> ();

		//TODO://must false inited
		Displayer.Enable = true;
		_textQueue = new Queue<DangmuViewDataConveter>();
		_textDicCache = new Dictionary<uint, DangmuViewDataConveter> ();
		counter = 0;
		timeCacl = 0.0f;

		//展示弹幕...
		//另外一种思维是在update中不停渲染....
		//StartCoroutine(StartDisplayBulletScreenEffect());
	}

	public void Update() {
		if ( !Displayer.Enable )
			return;

		timeCacl += Time.deltaTime;
		if (timeCacl < displayGapTime)
			return;

		timeCacl = 0.0f;

		//随机获取某一行....
		DangmuViewDataConveter dangmuData = dequeueFromDataQueue();
		if (null == dangmuData) {
			return;
		}
		_textDicCache.Add (counter, dangmuData);

		//在本类的函数中实现Sprite加载....
		//Sprite 
		//Sprite messageBody = dangmuData._content;
		TextureLoader.Instance().StartSpriteLoad(dangmuData._avatar, counter, 
			new TextureLoader.DelegateSpriteCallMethod(this.HeadCallBack) );

		if (dangmuData.IsPicture ()) {
			Log.info (this, "==============================Update: the content url is: " + dangmuData._image);
			TextureLoader.Instance().StartSpriteLoad(dangmuData._image, counter, BodyCallBack);
		}
		++counter;

		//add later...
		//Displayer.AddBullet(dangmuData.GetColoredText(), CheckShowBox(), GetDirection(), dangmuData._avatar, messageBody);
	}

	/// <summary>
	/// Checks the show box.随机文字边框，待确认实现....
	/// </summary>
	/// <returns><c>true</c>, if show box was checked, <c>false</c> otherwise.</returns>
	/*
	private bool CheckShowBox() {
		var weightDict = new Dictionary<object, float>() {
			{true, 20f},
			{false, 80f}
		};
		bool ret = (bool)Utility.RandomObjectByWeight(weightDict);
		return ret;
	}*/

	//随机从左到右或从右到左...
	private VerticalScreenTextElement.VerticalDirection GetDirection() {
		/*
var weightDict = new Dictionary<object, float>() {
	{ScrollDirection.LeftToRight, 5f},
	{ScrollDirection.RightToLeft, 80f}
};
ScrollDirection direction = (ScrollDirection)Utility.RandomObjectByWeight(weightDict);*/

		return VerticalScreenTextElement.VerticalDirection.LowerToUp;
	}

	//以下回调如果图片信息未完成或头像失败则不会显示最终结果...
	//需要判断如果是图片信息的话需要等图片加载完....
	//根据DangmuViewDataConveter的sprite成员判断图片是否都加载完...
	#region Sprite_Load_Callback
	public void HeadCallBack(Sprite headSprite, uint textIndex)
	{
		//
		DangmuViewDataConveter dangmuData = null;
		if ( false == _textDicCache.TryGetValue(textIndex, out dangmuData) ) {
			Log.error (this, "DangmuViewDataConveter not found,index: " + textIndex + " size:" + _textDicCache.Count);
			return;
		}

		dangmuData._head = headSprite;

		//must clear text after displaying the bullet...
		if (dangmuData.IsText ()) {
			Displayer.AddBullet (dangmuData.GetColoredText (), dangmuData._content, GetDirection (), headSprite, null);
			CycleText (textIndex);
		} else if (dangmuData.IsPicture () && dangmuData._body != null) { //如果body加载完则创建弹幕...
			Displayer.AddBullet (dangmuData.GetColoredText (), dangmuData._content, GetDirection (), dangmuData._head, dangmuData._body);
			CycleText (textIndex);
		} else if ( dangmuData.IsUnknown () ){
			CycleText (textIndex);
		}
	}

	/// <summary>
	/// Bodies the call back.
	/// </summary>
	/// <param name="bodySprite">Body sprite.</param>
	/// <param name="textIndex">Text index.</param>
	public void BodyCallBack(Sprite bodySprite, uint textIndex)
	{
		//
		DangmuViewDataConveter dangmuData = null;
		if ( false == _textDicCache.TryGetValue(textIndex, out dangmuData) ) {
			Log.warin (this, "DangmuViewDataConveter not found,index: " + textIndex + " size:" + _textDicCache.Count);
			return;
		}

		Log.debug (this, "BodyCallBack: bodySprite: " + bodySprite);
		Log.debug (this, "BodyCallBack: textIndex: " + textIndex);
		dangmuData._body = bodySprite;
		if (dangmuData.IsPicture () && dangmuData._head != null ) {
			Displayer.AddBullet(dangmuData.GetColoredText(), dangmuData._content, GetDirection(), dangmuData._head, dangmuData._body);
			CycleText (textIndex);
		}
		//NUll to od
	}

	/// <summary>
	/// Cycles the text.
	/// </summary>
	/// <param name="textIndex">Text index.</param>
	private void CycleText( uint textIndex )
	{
		DangmuViewDataConveter dangmuData = null;
		if ( false == _textDicCache.TryGetValue(textIndex, out dangmuData) ) {
			Log.error (this, "DangmuViewDataConveter not found,index: " + textIndex + " size:" + _textDicCache.Count);
			return;
		}
		if ( !IsCycle ) {
			_textDicCache.Remove (textIndex);
			return;
		}

		//回收...
		enqueueToDataQueue ( dangmuData );
		_textDicCache.Remove (textIndex);
	}

	#endregion

}
