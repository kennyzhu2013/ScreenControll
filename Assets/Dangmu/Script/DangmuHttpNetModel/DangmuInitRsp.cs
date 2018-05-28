using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

/// <summary>
/// Dangmu init rsponse.
/// </summary>
public class DangmuInitRsp 
{
	public int result;
	public uint lastId;
	public List<DangmuInitData> dataList = new List<DangmuInitData>();

	//data body
	public struct DangmuInitData
	{
		public uint indexId;
		public uint weixinId;
		public uint rid;

		public string openid;
		public string nickname;
		public string avatar; //用户头像url...
		public string content; //内容为文字或图片...

		public string type;
		//public uint status;
		public uint bp_time;

		public string createtime;
		//public string text; //
		//public string name;
		public string imageurl; //需要load...
	}

	//public const string CLASS_NAME="AwardRsp";
	//jsonData2["name"]
	public bool ParseJsonObject(JsonData _jsonObect)
	{
		//JSONObject(Dictionary<string, JSONObject> dic)
		//if (_jsonObect.className != CLASS_NAME)
		//return false;
		result = int.Parse(_jsonObect["resultCode"].ToString());//_jsonObect["ret"];
		//_jsonObect.GetField (ref Result, "ret");

		//data array partition...
		JsonData _jsonData = _jsonObect["data"];

		lastId = uint.Parse(_jsonData["lastid"].ToString());
		JsonData _jsonArray = _jsonData["list"];
		//JSONObject _objectListTemp = _jsonObect.GetField ("data");
		if ( null == _jsonArray ) {
			Debug.LogError ("DangmuInitRsp: data get failed!");
			return false;
		}

		//Get array list..
		foreach (JsonData item in _jsonArray)
		{
			DangmuInitData _jo = new DangmuInitData();
			uint itemp = uint.Parse(item["id"].ToString());
			_jo.indexId = itemp;
			itemp = uint.Parse(item["weid"].ToString());
			_jo.weixinId = itemp;
			itemp = uint.Parse(item["rid"].ToString());
			_jo.rid = itemp;

			_jo.openid = item["openid"].ToString();
			_jo.nickname = item["nickname"].ToString();
			_jo.avatar = item["avatar"].ToString();
			_jo.content = item["content"].ToString();

			//itemp = uint.Parse(item["type"].ToString());
			//_jo.type = itemp;
			_jo.type = item["type"].ToString();
			//itemp = uint.Parse(item["status"].ToString());
			//_jo.status = itemp;
			itemp = uint.Parse(item["bp_time"].ToString());
			_jo.bp_time = itemp;

			_jo.createtime = item["createtime"].ToString();
			//_jo.text = item["txt"].ToString();
			//_jo.name = item["name"].ToString();
			_jo.imageurl = item["image"].ToString();

			dataList.Add(_jo);
		}

		return true;
	}
}
