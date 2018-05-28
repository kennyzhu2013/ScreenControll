using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletScreen;
using LitJson;

public class ExampleDangmuScreen : MonoBehaviour {
	public CombBulletScreenDisplayer Displayer;

	//测试数据.....
	public List<DangmuViewDataConveter> _textPool; /*= new List<DangmuViewDataConveter>() {
		{20, 3, 2, "oYZTz0n7_wbHTNjn9euQCdDjO2p0", "Nopromises", "http://wx.qlogo.cn/mmopen/vi_32/OJcLpmXK9LVicBl9PgMyjQroh2XDPkh6AB6C1W3bxqEtVoOhbKcq546LBU4xlJxlFUhaia7u1rm1Gz1GH90ZIMwA/0", 
			"hello world 1数据接口接口数据库", 1, "1513326199"},
		{21, 3, 2, "oYZTz0n7_wbHTNjn9euQCdDjO2p0", "老虎怕小羊", "http://wx.qlogo.cn/mmopen/vi_32/IN2icsuhGqEdGribiaXf6oqCWLZAh0V6Z5UiacN6sbGyzXibjLomY5It6CSGia9190XAMPTB7NkD0jc4pTXZ9r9Dw5PQ/0", 
			"http://yxbwx.mmarket.com/weizannew/attachment/images/meepo_xianchang/N5kkwL2SvlY2N714sy1knAy7L112zu.jpg", 2, "1513308112"},
		{22, 3, 2, "oYZTz0n7_wbHTNjn9euQCdDjO2p0", "Nopromises", "http://wx.qlogo.cn/mmopen/vi_32/OJcLpmXK9LVicBl9PgMyjQroh2XDPkh6AB6C1W3bxqEtVoOhbKcq546LBU4xlJxlFUhaia7u1rm1Gz1GH90ZIMwA/0", 
			"每一步风起云涌...", 1, "1513326199"},
		{23, 3, 2, "oYZTz0n7_wbHTNjn9euQCdDjO2p0", "老虎怕小羊", "http://wx.qlogo.cn/mmopen/vi_32/IN2icsuhGqEdGribiaXf6oqCWLZAh0V6Z5UiacN6sbGyzXibjLomY5It6CSGia9190XAMPTB7NkD0jc4pTXZ9r9Dw5PQ/0", 
			"http://yxbwx.mmarket.com/weizannew/attachment/images/meepo_xianchang/N5kkwL2SvlY2N714sy1knAy7L112zu.jpg", 2, "1513308112"},
		{24, 3, 2, "oYZTz0n7_wbHTNjn9euQCdDjO2p0", "Nopromises", "http://wx.qlogo.cn/mmopen/vi_32/OJcLpmXK9LVicBl9PgMyjQroh2XDPkh6AB6C1W3bxqEtVoOhbKcq546LBU4xlJxlFUhaia7u1rm1Gz1GH90ZIMwA/0", 
			"在风雨之中你追我逐...", 1, "1513326199"},
		{25, 3, 2, "oYZTz0n7_wbHTNjn9euQCdDjO2p0", "老虎怕小羊", "http://wx.qlogo.cn/mmopen/vi_32/OJcLpmXK9LVicBl9PgMyjQroh2XDPkh6AB6C1W3bxqEtVoOhbKcq546LBU4xlJxlFUhaia7u1rm1Gz1GH90ZIMwA/0", 
			"把风花雪月留在心中...", 1, "1513326199"},
		{26, 3, 2, "oYZTz0n7_wbHTNjn9euQCdDjO2p0", "Nopromises", "http://wx.qlogo.cn/mmopen/vi_32/OJcLpmXK9LVicBl9PgMyjQroh2XDPkh6AB6C1W3bxqEtVoOhbKcq546LBU4xlJxlFUhaia7u1rm1Gz1GH90ZIMwA/0", 
			"剩下度追忆是不舍不倦...", 1, "1513326199"},
		{27, 3, 2, "oYZTz0n7_wbHTNjn9euQCdDjO2p0", "老虎怕小羊", "http://wx.qlogo.cn/mmopen/vi_32/OJcLpmXK9LVicBl9PgMyjQroh2XDPkh6AB6C1W3bxqEtVoOhbKcq546LBU4xlJxlFUhaia7u1rm1Gz1GH90ZIMwA/0", 
			"长夜里没法睡男儿无泪...", 1, "1513326199"},
		{28, 3, 2, "oYZTz0n7_wbHTNjn9euQCdDjO2p0", "老虎怕小羊", "http://wx.qlogo.cn/mmopen/vi_32/IN2icsuhGqEdGribiaXf6oqCWLZAh0V6Z5UiacN6sbGyzXibjLomY5It6CSGia9190XAMPTB7NkD0jc4pTXZ9r9Dw5PQ/0", 
			"http://yxbwx.mmarket.com/weizannew/attachment/images/meepo_xianchang/N5kkwL2SvlY2N714sy1knAy7L112zu.jpg", 2, "1513308112"},
		{29, 3, 2, "oYZTz0n7_wbHTNjn9euQCdDjO2p0", "Nopromises", "http://wx.qlogo.cn/mmopen/vi_32/OJcLpmXK9LVicBl9PgMyjQroh2XDPkh6AB6C1W3bxqEtVoOhbKcq546LBU4xlJxlFUhaia7u1rm1Gz1GH90ZIMwA/0", 
			"问大地慈爱迟迟短缺是深刻思考...", 1, "1513326199"}
	};*/
	void constructTextPool()
	{
		_textPool = new List<DangmuViewDataConveter> ();
		DangmuViewDataConveter dataTemp = new DangmuViewDataConveter (20, 3, 2, "oYZTz0n7_wbHTNjn9euQCdDjO2p0", "Nopromises", 
			"http://wx.qlogo.cn/mmopen/vi_32/OJcLpmXK9LVicBl9PgMyjQroh2XDPkh6AB6C1W3bxqEtVoOhbKcq546LBU4xlJxlFUhaia7u1rm1Gz1GH90ZIMwA/0", 
			"hello world 1数据接口接口数据库", 1, "1513326199");
		_textPool.Add (dataTemp);

		dataTemp = new DangmuViewDataConveter (21, 3, 2, "oYZTz0n7_wbHTNjn9euQCdDjO2p0", "老虎怕小羊", 
			"http://wx.qlogo.cn/mmopen/vi_32/IN2icsuhGqEdGribiaXf6oqCWLZAh0V6Z5UiacN6sbGyzXibjLomY5It6CSGia9190XAMPTB7NkD0jc4pTXZ9r9Dw5PQ/0", 
			"http://yxbwx.mmarket.com/weizannew/attachment/images/meepo_xianchang/N5kkwL2SvlY2N714sy1knAy7L112zu.jpg", 2, "1513308112");
		_textPool.Add (dataTemp);

		dataTemp = new DangmuViewDataConveter (22, 3, 2, "oYZTz0n7_wbHTNjn9euQCdDjO2p0", "Nopromises", 
			"http://wx.qlogo.cn/mmopen/vi_32/OJcLpmXK9LVicBl9PgMyjQroh2XDPkh6AB6C1W3bxqEtVoOhbKcq546LBU4xlJxlFUhaia7u1rm1Gz1GH90ZIMwA/0", 
			"每一步风起云涌...", 1, "1513326199");
		_textPool.Add (dataTemp);

		dataTemp = new DangmuViewDataConveter (23, 3, 2, "oYZTz0n7_wbHTNjn9euQCdDjO2p0", "老虎怕小羊", 
			"http://wx.qlogo.cn/mmopen/vi_32/IN2icsuhGqEdGribiaXf6oqCWLZAh0V6Z5UiacN6sbGyzXibjLomY5It6CSGia9190XAMPTB7NkD0jc4pTXZ9r9Dw5PQ/0", 
			"http://yxbwx.mmarket.com/weizannew/attachment/images/meepo_xianchang/N5kkwL2SvlY2N714sy1knAy7L112zu.jpg", 2, "1513308112");
		_textPool.Add (dataTemp);

		dataTemp = new DangmuViewDataConveter (24, 3, 2, "oYZTz0n7_wbHTNjn9euQCdDjO2p0", "Nopromises", 
			"http://wx.qlogo.cn/mmopen/vi_32/OJcLpmXK9LVicBl9PgMyjQroh2XDPkh6AB6C1W3bxqEtVoOhbKcq546LBU4xlJxlFUhaia7u1rm1Gz1GH90ZIMwA/0", 
			"在风雨之中你追我逐...", 1, "1513326199");
		_textPool.Add (dataTemp);

		dataTemp = new DangmuViewDataConveter (25, 3, 2, "oYZTz0n7_wbHTNjn9euQCdDjO2p0", "老虎怕小羊", 
			"http://wx.qlogo.cn/mmopen/vi_32/OJcLpmXK9LVicBl9PgMyjQroh2XDPkh6AB6C1W3bxqEtVoOhbKcq546LBU4xlJxlFUhaia7u1rm1Gz1GH90ZIMwA/0", 
			"http://yxbwx.mmarket.com/weizannew/attachment/images/meepo_xianchang/N5kkwL2SvlY2N714sy1knAy7L112zu.jpg", 2, "1513326199"); //"把风花雪月留在心中..."..
		_textPool.Add (dataTemp);

		dataTemp = new DangmuViewDataConveter (26, 3, 2, "oYZTz0n7_wbHTNjn9euQCdDjO2p0", "Nopromises", 
			"http://wx.qlogo.cn/mmopen/vi_32/OJcLpmXK9LVicBl9PgMyjQroh2XDPkh6AB6C1W3bxqEtVoOhbKcq546LBU4xlJxlFUhaia7u1rm1Gz1GH90ZIMwA/0", 
			"剩下度追忆是不舍不倦...", 1, "1513326199");
		_textPool.Add (dataTemp);

		dataTemp = new DangmuViewDataConveter (27, 3, 2, "oYZTz0n7_wbHTNjn9euQCdDjO2p0", "老虎怕小羊", 
			"http://wx.qlogo.cn/mmopen/vi_32/OJcLpmXK9LVicBl9PgMyjQroh2XDPkh6AB6C1W3bxqEtVoOhbKcq546LBU4xlJxlFUhaia7u1rm1Gz1GH90ZIMwA/0", 
			"http://yxbwx.mmarket.com/weizannew/attachment/images/meepo_xianchang/N5kkwL2SvlY2N714sy1knAy7L112zu.jpg", 2, "1513326199"); //"长夜里没法睡男儿无泪..."..
		_textPool.Add (dataTemp);

		dataTemp = new DangmuViewDataConveter (28, 3, 2, "oYZTz0n7_wbHTNjn9euQCdDjO2p0", "老虎怕小羊", 
			"http://wx.qlogo.cn/mmopen/vi_32/IN2icsuhGqEdGribiaXf6oqCWLZAh0V6Z5UiacN6sbGyzXibjLomY5It6CSGia9190XAMPTB7NkD0jc4pTXZ9r9Dw5PQ/0", 
			"http://yxbwx.mmarket.com/weizannew/attachment/images/meepo_xianchang/N5kkwL2SvlY2N714sy1knAy7L112zu.jpg", 2, "1513308112");
		_textPool.Add (dataTemp);

		dataTemp = new DangmuViewDataConveter (29, 3, 2, "oYZTz0n7_wbHTNjn9euQCdDjO2p0", "Nopromises", 
			"http://wx.qlogo.cn/mmopen/vi_32/OJcLpmXK9LVicBl9PgMyjQroh2XDPkh6AB6C1W3bxqEtVoOhbKcq546LBU4xlJxlFUhaia7u1rm1Gz1GH90ZIMwA/0", 
						"问大地慈爱迟迟短缺是深刻思考...", 1, "1513326199");
		_textPool.Add (dataTemp);
	}

