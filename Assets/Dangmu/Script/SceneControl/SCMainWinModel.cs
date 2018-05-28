using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCMainWinModel {
	[HideInInspector] public string qrcode;// url for qrcode...
	[HideInInspector] public int CurRid = -1; //当前活动id...,
	[HideInInspector] public uint WexinId; //关联的微信id...
	[HideInInspector] public uint IndexId; //当前播放到的活动id,每次取活动的indexid最大值...
	[HideInInspector] public string LoginTime; //
	[HideInInspector] public bool IsLogined = false; //login and inited...
	[HideInInspector] public string nickname;
	[HideInInspector] public bool IsMark;
	[HideInInspector] public string actName;
	[HideInInspector] public string Passwd;

	public void Init()
	{
		IsLogined = false;
		CurRid = -1;
		WexinId = 0;
		IsMark = true;
	}

	public void LoginRequest(string strRid, string strPasswd)
	{
		//登录接口没有情况下直接写死...
	}
}
