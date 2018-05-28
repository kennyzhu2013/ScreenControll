using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletScreen;
using UnityEngine.UI;
using DG.Tweening;
using ZenFulcrum.EmbeddedBrowser;

/// <summary>
/// Vertical screen text element.
/// </summary>
public class VerticalScreenTextElement : MonoBehaviour {
	public enum VerticalDirection {
		LowerToUp = 0,
		UpToLower = 1
	}

	public const int HEAD_IMAGE_SIZE = 90; //head = 90*90 px..
	public const float MAX_ELEMENT_SIZE = 400f; //head = 90*90 px..
	public const float MIN_ELEMENT_SIZE = 100f; //head = 90*90 px..

	//_bullettext...
	[SerializeField]private VerticalScreenDisplayer _displayer;
	[SerializeField]private string                  _nameText; //文本内容...
	[SerializeField]private string                  _htmlContent; //文本内容...
	//[SerializeField]private bool                    _showBox; //文字是否加边框...
	[SerializeField]private VerticalDirection         _scrollDirection; //文字滚动方向..
	[SerializeField]private Text                    _text; //文本text....
	[SerializeField]private Browser                 _browser; //文本text....
	//[SerializeField]private float                   _textWidth;
	[SerializeField]private float                   _textHeight;

	[SerializeField]private Vector3                 _startPos;
	[SerializeField]private Vector3                 _endPos;
	public float Distance {
		get { return (_endPos.y - _startPos.y) > 0 ? (_endPos.y - _startPos.y) : (_startPos.y - _endPos.y); }
	}

	[SerializeField]private Sprite _head; //正上方左边图
	[SerializeField]private Sprite _image; //位于正下方...
	[SerializeField]private Image _headImage;
	[SerializeField]private Image _messageImage;
	[SerializeField]private Image _backgroundImage;
	[SerializeField]private float _bodyImageWidth; //图片body的宽度...
	[SerializeField]private float _bodyImageheight; //图片body的长度...
	[SerializeField]private CanvasGroup _canvasGroup;
	[SerializeField]private float  _timeGap = 0f;
	public CombBulletTextInfo TextInfo;

	enum ElementsStatus
	{
		None,
		Inited,
		Moving,
		Finished
	}
	[SerializeField]private ElementsStatus _status;
	[SerializeField]private float  _timeCounter = 0f;
	//TODO:video, add later...
	//TODO:video需要居中显示....

	/// <summary>
	/// Create the specified displayer, textContent, showBox, direction, head and photo.
	/// 垂直弹幕创建...
	/// </summary>
	/// <param name="displayer">Displayer.</param>
	/// <param name="textContent">Text content.</param>
	/// <param name="showBox">If set to <c>true</c> show box.</param>
	/// <param name="direction">Direction.</param>
	/// <param name="head">Head.</param>
	/// <param name="photo">Photo.</param>
	public static VerticalScreenTextElement Create(VerticalScreenDisplayer displayer, string name, string htmlText,
		VerticalDirection direction = VerticalDirection.LowerToUp,
		Sprite head = null,
		Sprite photo = null) {
		VerticalScreenTextElement instance = null;
		if (displayer == null) {
			Debug.Log("VerticalScreenTextElement.Create(), displayer can not be null !");
			return null;
		}

		GameObject go = Instantiate(displayer.VerticalTextElementPrefab) as GameObject;
		go.transform.SetParent(displayer.GetTempRoot());
		go.transform.localPosition = Vector3.up*10000F;//(0,10000,0)?...not seen,
		go.transform.localScale = Vector3.one;

		//add text...
		//RectTransform rect = go.GetComponent<RectTransform> ();
		//rect.rect.height = 100;
		//rect.rect.width = 100;
		instance = go.AddComponent<VerticalScreenTextElement>();
		instance._displayer = displayer;
		instance._nameText = name;
		instance._htmlContent = htmlText;

		//instance._showBox = showBox;
		instance._scrollDirection = direction; 

		//set photo and image photo...
		instance._head = head;
		instance._image = photo;

		return instance;
	}