	//测试解析数据...
	List<DangmuInitRsp.DangmuInitData> dataList;
	void test()
	{
		string testString = "{ \"ret\": 0, " +
			"\"data\": " +
			"    [" +
			"        {" +
			"            \"id\": \"19\", " +
			"            \"weid\": \"3\", " +
			"            \"rid\": \"2\", " +
			"            \"openid\": \"oYZTz0n7_wbHTNjn9euQCdDjO2p0\", " +
			"            \"nick_name\": \"Nopromises\", " +
			"            \"avatar\": \"http://wx.qlogo.cn/mmopen/vi_32/OJcLpmXK9LVicBl9PgMyjQroh2XDPkh6AB6C1W3bxqEtVoOhbKcq546LBU4xlJxlFUhaia7u1rm1Gz1GH90ZIMwA/0\"," +
			"            \"content\": \"hello world 啊啊啊啊啊啊啊啊\"," +
			"            \"type\": \"1\"," +
			"            \"status\": \"1\"," +
			"            \"createtime\": \"1513326199\"," +
			"            \"txt\": \"hello world 啊啊啊啊啊啊啊啊\"," +
			"            \"name\": \"Nopromises\"," +
			"            \"img\": \"http://wx.qlogo.cn/mmopen/vi_32/OJcLpmXK9LVicBl9PgMyjQroh2XDPkh6AB6C1W3bxqEtVoOhbKcq546LBU4xlJxlFUhaia7u1rm1Gz1GH90ZIMwA/0\"" +
			"        }," +
			"        {" +
			"            \"id\": \"16\"," +
			"            \"weid\": \"3\"," +
			"            \"rid\": \"2\"," +
			"            \"openid\": \"oYZTz0rXfW2NmeQry9K90F6S29So\"," +
			"            \"nick_name\": \"雪花神剑\"," +
			"            \"avatar\": \"http://wx.qlogo.cn/mmopen/vi_32/IN2icsuhGqEdGribiaXf6oqCWLZAh0V6Z5UiacN6sbGyzXibjLomY5It6CSGia9190XAMPTB7NkD0jc4pTXZ9r9Dw5PQ/0\"," +
			"            \"content\": \"http://yxbwx.mmarket.com/weizannew/attachment/images/meepo_xianchang/N5kkwL2SvlY2N714sy1knAy7L112zu.jpg\"," +
			"            \"type\": \"2\"," +
			"            \"status\": \"1\"," +
			"            \"createtime\": \"1513308112\"," +
			"            \"txt\": \"http://yxbwx.mmarket.com/weizannew/attachment/images/meepo_xianchang/N5kkwL2SvlY2N714sy1knAy7L112zu.jpg\"," +
			"            \"name\": \"雪花神剑\"," +
			"            \"img\": \"http://wx.qlogo.cn/mmopen/vi_32/IN2icsuhGqEdGribiaXf6oqCWLZAh0V6Z5UiacN6sbGyzXibjLomY5It6CSGia9190XAMPTB7NkD0jc4pTXZ9r9Dw5PQ/0\"" +
			"        }" +
			"    ]" +
			"}";

		JsonData _jsonObect = JsonMapper.ToObject( testString );
		int result = int.Parse(_jsonObect["ret"].ToString());//_jsonObect["ret"];
		//_jsonObect.GetField (ref Result, "ret");
		Debug.Log ("LoginManager#################result is:" + result);

		//data array partition...
		JsonData _jsonArray = _jsonObect["data"];
		//JSONObject _objectListTemp = _jsonObect.GetField ("data");
		if ( null == _jsonArray ) {
			Debug.LogError ("DangmuInitRsp: data get failed!");
			return;
		}

		//Get array list..
		foreach (JsonData item in _jsonArray)
		{
			DangmuInitRsp.DangmuInitData _jo = new DangmuInitRsp.DangmuInitData();
			uint itemp = uint.Parse(item["id"].ToString());
			_jo.indexId = itemp;
			itemp = uint.Parse(item["weid"].ToString());
			_jo.weixinId = itemp;
			itemp = uint.Parse(item["rid"].ToString());
			_jo.rid = itemp;
			Debug.Log ("LoginManager#################indexId is:" + _jo.indexId);
			Debug.Log ("            #################weixinId is:" + _jo.weixinId);
			Debug.Log ("            #################rid is:" + _jo.rid);

			_jo.openid = item["openid"].ToString();
			_jo.nickname = item["nick_name"].ToString();
			_jo.avatar = item["avatar"].ToString();
			_jo.content = item["content"].ToString();

			itemp = uint.Parse(item["type"].ToString());
			_jo.type = "text";
			//itemp = uint.Parse(item["status"].ToString());
			//_jo.status = itemp;

			_jo.createtime = item["createtime"].ToString();
			//_jo.text = item["txt"].ToString();
			//_jo.name = item["name"].ToString();
			_jo.imageurl = item["img"].ToString();
			//Debug.Log ("LoginManager#################text is:" + _jo.text);
			Debug.Log ("            #################name is:" + _jo.nickname);
			Debug.Log ("            #################imageurlis:" + _jo.imageurl);
			//_textPool.Add(_jo);
			dataList.Add(_jo);
		}
	}

