using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

/// <summary>
/// Login manager.负责登录逻辑控制...包括二维码图片获取和现实...
/// Login Controller..
/// </summary>
public class LoginModel {
	[HideInInspector] public int CurRid = -1; //当前活动id...,
	[HideInInspector] public uint WexinId; //关联的微信id...
	[HideInInspector] public uint IndexId; //当前播放到的活动id,每次取活动的indexid最大值...
	[HideInInspector] public string LoginTime; //
	[HideInInspector] public bool IsLogined = false; //login and inited...
	[HideInInspector] public string nickname;
	[HideInInspector] public string qrcode;
	public void Init()
	{
		IsLogined = false;
		CurRid = -1;
		WexinId = 0;

	}
	
	// Update is called once per frame
	void Update () {
		//null to do ....
	}

	/// <summary>
	/// Logins the request.
	/// 登录按钮回调...
	/// </summary>
	/// <param name="strRid">String rid.</param>
	/// <param name="strPasswd">String passwd.</param>
	public void LoginRequest(string strRid, string strPasswd)
	{
		//登录接口没有情况下直接写死...
	}
}
