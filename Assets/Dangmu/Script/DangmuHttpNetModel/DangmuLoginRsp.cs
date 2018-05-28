using System;
using LitJson;
using System.Collections.Generic;
using UnityEngine;

public class DangmuLoginRsp
{
	public int result;
	public uint weixinId;
	public bool bMark;
	//public uint rid;

	public string nickname;
	public string avatar; //用户头像url...

	//public string name;
	public string qrcodeUrl; //需要load...

	public string actName; //activity name..

	//public const string CLASS_NAME="AwardRsp";
	//jsonData2["name"]
	public bool ParseJsonObject(JsonData _jsonObect)
	{
		//if (_jsonObect.className != CLASS_NAME)
		//return false;
		result = int.Parse(_jsonObect["resultCode"].ToString());//_jsonObect["ret"];
		JsonData _jsonData = _jsonObect["data"];
		if ( null == _jsonData ) {
			Debug.LogError ("ParseJsonObject: data get failed!");
			return false;
		}

		weixinId = uint.Parse(_jsonData["weid"].ToString());
		qrcodeUrl = _jsonData ["qrcode"].ToString ();
		bMark = int.Parse (_jsonData["is_mark"].ToString()) == 1;
		actName = _jsonData ["activityName"].ToString ();
		//_jsonObect.GetField (ref Result, "ret");

		//data array partition...
		/*
		JsonData _jsonArray = _jsonObect["list"];
		//JSONObject _objectListTemp = _jsonObect.GetField ("data");
		if ( null == _jsonArray ) {
			Debug.LogError ("DangmuInitRsp: data get failed!");
			return false;
		}*/

		JsonData _jsonUserInfo = _jsonData["userinfo"];
		nickname = _jsonUserInfo ["uid_username"].ToString ();
		avatar = _jsonUserInfo ["avatar"].ToString ();

		return true;
	}
}
