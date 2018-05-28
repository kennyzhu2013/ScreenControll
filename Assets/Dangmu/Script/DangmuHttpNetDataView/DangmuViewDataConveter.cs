using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangmuViewDataConveter  {
	public uint _indexId;
	public uint _weixinId;
	public uint _rid; //活动id标记....

	public string _openid;
	public string _nickname; //
	public string _avatar; //用户头像url...
	public string _content; //内容为文字或图片,需要load...
	public string _image;

	//1:文本 2:图片....
	public enum ETextType{
		ETextType_NULL = 0,
		ETextType_TEXT = 1,
		ETextType_PICTURE = 2,
		ETextType_BAPING = 3,
		ETextType_other //not support for present
	}
	public uint _type; //消息类型,ETextType
	public string _createtime;

	public Sprite _head;
	public Sprite _body;

	/// <summary>
	/// Determines whether this instance is text.
	/// </summary>
	/// <returns><c>true</c> if this instance is text; otherwise, <c>false</c>.</returns>
	public bool IsText()
	{
		return (_type == (uint)ETextType.ETextType_TEXT);
	}

	public bool IsPicture()
	{
		return (_type == (uint)ETextType.ETextType_PICTURE);
	}

	/// <summary>
	/// Determines whether this instance is unknown.
	/// </summary>
	/// <returns><c>true</c> if this instance is unknown; otherwise, <c>false</c>.</returns>
	public bool IsUnknown()
	{
		return (_type == (uint)ETextType.ETextType_other) || (_type == (uint)ETextType.ETextType_BAPING) || (_type == (uint)ETextType.ETextType_NULL);
	}

	/// <summary>
	/// The weight dict.可选颜色定义和对应权重,综合为累加值...
	/// </summary>
	/*
	public static Dictionary<object, float> WeightDict = new Dictionary<object, float>() {
		{"<color=yellow>{0}</color>", 30f},
		{"<color=red>{0}</color>", 10f},
		{"<color=white>{0}</color>", 60f}
	};*/
	public static Dictionary<object, float> WeightDict = new Dictionary<object, float>() {
		{"<div style='font-size:24px;color:blue;font-weight:bold'>", 30f},
		{"<div style='font-size:24px;color:red;font-weight:bold'>", 10f},
		{"<div style='font-size:24px;color:white;font-weight:bold'>", 60f}
	};

	//

	/// <summary>
	/// Initializes a new instance of the <see cref="DangmuViewDataConveter"/> class.
	/// To delete...
	/// </summary>
	/// <param name="indexId">Index identifier.</param>
	/// <param name="weixinId">Weixin identifier.</param>
	/// <param name="rid">Rid.</param>
	/// <param name="openid">Openid.</param>
	/// <param name="nickname">Nickname.</param>
	/// <param name="avatar">Avatar.</param>
	/// <param name="content">Content.</param>
	/// <param name="type">Type.</param>
	/// <param name="createtime">Createtime.</param>
	public DangmuViewDataConveter(uint indexId, uint weixinId, uint rid,
		string openid, string nickname, string avatar, string content,
		uint type, string createtime)
	{
		this._indexId = indexId;
		this._weixinId = weixinId;
		this._rid = rid;

		this._openid = openid;
		this._nickname = nickname;
		this._avatar = avatar;
		this._content = content;

		this._type = type;
		this._createtime = createtime;
		this._head = null;
		this._body = null;
	}

	public DangmuViewDataConveter(DangmuInitRsp.DangmuInitData source)
	{
		this._indexId = source.indexId;
		this._weixinId = source.weixinId;
		this._rid = source.rid;

		this._openid = source.openid;
		this._nickname = source.nickname;
		this._avatar = source.avatar;
		this._content = source.content;
		this._image = source.imageurl;

		this._type = (uint)ParseType( source.type );
		this._createtime = source.createtime;
		this._head = null;
		this._body = null;
	}

	public DangmuViewDataConveter(DangmuTextRsp.DangmuTextData source)
	{
		this._indexId = source.indexId;
		this._weixinId = source.weixinId;
		this._rid = source.rid;

		this._openid = source.openid;
		this._nickname = source.nickname;
		this._avatar = source.avatar;
		this._content = source.content;
		this._image = source.imageurl;

		this._type = (uint)ParseType( source.type );
		this._createtime = source.createtime;
		this._head = null;
		this._body = null;
	}

	//type: 消息类型  text:文本   image:图片  bp：霸屏  ds打sang  hb hong包    bb表白    gift送礼 ...
	public ETextType ParseType(string type)
	{
		ETextType result = ETextType.ETextType_other; //default...
		if ("text" == type)
			result = ETextType.ETextType_TEXT;
		else if ("image" == type)
			result = ETextType.ETextType_PICTURE;
		else if ("bp" == type)
			result = ETextType.ETextType_BAPING;
		else{
			//nothing to do.
		}
		return result;
	}

	/// <summary>
	/// Gets the text.测试时随机获取,颜色也随机....
	/// </summary>
	/// <returns>The text.</returns>
	public string GetColoredText() {
		string disText = "<color=yellow>" + _nickname + "</color>: ";

		//TODO: add rando color
		if ((uint)ETextType.ETextType_TEXT == _type) {
			//_content
			string randomColor = (string)Utility.RandomObjectByWeight(WeightDict);
			_content = randomColor + _content + "</div>";
		}


		//string text = string.Format(randomColor, disText);
		return disText;
	}
}
