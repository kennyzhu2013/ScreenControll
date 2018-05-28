using System;
using UnityEngine;

public class AwardGetRsp
{
	public uint Result;
	public uint awardId;
	public uint awardType;
	public string  awardName;
	public string  awardLevel;
	//public const string CLASS_NAME="AwardRsp";

	public bool ParseJsonObject(JSONObject _jsonObect)
	{
		//JSONObject(Dictionary<string, JSONObject> dic)
		//if (_jsonObect.className != CLASS_NAME)
		//return false;

		_jsonObect.GetField (ref Result, "resultCode");

		JSONObject _objectTemp = _jsonObect.GetField ("data");
		if (null == _objectTemp) {
			Debug.LogError ("AwardGetRsp: data get failed!");
			return false;
		}

		_objectTemp.GetField (ref awardId, "awardId");
		_objectTemp.GetField (ref awardType, "awardType");
		_objectTemp.GetField (ref awardName, "awardName");
		_objectTemp.GetField (ref awardName, "awardLevel");
		return true;
	}
}