	// Use this for initialization
	void Start() {
		Displayer.Enable = true;
		dataList = new List<DangmuInitRsp.DangmuInitData>();
		test ();
		constructTextPool ();

		//展示弹幕...
		StartCoroutine(StartDisplayBulletScreenEffect());
	}

	/// <summary>
	/// Starts the display bullet screen effect.
	/// 由于图片资源要下载，因此需要等图片下载完后回调....
	/// </summary>
	/// <returns>The display bullet screen effect.</returns>
	private IEnumerator StartDisplayBulletScreenEffect() {
		while (Displayer.Enable) {
			//随机获取某一行....
			int textIndex = Random.Range(0, _textPool.Count);
			DangmuViewDataConveter dangmuData = _textPool [textIndex];

			//在本类的函数中实现Sprite加载....
			//Sprite 
			//Sprite messageBody = dangmuData._content;
			TextureLoader.Instance().StartSpriteLoad(dangmuData._avatar, (uint)textIndex, HeadCallBack);

			if (dangmuData.IsPicture ()) {
				Debug.Log ("StartDisplayBulletScreenEffect: the content url is: " + dangmuData._content);
				TextureLoader.Instance().StartSpriteLoad(dangmuData._content, (uint)textIndex, BodyCallBack);
			}

			//add later...
			//Displayer.AddBullet(dangmuData.GetColoredText(), CheckShowBox(), GetDirection(), dangmuData._avatar, messageBody);
			yield return new WaitForSeconds(1.0f);
		}
	}

