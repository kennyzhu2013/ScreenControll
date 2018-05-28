using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AwardGetReq : MonoBehaviour {
	public int activityId = 160928001;
	public string mobile = "13802881234";
	public int step = 2;
	//public string Password;
	//public const string CLASS_NAME="";//all the same..

	private JSONObject _jsonObect = null;
	public string ToJsonStirng()
	{
		//JSONObject(Dictionary<string, JSONObject> dic)
		Dictionary<string, string> dicionary = new Dictionary<string, string>();
		//dicionary.Add ("activityId", activityId);
		dicionary.Add ("phone", mobile);
		_jsonObect = new JSONObject (dicionary);
		//_jsonObect = JSONObject.Create();
		//_jsonObect.AddField ("phone", mobile);
		_jsonObect.AddField ("activityId", activityId);
		_jsonObect.AddField ("activityStep", step);

		_jsonObect.bClassNameExist = false;

		//StringBuilder builder = new StringBuilder();
		//_jsonObect.Stringify (2, builder, false);
		//dicionary = null;
		return _jsonObect.Print(false);
	}

}

