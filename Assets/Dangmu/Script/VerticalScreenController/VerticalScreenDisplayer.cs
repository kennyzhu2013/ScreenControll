using System;
using Common;
using UnityEngine;
using BulletScreen;

/// <summary>
/// Vertical screen displayer.
/// Same as CombBulletScreenDisplayer..
/// </summary>
public class VerticalScreenDisplayer : BaseView {

	#if !NOT_CODE_ING
	public bool Enable { get; set; }

	/**隐藏界面**/
	public override void Hide()
	{
		Enable = false;
		currentState = ViewState.Close;
	}

	/**在界面隐藏的情况下 显示界面**/
	public override void Show()
	{
		Enable = true;
		currentState = ViewState.Open;
	}

	/// <summary>
	/// The curr bullet text info list.
	/// 只记录要显示的下一条或当前的这条BulletText，全局缓存放到controller里...
	/// </summary>
	public VerticalScreenTextElement _currBulletTextElement; 
	[SerializeField]private BulletScreenDisplayerModel _info; //for model...
	public float ScrollDuration {
		get { return _info.ScrollDuration; }
	}

	//private float _bulletScreenWidth;
	//for GetAheadTime and animation, set screen height..
	private float _bulletScreenHeight;
	public float BulletScreenHeight {
		get { return _bulletScreenHeight; }
	}

	//vertical prefab...
	public GameObject VerticalTextElementPrefab {
		get { return _info.TextPrefab; }
	}

	//文字边框....
	public string TextBoxNodeName {
		get { return _info.TextBoxNodeName; }
	}

	//delay to destory...
	public float KillBulletTextDelay {
		get { return _info.KillBulletTextDelay; }
	}

	//GameObject as screen root node..
	public Transform ScreenRoot {
		get { return _info.ScreenRoot.transform; }
	}

	//
	private const float ROW_WIDTH = 1000f;

	/// <summary>
	/// Create the specified displayerInfo.not used yet..
	/// </summary>
	/// <param name="displayerInfo">Displayer info.</param>
	public static VerticalScreenDisplayer Create(BulletScreenDisplayerModel displayerInfo) {
		VerticalScreenDisplayer instance = displayerInfo.Owner.gameObject.AddComponent<VerticalScreenDisplayer>();
		instance._info = displayerInfo;
		return instance;
	}

	/// <summary>
	/// Adds the bullet, then it will display the bullet screen effect..
	/// </summary>
	/// <param name="textContent">Text content.</param>
	/// <param name="showBox">If set to <c>true</c> show box.</param>
	/// <param name="direction">Direction.</param>
	public void AddBullet(string name, string content,
		VerticalScreenTextElement.VerticalDirection direction = VerticalScreenTextElement.VerticalDirection.LowerToUp, 
		Sprite head = null,  Sprite photo = null) {
		VerticalScreenTextElement.Create(this, name, content, direction, head, photo);
	}

	public override void Start() {
		base.Start ();
		SetScrollScreen();
		InitRow();
	}

	/// <summary>
	/// Inits the row.
	/// </summary>
	private void InitRow() {
		Utility.DestroyAllChildren(_info.ScreenRoot.gameObject);

		//显示的函数列表..总共有TotalRowCount行显示信息...
		/*
		_currBulletTextInfoList = new CombBulletTextInfo[_info.TotalRowCount];
		for (int rowIndex = 0; rowIndex < _info.TotalRowCount; rowIndex++) {
			_currBulletTextInfoList[rowIndex] = null;
		}*/
		_currBulletTextElement = null;

		//节点名字....设置节点的父亲节点为_info的ScreenRoot节点...
		//only need one column
		string rowNodeName = string.Format("column_{0}", 0);
		GameObject newRow = new GameObject(rowNodeName);
		var rt = newRow.AddComponent<RectTransform>();
		rt.SetParent(_info.ScreenRoot.transform, false);
	}

	/// <summary>
	/// Sets the scroll screen using Grid Layout Group.
	/// </summary>
	private void SetScrollScreen() {
		_info.ScreenRoot.childAlignment = TextAnchor.LowerLeft;

		//cell width and height: 1000 * BulletScreenDisplayerModel.RowHeight,1000 fixed...
		_info.ScreenRoot.cellSize = new Vector2(ROW_WIDTH, _info.RowHeight);

		//all height...
		//_bulletScreenHeight = _info.ScreenRoot.GetComponent<RectTransform>().rect.height;
		_bulletScreenHeight = Screen.height;
	}

	public Transform GetTempRoot() {
		return _info.ScreenRoot.transform.Find(string.Format("column_{0}", 0));
	}

	/// <summary>
	/// Gets the row root.
	/// Gets the row root.根据当前节点下是否有textinfo元素挂靠,写死为0.
	/// 图片和视频居中...
	/// </summary>
	/// <returns>The row root.</returns>
	/// <param name="newTextInfo">New text info.</param>
	/// <param name="timeGap">Time gap.</param>
	public Transform GetRowRoot( VerticalScreenTextElement newTextElement, out float timeGap ) {
		//set ahead time..
		newTextElement.TextInfo.SendTime = Time.realtimeSinceStartup;
		timeGap = 0;
		if (_currBulletTextElement != null) {
			float lastHeight = _currBulletTextElement.TextInfo.TextHeight + _currBulletTextElement.TextInfo.ImageHeight;
			timeGap = GetAheadTime(lastHeight, newTextElement.TextInfo.TextHeight + newTextElement.TextInfo.ImageHeight);

			//add animation delay time and send delta time..
			timeGap += _currBulletTextElement.GetMoveReadyTime ();

			//update send time
			//timeGap += newTextElement.TextInfo.SendTime - _currBulletTextElement.TextInfo.SendTime; //send delta time
		}

		//_currBulletTextInfoList[searchedRowIndex] = newTextInfo;
		_currBulletTextElement = newTextElement;
		Transform root = _info.ScreenRoot.transform.Find(string.Format("column_{0}", 0));
		return root;
	}

	/// <summary>
	/// Logic of last bullet text go ahead.
	/// Gets the ahead time.
	/// One picture after another must lag...
	/// TODO:待 优化》...
	/// </summary>
	/// <returns>The ahead time.</returns>
	/// <param name="lastBulletTextHeight">Last bullet text height.</param>
	/// <param name="newCameBulletTextHeight">New came bullet text height.</param>
	private float GetAheadTime(float lastBulletTextHeight, float newCameBulletTextHeight) {
		Log.info (this, "GetAheadTime : lastBulletTextHeight:" + lastBulletTextHeight + ":newCameBulletTextHeight:" + newCameBulletTextHeight);
		float aheadTime = 0f;
		/*
		if (lastBulletTextHeight <= newCameBulletTextHeight) {
			float s1 = lastBulletTextHeight + BulletScreenHeight + _info.MinInterval;
			float v1 = (lastBulletTextHeight + BulletScreenHeight) / _info.ScrollDuration;
			float s2 = BulletScreenHeight;
			float v2 = (newCameBulletTextHeight + BulletScreenHeight) / _info.ScrollDuration;
			aheadTime = s1 / v1 - s2 / v2;  
		}
		else {
			float aheadDistance = lastBulletTextHeight + _info.MinInterval;
			float v1 = (lastBulletTextHeight + BulletScreenHeight) / _info.ScrollDuration;
			aheadTime = aheadDistance / v1;
		}*/
		float s1 = lastBulletTextHeight + _info.MinInterval;
		float v1 = (lastBulletTextHeight * 2 + BulletScreenHeight) / _info.ScrollDuration;
		aheadTime = s1 / v1;

		return aheadTime;
	}
	#endif
}

