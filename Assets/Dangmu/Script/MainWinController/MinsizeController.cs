using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinsizeController : DMonoSingleton<MinsizeController> {
	private Transform iconWin;
	public MinIconView MinIconVw;

	//for double click
	private float doubleClickGap = 0.25f;
	private float doubleClickCouter = 0f;
	private int   clickCount = 0;

	// Use this for initialization
	void Start () {
		if ( null == instance )
			instance = this;
		
		iconWin = transform.Find("Icon");
		MinIconVw = GetComponentInChildren<MinIconView> (true);

		//Register window...
		//WindowsManager..
		SCWindowsManager.Instance.addWin (typeof(MinIconView), MinIconVw);
	}
	
	// Update is called once per frame
	void Update () {
		if (clickCount > 0) {
			doubleClickCouter += Time.deltaTime;
		}
	}

	/// <summary>
	/// Minisizes or maxsizes control.
	/// TODO: event based..
	/// </summary>
	public void MinMaxControl()
	{
		++clickCount;
		if (clickCount >= 2) {
			if (doubleClickCouter < doubleClickGap) {
				//close icon and open
				Debug.LogWarning ("MinMaxControl: restore window!");
				//WindowsManager
				SCWindowsManager.Instance.OpenAndCloseWin (typeof(MainView), typeof(MinIconView));
				return;
			}
		}

		clickCount = 1;
		doubleClickCouter = 0f;
	}
}