	private bool CheckShowBox() {
		var weightDict = new Dictionary<object, float>() {
			{true, 20f},
			{false, 80f}
		};
		bool ret = (bool)Utility.RandomObjectByWeight(weightDict);
		return ret;
	}

	private ScrollDirection GetDirection() {
		var weightDict = new Dictionary<object, float>() {
			{ScrollDirection.LeftToRight, 5f},
			{ScrollDirection.RightToLeft, 80f}
		};
		ScrollDirection direction = (ScrollDirection)Utility.RandomObjectByWeight(weightDict);
		return direction;
	}

	//以下回调如果图片信息未完成或头像失败则不会显示最终结果...
	//需要判断如果是图片信息的话需要等图片加载完....
	//根据DangmuViewDataConveter的sprite成员判断图片是否都加载完...
	#region Sprite_Load_Callback
	public void HeadCallBack(Sprite headSprite, uint textIndex)
	{
		//
		DangmuViewDataConveter dangmuData = _textPool [(int)textIndex];
		if (null == dangmuData) {
			Debug.LogError ("DangmuViewDataConveter not found,index: " + textIndex);
			return;
		}

		dangmuData._head = headSprite;
		if (dangmuData.IsText ()) {
			Displayer.AddBullet (dangmuData.GetColoredText (), dangmuData._content, CheckShowBox (), GetDirection (), headSprite, null);
		}
		else if (dangmuData.IsPicture () && dangmuData._head != null ) { //如果body加载完则创建弹幕...
			Displayer.AddBullet(dangmuData.GetColoredText(), dangmuData._content, CheckShowBox(), GetDirection(), dangmuData._head, dangmuData._body);
		}
	}

	//
	public void BodyCallBack(Sprite bodySprite, uint textIndex)
	{
		//
		DangmuViewDataConveter dangmuData = _textPool [(int)textIndex];
		if (null == dangmuData) {
			Debug.LogError ("DangmuViewDataConveter not found,index: " + textIndex);
			return;
		}

		Debug.Log ("BodyCallBack: bodySprite: " + bodySprite);
		Debug.Log ("BodyCallBack: textIndex: " + textIndex);
		dangmuData._body = bodySprite;
		if (dangmuData.IsPicture () && dangmuData._head != null ) {
			Displayer.AddBullet(dangmuData.GetColoredText(), dangmuData._content, CheckShowBox(), GetDirection(), dangmuData._head, dangmuData._body);
		}

		//NUll to od
	}

	#endregion
}