	/// <summary>
	/// Start to show the element....
	/// </summary>
	private IEnumerator Start() {
		//TODO: add 3d effects..
		//SetBoxView();
		SetText();
		//get correct text width in next frame.
		yield return new WaitForSeconds(0.2f);

		//Set html text...
		SetHtmlText ();
		yield return new WaitForSeconds(0.2f);

		SetSprites ();
		//get correct sprite width and height in next frame.
		yield return new WaitForSeconds(0.2f);

		//record height
		RecordTextImageWidthAfterFrame();

		//Get Root row
		SetRowInfo();

		//set image heith and adjust background
		SetAndAjustImageHeight ();

		//
		SetTweenStartPosition();
		SetTweenEndPosition();
		StartMove();
	}

	// Update is called once per frame
	void Update () {
		if (ElementsStatus.Inited == _status) {
			_timeCounter += Time.deltaTime;
			if (_timeCounter >= _timeGap)
				Move ();
		}
	}
	#region func_start_done	

	/// <summary>
	/// Sets the text...
	/// 设置文本内容...
	/// </summary>
	private void SetText() {
		_text = GetComponentInChildren<Text>();

		//_text.enabled = false;
		if (_text == null) {
			Log.error(this, "VerticalScreenTextElement.SetText(), not found Text!");
			return;
		}

		//飘字方向...
		_text.alignment = _scrollDirection == VerticalDirection.LowerToUp ? TextAnchor.UpperLeft : TextAnchor.LowerLeft;

		//make sure there exist ContentSizeFitter componet for extend text width
		//文本大小输入框根据文字自动适应大小,和text,image and LayoutGroup系列组件配套...
		//https://www.woxueyuan.com/article/2547
		//The size is determined by the minimum or preferred sizes provided by layout element components on the Game Object. 
		//Such layout elements can be Image or Text components, layout groups, or a Layout Element component.
		var sizeFitter = _text.GetComponent<ContentSizeFitter>();
		if (!sizeFitter) {
			sizeFitter = _text.gameObject.AddComponent<ContentSizeFitter>();
		}

		//text should extend in horizontal
		//set vertical....
		sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
		_text.text = _nameText;
	}

	/// <summary>
	/// Sets the html text.
	/// </summary>
	private void SetHtmlText() {
		_browser = GetComponentInChildren<Browser>();

		//_text.enabled = false;
		if (_browser == null) {
			Log.error(this, "VerticalScreenTextElement.SetText(), not found _browser!");
			return;
		}

		Log.info (this, @_htmlContent);
		_browser.LoadHTML(@_htmlContent, null);
		//adjust 
	}

	/// <summary>
	/// Sets the head...
	/// </summary>
	private void SetSprites() {
		//include inactive images..
		Image[] images = GetComponentsInChildren<Image>(true);

		//_head = GetComponentInChildren(
		//_text.enabled = false;
		if (images == null) {
			Debug.Log("CombScreenTextElement.SetSprites(), not found any image!");
			return;
		}

		//find head and message images
		foreach (Image item in images) {
			if ("Head" == item.gameObject.name)
				_headImage = item;
			else if ("Photo" == item.gameObject.name)
				_messageImage = item;
			else if ("Background" == item.gameObject.name)
				_backgroundImage = item;
			else {
				//nothing to do..
			}
		}

		//SetHead
		if (_headImage == null) {
			Debug.Log("CombScreenTextElement.SetSprites(), not found headimage!");
			return;
		}
		_headImage.sprite = _head;

		//set message, optional,must fize fit..
		//判断是否有body，没有的话需要inactive...

		if (_messageImage != null && _image != null) {
			//TODO:...must set image active ..


			//_text.alignment = _scrollDirection == ScrollDirection.RightToLeft ? TextAnchor.MiddleLeft : TextAnchor.MiddleRight;
			//make sure there exist ContentSizeFitter componet for extend text width...
			var sizeFitter = _messageImage.GetComponent<ContentSizeFitter> ();
			if (!sizeFitter) {
				sizeFitter = _messageImage.gameObject.AddComponent<ContentSizeFitter> ();
			}

			//text should extend in horizontal
			sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
			sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
			_messageImage.sprite = _image;

			//手机分辨率太大了需要缩放更小些....
			_messageImage.transform.localScale = Vector3.one * 0.25f;

		} else if (_image == null) {
			_messageImage.enabled = false;
			//else to do?..
		}
	}

