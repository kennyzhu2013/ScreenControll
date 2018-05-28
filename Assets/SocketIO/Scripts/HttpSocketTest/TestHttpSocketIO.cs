#region License
/*
 * TestSocketIO.cs
 *
 * The MIT License
 *
 * Copyright (c) 2016 kennyzhu
 * for China-mobile interface...
 *
 */
using System.Collections.Generic;
using System.Text;
using HttpIO;
using UnityEngine.UI;

#endregion

using System.Collections;
using UnityEngine;
//using SocketIO;

public class TestHttpSocketIO : MonoBehaviour
{
	public Text mobile;
	public Text activeId;
	public Text activityStep;
	public Text output;

	private HttpIOComponent socket;

	public void Start() 
	{
		GameObject go = GameObject.Find("SocketIO");
		socket = go.GetComponent<HttpIOComponent>();
		mobile.text = "13802885090";
		activeId.text = "9900002";
		activityStep.text = "2";
		
		//socket.On("open", TestOpen);
		//StartCoroutine("AwardBoop");
	}

	//award loop ....
	private IEnumerator AwardBoop()
	{
		// wait 1 seconds and continue
		yield return new WaitForSeconds(1);
		AwardReq req = new AwardReq ();
		req.activityId = 9900002;
		req.mobile = "13802881234";
		socket.EmitRawMessage (req.ToJsonStirng ());

		//socket.EmitBet("{GameAccountRegisterReq: {Name: 'leaftest6',Password: 'leaftest6'}}");

		// wait 3 seconds and continue
		yield return new WaitForSeconds(5);

		//wait for response
		JSONObject resObj = socket.GetResponseJsonObject();
		if (resObj == null) {
			Debug.Log ("[SocketIO] AwardBoop null, no response! ");
			yield return null;
			yield break;
		}

		//while( resObj == null ) resObj = socket.GetResponseJsonObject();
		AwardRsp rsp = new AwardRsp();
		rsp.ParseJsonObject (resObj);
		Debug.Log ("[SocketIO] AwardBoop resultCode: "+ rsp.Result);


		//for prize get.....................................................................
		socket.path = "draw/draw.do";
		AwardGetReq reqGet = new AwardGetReq ();
		reqGet.activityId = 9900002;
		reqGet.mobile = "13802881234";
		socket.EmitRawMessage (reqGet.ToJsonStirng ());

		// wait 5 seconds and continue
		yield return new WaitForSeconds(5);

		//wait for response
		resObj = socket.GetResponseJsonObject();
		if (resObj == null) {
			Debug.Log ("[SocketIO] AwardBoop null, no response! ");
			yield return null;
			yield break;
		}

		//while( resObj == null ) resObj = socket.GetResponseJsonObject();
		AwardGetRsp rspGet = new AwardGetRsp();
		rspGet.ParseJsonObject (resObj);
		Debug.Log ("[SocketIO] AwardBoop resultCode: "+ rspGet.Result);

		// wait ONE FRAME and continue
		//yield return null;

	}



	public void TestBoop(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Boop received: " + e.name + " " + e.data);

		if (e.data == null) { return; }

		Debug.Log(
			"#####################################################" +
			"THIS: " + e.data.GetField("this").str +
			"#####################################################"
		);
	}

	private IEnumerator PrizeChanceBoop()
	{
		AwardReq req = new AwardReq ();
		req.activityId = int.Parse(activeId.text);
		req.mobile = mobile.text;
		req.step = int.Parse (activityStep.text);
		socket.EmitRawMessage (req.ToJsonStirng ());

		//socket.EmitBet("{GameAccountRegisterReq: {Name: 'leaftest6',Password: 'leaftest6'}}");

		// wait 3 seconds and continue
		output.text = "Please wait for 3 seconds......";
		yield return new WaitForSeconds(3);

		//wait for response
		JSONObject resObj = socket.GetResponseJsonObject();
		while (resObj == null) {
			Debug.Log ("[SocketIO] AwardBoop null, no response! ");
			output.text = "[SocketIO] AwardBoop null, no response!";
			resObj = socket.GetResponseJsonObject();
			yield return new WaitForSeconds(1);
			//yield return null;
			//yield break;
		}


		//while( resObj == null ) resObj = socket.GetResponseJsonObject();
		AwardRsp rsp = new AwardRsp();
		rsp.ParseJsonObject (resObj);
		Debug.Log ("[SocketIO] AwardBoop resultCode: "+ rsp.Result);
		output.text = "[SocketIO] AwardBoop resultCode: " + rsp.Result;
	}

	public void GetPrizeChance()
	{
		StartCoroutine("PrizeChanceBoop");
	}


	private IEnumerator PrizeBoop()
	{
		//for prize get.....................................................................
		socket.path = "draw/draw.do";
		AwardGetReq reqGet = new AwardGetReq ();
		reqGet.activityId = int.Parse(activeId.text);
		reqGet.mobile = mobile.text;
		reqGet.step = int.Parse (activityStep.text);
		socket.EmitRawMessage (reqGet.ToJsonStirng ());

		// wait 3 seconds and continue
		output.text = "Please wait for 3 seconds......";
		yield return new WaitForSeconds(1);

		//wait for response
		JSONObject resObj = socket.GetResponseJsonObject();
		while (resObj == null) { //wait for all the time...
			Debug.Log ("[SocketIO] AwardBoop null, no response! ");
			output.text = "[SocketIO] AwardBoop null, no response!";
			resObj = socket.GetResponseJsonObject();
			yield return new WaitForSeconds(1);
			//yield return null;
			//yield break;
		}

		//while( resObj == null ) resObj = socket.GetResponseJsonObject();
		AwardGetRsp rspGet = new AwardGetRsp();
		rspGet.ParseJsonObject (resObj);
		Debug.Log ("[SocketIO] AwardBoop resultCode: "+ rspGet.Result);
		output.text = "[SocketIO] AwardBoop resultCode: " + rspGet.Result;
		if (0 == rspGet.Result) {
			output.text += " awardId:" + rspGet.awardId;
			output.text += " awardLevel:" + rspGet.awardLevel;
			output.text += " awardName:" + rspGet.awardName;
			output.text += " awardType:" + rspGet.awardType;
		}
	}


	public void GetPrize()
	{
		StartCoroutine("PrizeBoop");
	}
}
