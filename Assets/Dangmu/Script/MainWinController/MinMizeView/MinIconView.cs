using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using UnityEngine.UI;

public class MinIconView : BaseView {
	//public HitSecondTime hitSec;

	// Use this for initialization
	public override void Start () {
		base.Start ();

		//添加button事件...
		//HitSecondTime hitSec = gameObject.GetComponent<HitSecondTime>();
		//hitSec.DoubleClick = MinsizeController.Instance().MinMaxControl;

		Button btn = gameObject.GetComponent<Button>();
		btn.onClick.AddListener( delegate() {
			MinsizeController.Instance().MinMaxControl();
		});

		base.isTweenWin = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}
