using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class testMultiScreen : MonoBehaviour {
	[System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "MoveWindow")]
	public static extern bool MoveWindow(System.IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

	// Use this for initialization
	void Start () {
		UnityEngine.Debug.Log("displays connected: " + Display.displays.Length);
		// Display.displays[0] 是主显示器, 默认显示并始终在主显示器上显示.   ...     
		// 检查其他显示器是否可用并激活.        
		if (Display.displays.Length > 1)
			Display.displays[1].Activate();
		if (Display.displays.Length > 2)
			Display.displays[2].Activate();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Button1Click()
	{
		//Process.Start(“C:\Program Files\Tencent\QQ\Bin\QQ.exe”);
		Process pTemp = Process.Start("IExplore.exe", "http://www.baidu.com/");
		pTemp.WaitForInputIdle ();

		//TODO: not effect, how to do...
		//use BroswerGUI?
		MoveWindow (pTemp.Handle, 0, 0, 800, 600, true);
	}
}
