using System;

//data
public class AwardRsp
{
	public uint Result;
	public string  msg;
	//public const string CLASS_NAME="AwardRsp";

	public bool ParseJsonObject(JSONObject _jsonObect)
	{
		//JSONObject(Dictionary<string, JSONObject> dic)
		//if (_jsonObect.className != CLASS_NAME)
		//return false;

		_jsonObect.GetField (ref Result, "resultCode");

		_jsonObect.GetField (ref msg, "msg");
		return true;
	}
}

