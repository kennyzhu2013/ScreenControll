using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletScreen;
using UnityEngine.UI;
using DG.Tweening;
using ZenFulcrum.EmbeddedBrowser;

public class CombScreenTextElement : MonoBehaviour {
	public const int HEAD_IMAGE_SIZE = 65; //head = 65*65 px..

	//_bullettext...
	[SerializeField]private CombBulletScreenDisplayer   _displayer;
	[SerializeField]private string                  _nameText; //文本内容...
	[SerializeField]private string                  _htmlContent; //html内容..
	[SerializeField]private bool                    _showBox; //文字是否加边框...
	[SerializeField]private ScrollDirection         _scrollDirection; //文字滚动方向..
	[SerializeField]private Text                    _text; //文本text....
	[SerializeField]private Browser                 _browser; //htmltext....
	[SerializeField]private float                   _textWidth;
	[SerializeField]private Vector3                 _startPos;
	[SerializeField]private Vector3                 _endPos;

	[SerializeField]private Sprite _head; //正上方左边图
	[SerializeField]private Sprite _image; //位于正下方...
	[SerializeField]private Image _headImage;
	[SerializeField]private Image _messageImage;
	[SerializeField]private float                   _bodyImageWidth; //图片body的宽度...
	[SerializeField]private float                   _bodyImageheight; //图片body的长度...

	//TODO:video, add later...
	//TODO:video需要居中显示....

	/// <summary>
	/// Create the specified displayer, textContent, showBox and direction.
	/// 创建多媒体弹幕....
	/// </summary>
	/// <param name="displayer">Displayer.</param>
	/// <param name="textContent">Text content.</param>
	/// <param name="showBox">If set to <c>true</c> show box.</param>
	/// <param name="direction">Direction.</param>
	public static CombScreenTextElement Create(CombBulletScreenDisplayer displayer, string name, string htmlText,
		bool showBox = false,
		ScrollDirection direction = ScrollDirection.RightToLeft,
		Sprite head = null,
		Sprite photo = null) {
		CombScreenTextElement instance = null;
		if (displayer == null) {
			Debug.Log("CombScreenTextElement.Create(), displayer can not be null !");
			return null;
		}

		GameObject go = Instantiate(displayer.CombTextElementPrefab) as GameObject;
		go.transform.SetParent(displayer.GetTempRoot());
		go.transform.localPosition = Vector3.up * 1000F;
		go.transform.localScale = Vector3.one;

		//add text...
		//RectTransform rect = go.GetComponent<RectTransform> ();
		//rect.rect.height = 100;
		//rect.rect.width = 100;
		instance = go.AddComponent<CombScreenTextElement>();
		instance._displayer = displayer;
		instance._nameText = name;
		instance._htmlContent = htmlText;
		instance._showBox = showBox;
		instance._scrollDirection = direction; 

		//set photo and image photo...
		instance._head = head;
		instance._image = photo;

		return instance;
	}

	/// <summary>
	/// Start this instance....
	/// </summary>
	private IEnumerator Start() {
		SetBoxView();
		SetText();
		//get correct text width in next frame.
		yield return new WaitForSeconds(0.2f);

		//Set html text...
		SetHtmlText ();

		SetSprites ();
		//get correct sprite width and height in next frame.
		yield return new WaitForSeconds(0.2f);

		RecordTextWidthAfterFrame();
		SetRowInfo();
		SetTweenStartPosition();
		SetTweenEndPosition();
		StartMove();
	}
	#region func_start_done	
	/// <summary>
	/// The outer box view of text...
	/// 设置文字边框....
	/// </summary>
	private void SetBoxView() {
		Transform boxNode = transform.Find(_displayer.TextBoxNodeName);
		if (boxNode == null) {
			//Debug.LogErrorFormat("BulletScreenTextElement.SetBoxView(), boxNode == null. boxNodeName: {0}",
			//_displayer.TextBoxNodeName);
			return;
		}
		boxNode.gameObject.SetActive(_showBox);
	}

