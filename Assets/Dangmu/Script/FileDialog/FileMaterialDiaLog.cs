using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;
using System.Runtime.InteropServices;
using System.Threading;

/// <summary>
/// File material dialog. for canvas-bottom
/// controlled by main window...
/// MVC single...
/// </summary>
public class FileMaterialDiaLog: BaseView {
	private FileTypeTabSwitch[] tabArray;
	private FileDialog fileDialog;
	public Button closeBtn;
	private WebFileDialog webfileDialog;

	[Header("视频面板")]
	public GameObject videoObject;

	[Header("网页面板")]
	public GameObject webObject;

	[Header("网页选项")]
	public WebFileDialog webOption = new WebFileDialog ();

	// Use this for initialization
	void Start () {
		base.Start ();
		fileDialog = new FileDialog ();
		fileDialog.merialDlg = this;
		fileDialog.objectThread.BUnityWorkMutex = false;
		fileDialog.objectThread.UnityFullpath = UnityEngine.Application.dataPath;
		webOption.merialDlg = this;


		tabArray = GetComponentsInChildren<FileTypeTabSwitch> ();
		for (int i = 0; i < tabArray.Length; ++i) {
			tabArray [i].MgrDlg = this;
		}
		isTweenWin = true;

		//http://blog.csdn.net/teng_ontheway/article/details/47188141..
		//Button closeBtn = transform.Find ("MaterialWindow/Canvas/Header/CloseBtn").GetComponent<Button>();
		closeBtn.onClick.AddListener( delegate() {
			this.Hide ();
		});
	}
	
	// Update is called once per frame
	void Update () {
		if ( fileDialog.objectThread.BUnityWorkMutex ) {
			lock ( fileDialog.objectThread ) {
				fileDialog.objectThread.BUnityWorkMutex = false;
				VideoPreviewer.Instance ().StartPreview (fileDialog.objectThread.UnityFullpath, fileDialog.PreViewDone);
				fileDialog.merialDlg.Hide ();
			}
		}
	}

	public void OnClickFileType(FileItem.FileType filetype)
	{
		FileItem.FileType lastType = FileListControl.Instance ().SelectedType;
		FileListControl.Instance ().SelectedType = filetype;
		for (int i = 0; i < tabArray.Length; ++i) {
			if (tabArray [i].btnTpye == filetype) {
				tabArray [i].HighColor ();
			}

			if (tabArray [i].btnTpye == lastType) {
				tabArray [i].ReSetColor ();
			}
		}

		if (filetype == FileItem.FileType.FileType_Web) {
			webObject.SetActive (true);
			videoObject.SetActive (false);
		}
		else if (filetype == FileItem.FileType.FileType_Video) {
			webObject.SetActive (false);
			videoObject.SetActive (true);
		}
	}

	public void OnClickVideoSelect() 
	{
		fileDialog.objectThread.UnityFullpath = UnityEngine.Application.dataPath;
		Thread th1 = new Thread( fileDialog.VideoFileSelect );    
		//StartCoroutine(CoroutineSelect());
		th1.Start();
		//fileDialog.VideoFileSelect ();
	}

	public void OnClickWebSelect()
	{
		webOption.WebUrlConfirm ();
	}
}
