using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

public class MultiScreenWndProc {
	private static List<IntPtr> processWndList = null;
	public delegate bool WNDENUMPROC(IntPtr hwnd, uint lParam);
	[DllImport("user32.dll", EntryPoint = "EnumWindows", SetLastError = true)]
	public static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, uint lParam);
	[DllImport("user32.dll", EntryPoint = "EnumChildWindows", SetLastError = true)]
	public static extern bool EnumChildWindows(IntPtr hwnd, WNDENUMPROC lpEnumFunc, uint lParam);

	[DllImport("user32.dll", EntryPoint = "GetParent", SetLastError = true)]
	public static extern IntPtr GetParent(IntPtr hWnd);
	[DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId")]
	public static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref uint lpdwProcessId);
	[DllImport("user32.dll", EntryPoint = "IsWindow")]
	public static extern bool IsWindow(IntPtr hWnd);
	[DllImport("kernel32.dll", EntryPoint = "SetLastError")]
	public static extern void SetLastError(uint dwErrCode);
	[DllImport("user32.dll")]    
	static extern IntPtr GetForegroundWindow ();
	[DllImport("user32.dll")]
	public static extern bool IsWindowVisible(IntPtr hWnd);
	[DllImport("user32.dll")]
	public static extern bool IsWindowEnabled(IntPtr hWnd);

	[DllImport("user32")]
	private static extern uint GetWindowLong(IntPtr hwnd, int nIndex);
	[DllImport("user32.dll")]
	private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
	[DllImport("user32")]
	private static extern int SetLayeredWindowAttributes(IntPtr hwnd,int crKey,int bAlpha,int dwFlags);
	[DllImport("user32.dll")]//, CharSet = CharSet.Auto..
	private static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int Width, int Height, uint flags);

	// Use this for initialization
	static MultiScreenWndProc () {
		if (processWndList == null)
		{
			processWndList = new List<IntPtr>();
		}

		//Todo...
	}

	/// <summary>
	/// Gets the second window handle.
	/// 获取第二屏窗口句柄...
	/// </summary>
	/// <returns>The second window handle.</returns>
	public static IntPtr GetSecondWindowHandle()
	{
		IntPtr ptrWnd = IntPtr.Zero;

		//object objWnd = processWnd[uiPid];
		if ( processWndList != null && processWndList.Count > 0 )
		{
			ptrWnd = processWndList[0];
			if ( ptrWnd != IntPtr.Zero )  // 从缓存中获取句柄
			{
				return ptrWnd;
			}
			else
			{
				ptrWnd = IntPtr.Zero;
			}
		}

		uint uiPid = (uint)Process.GetCurrentProcess().Id;  // 当前进程 ID
		bool bResult = EnumWindows( new WNDENUMPROC(EnumChildWindowCallBack), uiPid );

		// 枚举窗口返回 false 并且没有错误号时表明获取成功..
		if ( !bResult && processWndList.Count > 0 )//&& Marshal.GetLastWin32Error() == 0 )
		{
			ptrWnd = processWndList[0];
		}
		return ptrWnd;
	}

	private static bool EnumChildWindowCallBack(IntPtr hWnd, uint lParam)  
	{  
		uint dwPid = 0;  
		GetWindowThreadProcessId(hWnd, ref dwPid); // 获得找到窗口所属的进程 ...
		if(dwPid == lParam) // 判断是否是目标进程的窗口...  
		{  
			//
			//MyDebug.Add ("MultiScreenWndProc:", hWnd.ToString()); // 输出窗口信息...  
			//判断窗体是否激活...
			if ( hWnd != IntPtr.Zero && IsWindow (hWnd) && hWnd != GetForegroundWindow() 
				&& IsWindowVisible(hWnd) && IsWindowEnabled(hWnd) ) {
				Log.info ("EnumChildWindowCallBack", "Get hwnd name:" + hWnd);
				//MyDebug.Add ("EnumChildWindowCallBack" + processWndList.Count, "Get hwnd name:" + hWnd);
				processWndList.Add (hWnd);
			}
			//processWnd[uiPid] = hWnd;   // 把句柄缓存起来
			SetLastError(0);    // 设置无错误

			//获取窗口内容...
			/*
			TCHAR buf[WINDOW_TEXT_LENGTH];  
			SendMessage(hWnd, WM_GETTEXT, WINDOW_TEXT_LENGTH, (LPARAM)buf);  
			wprintf(L"%s/n", buf);*/
			EnumChildWindows(hWnd, EnumChildWindowCallBack, lParam);    // 递归查找子窗口  
		}

		return true;
	}

	/*
	private static bool EnumWindowCallBack(IntPtr hWnd, uint lParam)  
	{  
		DWORD dwPid = 0;  
		GetWindowThreadProcessId(hWnd, &dwPid); // 获得找到窗口所属的进程  
		if(dwPid == lParam) // 判断是否是目标进程的窗口  
		{  
			Log.debug(this, hWnd.ToString()); // 输出窗口信息...

			EnumChildWindows(hWnd, EnumChildWindowCallBack, lParam);    // 继续查找子窗口  
		}  
		return true;  
	}*/
	
	// Update is called once per frame
	void Update () {
		
	}
}
