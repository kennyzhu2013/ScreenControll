using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;  
using System;  
using System.Runtime.InteropServices;  

[ StructLayout( LayoutKind.Sequential, CharSet=CharSet.Auto )]
public class OpenFileName {

	public int      structSize = 0;  
	public IntPtr   dlgOwner = IntPtr.Zero;   
	public IntPtr   instance = IntPtr.Zero;  
	public String   filter = null;  
	public String   customFilter = null;  
	public int      maxCustFilter = 0;  
	public int      filterIndex = 0;  
	public String   file = null;  
	public int      maxFile = 0;  
	public String   fileTitle = null;  
	public int      maxFileTitle = 0;  
	public String   initialDir = null;  
	public String   title = null;     
	public int      flags = 0;   
	public short    fileOffset = 0;  
	public short    fileExtension = 0;  
	public String   defExt = null;   
	public IntPtr   custData = IntPtr.Zero;    
	public IntPtr   hook = IntPtr.Zero;    
	public String   templateName = null;   
	public IntPtr   reservedPtr = IntPtr.Zero;   
	public int      reservedInt = 0;  
	public int      flagsEx = 0;  
}  

[System.Serializable]
public class OpenFileNameEx {

	public String   file = null;  
	public int      maxFile = 0;  
	public String   fileTitle = null;  
	public int      maxFileTitle = 0;  
	public String   initialDir = null;  
	public String   title = null;     
	public int      flags = 0;   
	public short    fileOffset = 0;  
	public short    fileExtension = 0;  
	public String   defExt = null;   
	public String   templateName = null;

	public OpenFileNameEx()
	{
		file = null;  
		maxFile = 0;  
		fileTitle = null;  
		maxFileTitle = 0;  
		initialDir = null;  
		title = null;     
		flags = 0;   
		fileOffset = 0;  
		fileExtension = 0;  
		defExt = null;   
		templateName = null;
	}

	public OpenFileNameEx(OpenFileName fileinfo)
	{
		this.file          = fileinfo.file;  
		this.maxFile       = fileinfo.maxFile;  
		this.fileTitle     = fileinfo.fileTitle;  
		this.maxFileTitle  = fileinfo.maxFileTitle;  
		this.initialDir    = fileinfo.initialDir;  
		this.title         = fileinfo.title;     
		this.flags         = fileinfo.flags;   
		this.fileOffset    = fileinfo.fileOffset;  
		this.fileExtension = fileinfo.fileExtension;  
		this.defExt        = fileinfo.defExt;   
		this.templateName  = fileinfo.templateName;
	}
}  

public class DllComdlg  
{  
	[DllImport("Comdlg32.dll",SetLastError=true,ThrowOnUnmappableChar=true, CharSet = CharSet.Auto)]            
	public static extern bool GetOpenFileName([ In, Out ] OpenFileName ofn );     
	public static  bool GetOpenFileName1([ In, Out ] OpenFileName ofn )
	{  
		return GetOpenFileName(ofn);  
	}  
}