	/// <summary>
	/// Sets the text...
	/// 设置文本内容...
	/// </summary>
	private void SetText() {
		_text = GetComponentInChildren<Text>();
		//_text.enabled = false;
		if (_text == null) {
			Debug.Log("CombScreenTextElement.SetText(), not found Text!");
			return;
		}

		//飘字方向...
		_text.alignment = _scrollDirection == ScrollDirection.RightToLeft ? TextAnchor.MiddleLeft : TextAnchor.MiddleRight;

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
		//set horizontal....
		sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
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
			_messageImage.transform.localScale = Vector3.one * 0.5f;

		} else if (_image == null) {
			_messageImage.enabled = false;
			//else to do?..
		}
	}

	/// <summary>
	/// Records the text and image width after frame.
	/// </summary>
	private void RecordTextWidthAfterFrame() {
		_textWidth = _text.GetComponent<RectTransform>().sizeDelta.x + HEAD_IMAGE_SIZE;

		//To adjust text height...
		if (_browser != null) {
			RectTransform rect = _browser.GetComponent<RectTransform> ();

			//adjust width..
			Vector3 position = rect.localPosition;
			position.x = _textWidth + 20;
			rect.localPosition = position;

			//reset...
			_textWidth += rect.sizeDelta.x + 20;
		}


		//adjust image width and height...
		_bodyImageWidth = _messageImage.GetComponent<RectTransform>().sizeDelta.x;
		_bodyImageheight = _messageImage.GetComponent<RectTransform>().sizeDelta.y;
	}

	/// <summary>
	/// Sets the tween start position for all the elements.
	/// </summary>
	private void SetTweenStartPosition() {
		Vector3 nor = _scrollDirection == ScrollDirection.RightToLeft ? Vector3.right : Vector3.left;
		_startPos = nor * (_displayer.BulletScreenWidth / 2f + _textWidth / 2f); // _textWidth / 2F
		transform.localPosition = _startPos;
		Debug.Log ("SetTweenStartPosition:_displayer.BulletScreenWidth:" + _displayer.BulletScreenWidth + 
			" _textWidth:" + _textWidth + " [Result]:" + transform.localPosition);
	}

	/// <summary>
	/// Sets the tween end position.
	/// </summary>
	private void SetTweenEndPosition() {
		Vector3 nor = _scrollDirection == ScrollDirection.RightToLeft ? Vector3.left : Vector3.right;
		//float f = Screen.width / _displayer.BulletScreenWidth;
		_endPos = nor * ( _displayer.BulletScreenWidth / 2f + _textWidth / 2f ); // _textWidth / 2F
		Debug.Log ("SetTweenEndPosition:" + _endPos);
	}

	/// <summary>
	/// Sets the row info and init CombBulletTextInfo.
	/// </summary>
	private void SetRowInfo() {
		var combBulletTextInfo = new CombBulletTextInfo() {
			SendTime = Time.realtimeSinceStartup,
			TextWidth = _textWidth,
			ImageWidth = _bodyImageWidth,
			ImageHeight = _bodyImageheight
		};

		//目前只有图片一类非文本消息....
		combBulletTextInfo.IsText = (null == _image);

		//找到对应行道的根节点row_{0},并且位置和父节点一致...
		var rowRoot = _displayer.GetRowRoot( combBulletTextInfo );
		transform.SetParent(rowRoot, false);

		//大小不进行缩放》...
		transform.localScale = Vector3.one;
	}

	//开始移动...
	private void StartMove() {
		//make sure the text is active.
		//the default ease of DoTewwen is not Linear.
		transform.DOLocalMoveX(_endPos.x, _displayer.ScrollDuration).OnComplete(OnTweenFinished).SetEase(Ease.Linear);
	}
	#endregion

	//结束后销毁...
	private void OnTweenFinished() {
		Destroy(gameObject, _displayer.KillBulletTextDelay);
	}

	//增加对DangmuInitData操作...
}