	/// <summary>
	/// Records the text and image width after frame.
	/// </summary>
	private void RecordTextImageWidthAfterFrame() {
		_textHeight = _text.GetComponent<RectTransform>().sizeDelta.y;

		//To adjust text height...
		if (_browser != null) {
			_textHeight = _browser.GetComponent<RectTransform> ().sizeDelta.y;
		}

		//adjust image width and height...
		if ( _image != null ) {
			_bodyImageWidth = _messageImage.GetComponent<RectTransform> ().sizeDelta.x;
			_bodyImageheight = _messageImage.GetComponent<RectTransform> ().sizeDelta.y;
		} else {
			_bodyImageWidth = 0f;
			_bodyImageheight = 0f;
		}
	}

	/// <summary>
	/// Sets the tween start position for all the elements.
	/// Low to up for moving...
	/// </summary>
	private void SetTweenStartPosition() {
		Vector3 nor = _scrollDirection == VerticalDirection.LowerToUp ? Vector3.down : Vector3.up;
		_startPos = nor * (_displayer.BulletScreenHeight / 2 + _textHeight); // _textWidth / 2F
		transform.localPosition = _startPos;
	}

	/// <summary>
	/// Sets the tween end position.
	/// </summary>
	private void SetTweenEndPosition() {
		Vector3 nor = _scrollDirection == VerticalDirection.LowerToUp ? Vector3.up : Vector3.down;
		//float f = Screen.width / _displayer.BulletScreenWidth;
		_endPos = nor * ( _displayer.BulletScreenHeight + _textHeight ); // _textWidth / 2F
	}

	/// <summary>
	/// set image heith and adjust background
	/// TODO://
	/// </summary>
	private void SetAndAjustImageHeight() {
		//20 for border...
		float totalHeight = _textHeight + _bodyImageheight + 20;

		//(float)MIN_ELEMENT_SIZE;
		if (totalHeight < MIN_ELEMENT_SIZE)
			totalHeight = MIN_ELEMENT_SIZE;
		else if (totalHeight >= MAX_ELEMENT_SIZE) {
			totalHeight = MAX_ELEMENT_SIZE;

			//picture suo fang...
			if (_image != null) {
				float ratio = (totalHeight - (float)HEAD_IMAGE_SIZE) / _bodyImageheight;
				//suofang..
				_messageImage.transform.localScale = Vector3.one * ratio;
			}
		}

		//set background height..
		_backgroundImage.GetComponent<RectTransform> ().sizeDelta 
			= new Vector2(_backgroundImage.GetComponent<RectTransform> ().sizeDelta.x, totalHeight);
	}

	/// <summary>
	/// Sets the row info and init CombBulletTextInfo.
	/// </summary>
	private void SetRowInfo() {
		TextInfo = new CombBulletTextInfo() {
			SendTime = Time.realtimeSinceStartup,
			TextHeight = _textHeight,
			ImageWidth = _bodyImageWidth,
			ImageHeight = _bodyImageheight
		};

		//目前只有图片一类非文本消息....
		TextInfo.IsText = (null == _image);

		//找到对应行道的根节点row_{0},并且位置和父节点一致...
		var rowRoot = _displayer.GetRowRoot( this, out _timeGap );
		transform.SetParent(rowRoot, false);

		//大小不进行缩放》...
		transform.localScale = Vector3.one;
	}

	/// <summary>
	/// Starts the move after _timeGap seconds...
	/// </summary>
	private void StartMove() {
		//make sure the text is active.
		//the default ease of DoTewwen is not Linear.
		//TODO:延时GetAheadTime后插入....
		//when gap not reach, how to ..
		//transform.DOScale(1f, _timeGap).OnComplete(Move);
		_timeCounter = 0f;
		_status = ElementsStatus.Inited;
		//Move();
	}

	private void Move()
	{
		_timeCounter = 0f;
		_status = ElementsStatus.Moving;
		transform.DOLocalMoveY(_endPos.y, _displayer.ScrollDuration).OnComplete(OnTweenFinished).SetEase(Ease.Linear);

		//alpha animation..
		_canvasGroup = GetComponentInChildren<CanvasGroup> ();
		_canvasGroup.DOFade (0, _displayer.ScrollDuration);
	}

	public float GetMoveReadyTime()
	{
		if (_status != ElementsStatus.Inited || 0.01f > _timeGap)
			return 0f;

		if ( _timeGap > _timeCounter )
			return (_timeGap - _timeCounter);

		return 0f;
	}
	#endregion

	//Destroy when finished...
	private void OnTweenFinished() {
		_status = ElementsStatus.Finished;
		Destroy(gameObject, _displayer.KillBulletTextDelay);
	}

	//增加对DangmuInitData操作...
}
