using System;

/// <summary>
/// Dangmu login req.
/// 根据活动id登录...
/// </summary>
public class DangmuLoginReq
{
	public int weixinId = 3;
	//public string mobile = "13802881234";
	public int rid = 2;
	public string passwd;
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
		urlResult += "&do=dada_pclogin&password=";
		urlResult += passwd;
		urlResult += "&op=api&m=meepo_xianchang";
		//return "index.php?i=3&c=entry&rid=45&do=dada_pclogin&password=5678&op=api&m=meepo_xianchang";
		return urlResult;
	}

}

