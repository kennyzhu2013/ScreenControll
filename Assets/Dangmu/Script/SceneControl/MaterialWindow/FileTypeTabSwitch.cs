using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileTypeTabSwitch : MonoBehaviour {
	[Header("素材类型")]
	public FileItem.FileType btnTpye;

	[HideInInspector]
	public FileMaterialDiaLog MgrDlg;
	private Button btn;
	private Image bkgGrd;

	public Color selectColor = Color.green;

	// Use this for initialization
	void Start () {
		btn = GetComponent<Button> ();
		btn.onClick.AddListener (delegate { 
			MgrDlg.OnClickFileType(btnTpye); });

		bkgGrd = GetComponent<Image> ();
	}

	//write dead..
	public void HighColor () {
		bkgGrd.color = selectColor;
	}

	public void ReSetColor () {
		bkgGrd.color = Color.black;
	}
}
