using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
public class DangmuInitReq {
	public uint weixinId = 3;
	//public string mobile = "13802881234";
	public int rid = 2;
	//public string Password;
	//public const string CLASS_NAME="";//all the same..

	private JSONObject _jsonObect = null;

	//return url body...
	/// <summary>
	/// Gets the URL string.
	/// 目前只有i表示后台分配的微信id，rid表示活动id...
	/// </summary>
	/// <returns>The URL string.</returns>
	public string GetUrlString()
	{
		string urlResult = "http://yxbwx.mmarket.com/weizannew/app/index.php?i=";
		urlResult += weixinId.ToString ();
		urlResult += "&c=entry&rid=";
		urlResult += rid.ToString ();
		//urlResult += "&do=dm&m=meepo_xianchang&op=api";
		urlResult += "&do=dada_getdm&lasttime=-1&op=api&m=dada_xianchang";
		//return "http://yxbwx.mmarket.com/weizannew/app/index.php?i=3&c=entry&rid=6&do=dada_getdm&lasttime=-1&op=api&m=dada_xianchang";
		return urlResult;
	}

}
