using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangmuTextReq {

	public uint weixinId = 3;
	//public string mobile = "13802881234";
	public int rid = 2;
	public uint lastid;
	//public string Password;
	//public const string CLASS_NAME="";//all the same..

	private JSONObject _jsonObect = null;

	//return url body...
	/// <summary>
	/// Gets the URL string.
	/// 目前只有i表示后台分配的微信id，rid表示活动id...
	/// open_time为当前时间..?..
	/// </summary>
	/// <returns>The URL string.</returns>
	public string GetUrlString()
	{
		string urlResult = "http://yxbwx.mmarket.com/weizannew/app/index.php?i=";
		urlResult += weixinId.ToString ();
		urlResult += "&c=entry&rid=";
		urlResult += rid.ToString ();
		urlResult += "&do=dada_getmoredm&lastid=";
		urlResult += lastid.ToString ();
		urlResult += "&op=api&m=dada_xianchang";

		//TODO:确认时间戳取值》....
		//urlResult += MainWinController.Instance().LoginMgr.LoginTime;//Utility.GetUnixTimeStamp ().ToString ();
		//return "http://yxbwx.mmarket.com/weizannew/app/index.php?
		//i=3&c=entry&rid=2&do=dada_getmoredm&lastid=100&op=api&m=dada_xianchang";
		return urlResult;
	}

}
