using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class TransparentSecondWindow : MonoBehaviour 
{
	[SerializeField]
	private Material m_Material;

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

	[DllImport("user32")]
	private static extern uint GetWindowLong(IntPtr hwnd, int nIndex);

	[DllImport("user32.dll")]
	private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);


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
	const int LWA_COLORKEY = 1;
	const int LWA_ALPHA = 2;

	// Definitions of window styles
	const int GWL_STYLE = (-16);
	const int GWL_EXSTYLE = (-20);
	//const int GWL_STYLE = -20; //-20 //-16.todo:-16不显示...
	const int HWND_TOPMOST = -1;
	const int HWND_NOTOPMOST = -2;

	const uint SWP_NOSIZE = 1; //{忽略 cx、cy, 保持大小}.
	const uint SWP_NOMOVE = 2; //{忽略 X、Y, 不改变位置}.
	const uint SWP_ASYNCWINDOWPOS = 0x4000;//{若调用进程不拥有窗口, 系统会向拥有窗口的线程发出需求}.
	const uint SWP_FRAMECHANGED = 0x0020;
	const uint SWP_SHOWWINDOW = 0x0040;

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

	int fWidth;
	int fHeight;

	public bool IsTransparent = false;
	//public MultiScreenWndProc SecondScreenProc;
	void Start()
	{
		//Screen.fullScreen = true;
		if (IsTransparent) 
		{
			#if !UNITY_EDITOR 
			//Screen.fullScreen = true;
			fWidth = Screen.width / 2;
			fHeight = Screen.height / 2;
			var margins = new MARGINS() { cxLeftWidth = -1,cxRightWidth = -1,cyTopHeight = -1,cyBottomHeight = -1 };

			// Get a handle to the window
			var hwnd = GetActiveWindow();

			//Remove title bar
			//SetLayeredWindowAttributes经测试必须是非child窗口(Overlapend或PoupUp)，否则设置无效！...
			uint lCurStyle = GetWindowLong(hwnd, GWL_STYLE);     // GWL_STYLE=-16
			//lCurStyle &= ~(WS_BORDER | WS_CAPTION | WS_SYSMENU | WS_MAXIMIZE);
			lCurStyle &= ( WS_POPUP | WS_VISIBLE); //
			SetWindowLong(hwnd, GWL_STYLE, lCurStyle);

			// Set properties of the window
			// See: https://msdn.microsoft.com/en-us/library/windows/desktop/ms633591%28v=vs.85%29.aspx
			// https://www.cnblogs.com/linyawen/archive/2011/01/17/1937863.html
			uint intExTemp = GetWindowLong(hwnd, GWL_EXSTYLE);
			SetWindowLong(hwnd, GWL_EXSTYLE, intExTemp | WS_EX_TRANSPARENT | WS_EX_LAYERED); // 
			//SetWindowLong(hwnd, GWL_EXSTYLE, GetWindowLong(hwnd, GWL_EXSTYLE) & ~WS_BORDER & ~WS_CAPTION);

			/*
			BOOL SetLayeredWindowAttributes(
			HWND hwnd, // 指定分层窗口句柄
			COLORREF crKey, // 指定需要透明的背景颜色值，可用RGB()宏
			BYTE bAlpha, // 设置透明度，0表示完全透明，255表示不透明
			DWORD dwFlags // 透明方式
			);
			其中，dwFlags参数可取以下值：
			LWA_ALPHA时：crKey参数无效，bAlpha参数有效；
			LWA_COLORKEY：窗体中的所有颜色为crKey的地方将变为透明，bAlpha参数无效。其常量值为1...
			LWA_ALPHA | LWA_COLORKEY：crKey的地方将变为全透明，而其它地方根据bAlpha参数确定透明度....
			*/
			//SetLayeredWindowAttributes(hwnd, 0, 255, LWA_ALPHA);
			//以下会整个不见...
			//SetLayeredWindowAttributes(hwnd, 0, 51, LWA_ALPHA);

			//窗口置顶必须的...
			SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, fWidth, fHeight, SWP_ASYNCWINDOWPOS | SWP_NOMOVE ); //| SWP_NOSIZE..
			//SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_FRAMECHANGED | SWP_SHOWWINDOW); //| SWP_NOSIZE..

			// Extend the window into the client area
			//See: https://msdn.microsoft.com/en-us/library/windows/desktop/aa969512%28v=vs.85%29.aspx 
			DwmExtendFrameIntoClientArea(hwnd, ref margins);

			/*
			private const int SW_HIDE = 0;
			private const int SW_NORMAL = 1;     //正常弹出窗体
			private const int SW_MAXIMIZE = 3;     //最大化弹出窗体
			private const int SW_SHOWNOACTIVATE = 4;
			private const int SW_SHOW = 5;
			private const int SW_MINIMIZE = 6;
			private const int SW_RESTORE = 9;
			private const int SW_SHOWDEFAULT = 10;
			*/
			//ShowWindow(hwnd, 1);
			//SetForegroundWindow(hwnd);

			//To delete..

			#endif
		}
		//SetSecondWindowTopMost();
	}

	public void SetWindowTranparent(IntPtr hwndSecond)
	{
		IsTransparent = true;
	}

	//for dangmu...
	public static void SetWindowsMouseDisable()
	{
		#if !UNITY_EDITOR
		// Get a handle to the window
		var hwnd = GetActiveWindow();

		// Set properties of the window
		// See: https://msdn.microsoft.com/en-us/library/windows/desktop/ms633591%28v=vs.85%29.aspx
		// https://www.cnblogs.com/linyawen/archive/2011/01/17/1937863.html
		uint intExTemp = GetWindowLong(hwnd, GWL_EXSTYLE);
		SetWindowLong(hwnd, GWL_EXSTYLE, intExTemp | WS_EX_TRANSPARENT); // 
		SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_ASYNCWINDOWPOS | SWP_NOSIZE);
		#endif
	}
	//::OnPaint()..

	// Pass the output of the camera to the custom material
	// for chroma replacement
	void OnRenderImage(RenderTexture from, RenderTexture to) 
	{
		if (IsTransparent)  {
			Graphics.Blit(from, to, m_Material);
		}
	}
}
