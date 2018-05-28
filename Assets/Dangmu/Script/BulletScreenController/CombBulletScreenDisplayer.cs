using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletScreen;
using Common;

public class CombBulletScreenDisplayer : BaseView {
	
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
	public CombBulletTextInfo[] _currBulletTextInfoList; 
	[SerializeField]private BulletScreenDisplayerModel _info; //for model...
	public float ScrollDuration {
		get { return _info.ScrollDuration; }
	}

	public Canvas DisPlayCanvas;
	private float _bulletScreenWidth;
	public float BulletScreenWidth {
		get { return _bulletScreenWidth; }
	}

	public GameObject CombTextElementPrefab {
		get { return _info.TextPrefab; }
	}

	public string TextBoxNodeName {
		get { return _info.TextBoxNodeName; }
	}

	public float KillBulletTextDelay {
		get { return _info.KillBulletTextDelay; }
	}

	public Transform ScreenRoot {
		get { return _info.ScreenRoot.transform; }
	}

	public static CombBulletScreenDisplayer Create(BulletScreenDisplayerModel displayerInfo) {
		CombBulletScreenDisplayer instance = displayerInfo.Owner.gameObject.AddComponent<CombBulletScreenDisplayer>();
		instance._info = displayerInfo;
		return instance;
	}

	/// <summary>
	/// Adds the bullet, then it will display the bullet screen effect..
	/// </summary>
	/// <param name="textContent">Text content.</param>
	/// <param name="showBox">If set to <c>true</c> show box.</param>
	/// <param name="direction">Direction.</param>
	public void AddBullet(string name, string content, bool showBox = false, ScrollDirection direction = ScrollDirection.RightToLeft, 
		Sprite head = null,  Sprite photo = null) {
		CombScreenTextElement.Create(this, name, content, showBox, direction, head, photo);
	}

	private void Start() {
		SetScrollScreen();
		InitRow();

	}

	/// <summary>
	/// Inits the row.
	/// </summary>
	private void InitRow() {
		Utility.DestroyAllChildren(_info.ScreenRoot.gameObject);

		//显示的函数列表..总共有TotalRowCount行显示信息...
		_currBulletTextInfoList = new CombBulletTextInfo[_info.TotalRowCount];
		for (int rowIndex = 0; rowIndex < _info.TotalRowCount; rowIndex++) {
			_currBulletTextInfoList[rowIndex] = null;

			//节点名字....设置节点的父亲节点为_info的ScreenRoot节点...
			string rowNodeName = string.Format("row_{0}", rowIndex);
			GameObject newRow = new GameObject(rowNodeName);
			var rt = newRow.AddComponent<RectTransform>();
			rt.SetParent(_info.ScreenRoot.transform, false);
		}
	}

	private void SetScrollScreen() {
		_info.ScreenRoot.childAlignment = TextAnchor.MiddleCenter;
		_info.ScreenRoot.cellSize = new Vector2(100F, _info.RowHeight);
		//_bulletScreenWidth = _info.ScreenRoot.GetComponent<RectTransform>().rect.width;

		//Canvas width
		//_bulletScreenWidth = Screen.width;
		_bulletScreenWidth = DisPlayCanvas.pixelRect.width;
		Log.debug (this, "SetScrollScreen: _bulletScreenWidth: " + _bulletScreenWidth);
	}

	public Transform GetTempRoot() {
		return _info.ScreenRoot.transform.Find(string.Format("row_{0}", 0));
	}

	/// <summary>
	/// Gets the row root.根据当前节点下是否有textinfo元素挂靠,没有的话就初始化一个..
	/// 图片和视频居中...
	/// </summary>
	/// <returns>The row root.</returns>
	/// <param name="newTextInfo">New text info.</param>
	public Transform GetRowRoot( CombBulletTextInfo newTextInfo ) {
		const int notFoundRowIndex = -1;
		int searchedRowIndex = notFoundRowIndex;

		//发送时间记为现在时间...
		newTextInfo.SendTime = Time.realtimeSinceStartup;

		//
		if ( newTextInfo.IsText ) {
			for (int rowIndex = 0; rowIndex < _currBulletTextInfoList.Length; rowIndex++) {
				var textInfo = _currBulletTextInfoList [rowIndex];
				//if no bullet text info exist in this row, create the new directly.
				if (textInfo == null) {
					searchedRowIndex = rowIndex;
					break;
				}

				//没有空余节点就重复使用....
				Debug.Log("textInfo.IsText:" + textInfo.IsText);
				Debug.Log("newTextInfo.IsText:" + newTextInfo.IsText);
				float l1 = ( textInfo.IsText ) ? textInfo.TextWidth : textInfo.ImageWidth;
				float l2 = ( newTextInfo.IsText ) ? newTextInfo.TextWidth : newTextInfo.ImageWidth;
				//float l1 = textInfo.TextWidth;
				//float l2 = newTextInfo.TextWidth;

				//发送时间间隔必须超过文本长度...
				float sentDeltaTime = newTextInfo.SendTime - textInfo.SendTime;
				var aheadTime = GetAheadTime (l1, l2);
				if (sentDeltaTime >= aheadTime) {//fit and add.
					searchedRowIndex = rowIndex;
					break;
				}
				//go on searching in next row.
			}
		} else {
			searchedRowIndex = _currBulletTextInfoList.Length / 2 ;
			if (_currBulletTextInfoList.Length > 6) {
				//手机图片有些太大了随机0-2....
				int repairRowIndex = Random.Range(0, 3);
				searchedRowIndex = repairRowIndex;
			}
		}

		//找不到合适节点, 随机一个...
		if (searchedRowIndex == notFoundRowIndex) {//no fit but random one row.
			int repairRowIndex = Random.Range(0, _currBulletTextInfoList.Length);
			searchedRowIndex = repairRowIndex;
		}

		//
		_currBulletTextInfoList[searchedRowIndex] = newTextInfo;
		Transform root = _info.ScreenRoot.transform.Find(string.Format("row_{0}", searchedRowIndex));
		return root;
	}

	/// <summary>
	/// Logic of last bullet text go ahead.
	/// </summary>
	/// <param name="lastBulletTextWidth">width of last bullet text</param>
	/// <param name="newCameBulletTextWidth">width of new came bullet text</param>
	/// <returns></returns>
	private float GetAheadTime(float lastBulletTextWidth, float newCameBulletTextWidth) {
		float aheadTime = 0f;
		if (lastBulletTextWidth <= newCameBulletTextWidth) {
			float s1 = lastBulletTextWidth + BulletScreenWidth + _info.MinInterval;
			float v1 = (lastBulletTextWidth + BulletScreenWidth) / _info.ScrollDuration;
			float s2 = BulletScreenWidth;
			float v2 = (newCameBulletTextWidth + BulletScreenWidth) / _info.ScrollDuration;
			aheadTime = s1 / v1 - s2 / v2;  
		}
		else {
			float aheadDistance = lastBulletTextWidth + _info.MinInterval;
			float v1 = (lastBulletTextWidth + BulletScreenWidth) / _info.ScrollDuration;
			aheadTime = aheadDistance / v1;
		}
		return aheadTime;
	}
	#endif
}
