using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using UnityEngine.UI;

public class MainView : BaseView {
	public Button btnmin;
	public Button btnquit;

	// Use this for initialization
	// Use this for initialization
	public override void Start () {
		base.Start ();

		base.isTweenWin = true;
		//Icon max and min


		Transform closeOpen = transform.Find ("CloseOpen");
		/*if (null == btnmin) {
			btnmin = closeOpen.Find ("Minsize").GetComponent<Button> ();
		}
		btnmin.onClick.AddListener (delegate() {
			SCMainWinController.Instance ().Minsize ();
		});*/

		if (null == btnquit) {
			btnquit = closeOpen.Find ("Close").GetComponent<Button> ();
		}
		btnquit.onClick.AddListener( delegate() {
			SCMainWinController.Instance().Exit();
		});
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
