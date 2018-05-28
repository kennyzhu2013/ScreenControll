using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class ResizableWindow : DMonoSingleton<ResizableWindow> {

	private struct MARGINS
	{
		public int cxLeftWidth;
		public int cxRightWidth;
		public int cyTopHeight;
		public int cyBottomHeight;
	}

	// Define function signatures to import from Windows APIs

	[DllImport("user32.dll")]
	private static extern IntPtr GetActiveWindow();

	[DllImport("user32.dll")]    
	static extern IntPtr GetForegroundWindow ();  

	[DllImport("user32")]
	private static extern uint GetWindowLong(IntPtr hwnd, int nIndex);

	[DllImport("user32.dll")]
	private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

	[StructLayout(LayoutKind.Sequential)]
	public struct RECT
	{
		public int Left;                             //最左坐标
		public int Top;                             //最上坐标
		public int Right;                           //最右坐标
		public int Bottom;                        //最下坐标
	}
	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);


	[DllImport("user32.dll")]//, CharSet = CharSet.Auto..
	private static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int Width, int Height, uint flags);


	[DllImport("Dwmapi.dll")]
	private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

	[DllImport("user32.dll")]
	private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

	[DllImport("User32.dll")]  
	private static extern bool SetForegroundWindow(IntPtr hWnd);

	[DllImport("user32")]
	private static extern int SetLayeredWindowAttributes(IntPtr hwnd,int crKey,int bAlpha,int dwFlags);

	[DllImport("user32.dll")]  
	public static extern bool ReleaseCapture();  
	[DllImport("user32.dll")]  
	public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);  
	//const int LWA_COLORKEY = 1;
	//const int LWA_ALPHA = 2;

	// Definitions of window styles
	const int GWL_STYLE = (-16);
	const int GWL_EXSTYLE = (-20);
	//const int GWL_STYLE = -20; //-20 //-16.todo:-16不显示...
	const int HWND_TOPMOST = -1;

	const uint SWP_NOSIZE = 1; //{忽略 cx、cy, 保持大小}.
	const uint SWP_NOMOVE = 2; //{忽略 X、Y, 不改变位置}.
	const uint SWP_ASYNCWINDOWPOS = 0x4000;//{若调用进程不拥有窗口, 系统会向拥有窗口的线程发出需求}.
	const uint SWP_FRAMECHANGED = 0x0020;
	const uint SWP_SHOWWINDOW = 0x0040;

	const int  SW_SHOWMINIMIZED   = 2; //{最小化, 激活}  
	const int  SW_SHOWMAXIMIZED   = 3; //{最大化, 激活}  

	const uint WS_BORDER = 0x00800000;
	const uint WS_CAPTION = 0x00C00000;
	const uint WS_SYSMENU = 0x00080000;
	const uint WS_MAXIMIZE = 0x01000000;

	//lCurStyle &= e;
	const uint WS_CHILD = 0x40000000;
	const uint WS_POPUP = 0x80000000;
	const uint WS_VISIBLE = 0x10000000;

	const uint WS_EX_LAYERED = 0x80000;
	const uint WS_EX_TRANSPARENT = 0x00000020;
	const uint WS_EX_TOPMOST = 0x00000008;

	int fWidth = 1920;
	int fHeight = 1080;
	int posX = 0;
	int poxY = 0;
	IntPtr hwnd;  
	float xx;  
	bool bx;
	// Use this for initialization
	void Start () {
		if (null == instance)
			instance = this;
		
		bx = false;  
		xx = 0f;
		//CenterWindow ();

		#if !UNITY_EDITOR
		// Get a handle to the window
		hwnd = GetActiveWindow();
		fWidth = Screen.width / 2 + Screen.width / 3;
		fHeight = Screen.height / 2 + Screen.height / 3;
		posX = Screen.width / 2 - fWidth / 2;
		poxY = Screen.height / 2 - fHeight / 2;
		//Remove title bar
		//SetLayeredWindowAttributes经测试必须是非child窗口(Overlapend或PoupUp)，否则设置无效！...
		SetWindowLong(hwnd, GWL_STYLE, WS_POPUP); //whether to delete...

		SetWindowPos(hwnd, 0, posX, poxY, fWidth, fHeight, SWP_FRAMECHANGED | SWP_SHOWWINDOW );// 设置屏大小和显示位置

		#endif
	}
	/*
	public void DragWindow(Vector2 offsetPos) {
		int posX = System.Convert.ToInt32(offsetPos.x);
		int poxY = System.Convert.ToInt32(offsetPos.y);
		#if !UNITY_EDITOR
		RECT rect = new RECT();
		var hwnd = GetActiveWindow();
		GetWindowRect(hwnd, ref rect);
		posX += rect.Left;
		poxY += rect.Top;
		SetWindowPos(hwnd, 0, posX, poxY, 0, 0, SWP_FRAMECHANGED | SWP_SHOWWINDOW | SWP_NOSIZE );// 设置屏大小和显示位置

		#endif
	}*/

	public void SetBx(bool bxValue) {
		xx =0f;
		bx=true;
	}

	// Update is called once per frame
	void Update () {
		#if UNITY_STANDALONE_WIN  
		/*
		if (Input.GetMouseButtonDown (0)) {
			xx =0f;
			bx=true;  
		}*/

		if(bx && xx >= 0.2f ){ //这样做为了区分界面上面其它需要滑动的操作  
			ReleaseCapture();   
			SendMessage(hwnd, 0xA1, 0x02, 0);   
			SendMessage(hwnd, 0x0202, 0, 0);  
		}

		if(bx)  
			xx +=Time.deltaTime;
		
		if(Input.GetMouseButtonUp(0)){  
			xx =0f;
			bx=false;
		}
		#endif   
	}

	public Image maxRestImage;
	public Sprite restoreSprite;
	public Sprite maxSprite;
	private bool bMax = false;
	public TransparentSecondWindow TWindow;

	//Todo:
	public void btn_onclick(){ //最小化   
		#if !UNITY_EDITOR
		ShowWindow(GetForegroundWindow(), SW_SHOWMINIMIZED );
		#endif
	}

	public void btn_onclickxx(){ //最大化  
		if (bMax) {
			maxRestImage.sprite = maxSprite;
			#if !UNITY_EDITOR
			// Get a handle to the window
			hwnd = GetForegroundWindow();
			SetWindowPos( hwnd, 0, posX, poxY, fWidth, fHeight, SWP_FRAMECHANGED | SWP_NOMOVE );// 设置屏大小和显示位置

			//ReleaseCapture();
			//SendMessage(hwnd, 0xA1, 0x02, 0);   
			//SendMessage(hwnd, 0x0202, 0, 0);
			#endif
			bMax = false;
		}
		else {
			maxRestImage.sprite = restoreSprite;
			#if !UNITY_EDITOR
			//ReleaseCapture();
			//SendMessage(hwnd, 0xA1, 0x02, 0);   
			//SendMessage(hwnd, 0x0202, 0, 0);
			ShowWindow(hwnd, SW_SHOWMAXIMIZED);
			//ReleaseCapture();
			//SendMessage(hwnd, 0xA1, 0x02, 0);   
			//SendMessage(hwnd, 0x0202, 0, 0);
			//ReleaseCapture(); 
			#endif

			bMax = true;
		}
	}  

	/// <summary>
	/// Sets the size of the window.
	/// </summary>
	/// <param name="sizeVec">Size vec.</param>
	public void SetWindowSize(int iWidth, int iHeight) {
		#if !UNITY_EDITOR
		hwnd = GetForegroundWindow();
		SetWindowPos( hwnd, 0, posX, poxY, iWidth, iHeight, SWP_FRAMECHANGED | SWP_NOMOVE );// 设置屏大小和显示位置

		//ReleaseCapture();
		//SendMessage(hwnd, 0xA1, 0x02, 0);   
		//SendMessage(hwnd, 0x0202, 0, 0);
		#endif
	}

	public IEnumerator SetSecondWindowTopMost() {
		var hwndSecond = MultiScreenWndProc.GetSecondWindowHandle ();
		while (hwndSecond == IntPtr.Zero) {
			hwndSecond = MultiScreenWndProc.GetSecondWindowHandle ();
			yield return 1;
		}
			
		//MyDebug.Add ("MainWnd", "GetSecondWindowHandle:" + hwnd);
		//MyDebug.Add ("SetSecondWindowTopMost", "GetSecondWindowHandle:" + hwndSecond);

		#if !UNITY_EDITOR
		var margins = new MARGINS() { cxLeftWidth = -1,cxRightWidth = -1,cyTopHeight = -1,cyBottomHeight = -1 };

		uint lCurStyle = GetWindowLong(hwndSecond, GWL_STYLE);     // GWL_STYLE=-16
		lCurStyle &= ( WS_POPUP | WS_VISIBLE); // WS_POPUP | 
		SetWindowLong(hwndSecond, GWL_STYLE, lCurStyle);

		uint intExTemp = GetWindowLong(hwndSecond, GWL_EXSTYLE);
		SetWindowLong(hwndSecond, GWL_EXSTYLE, intExTemp | WS_EX_TRANSPARENT | WS_EX_LAYERED);
		//SetWindowLong(hwndSecond, GWL_EXSTYLE, WS_EX_TRANSPARENT | WS_EX_LAYERED); 

		SetWindowPos(hwndSecond, HWND_TOPMOST, 0, 0, fWidth, fHeight, SWP_FRAMECHANGED | SWP_NOSIZE | SWP_NOMOVE); //SWP_FRAMECHANGED = 0x0020 (32); //SWP_SHOWWINDOW = 0x0040 (64)
		DwmExtendFrameIntoClientArea(hwndSecond, ref margins);

		//ShowWindow(hwndSecond, 3);
		//SetForegroundWindow(hwndSecond);
		TWindow.IsTransparent = true;
		#endif

		yield return 0;
	}
}
