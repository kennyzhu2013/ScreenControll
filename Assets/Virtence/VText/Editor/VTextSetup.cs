/*
 * $Id: VTextSetup.cs 225 2015-04-29 13:56:16Z dirk $
 *
 * Virtence VText package
 * Copyright 2015 by Virtence GmbH
 * http://www.virtence.com
 * 
 */

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class VTextSetup : EditorWindow
{
    [MenuItem("Window/Virtence/VText Setup...")]
    static void Init()
    {
        VTextSetup window = EditorWindow.GetWindow(typeof(VTextSetup)) as VTextSetup;
        window.Show();
    }

    Vector2 logScroll = Vector2.zero;
    bool checkDone = false;

    bool streamingAssetsExists = false;
    bool streamingAssetsFontsExists = false;
#if UNITY_5
#else
    bool pluginBaseExists = false;
#endif
    int numFonts = 0;
    string logString = "";

#if UNITY_5
    // No plugin copy required
#else
    // plugins installed
    bool _plugWin32 = false;
    bool _plugWin64 = false;
    bool _plugLinux32 = false;
    bool _plugLinux64 = false;
    bool _plugOSX = false;
    bool _plugIOS = false;
    bool _plugAndroid = false;
#endif

#if UNITY_5
    // No plugin copy required
#else
    private bool SetupPlugin(string name, bool install)
    {
        string pluginBase = System.IO.Path.Combine(Application.dataPath, "Plugins");
        string vBase = System.IO.Path.Combine(Application.dataPath, "Virtence/VText/Plugins");
        string dst = "";
        string src = "";
        DirectoryInfo di;
        if (install)
        {
            switch (name)
            {
                case "Android":
                    dst = System.IO.Path.Combine(pluginBase, "Android");
                    di = Directory.CreateDirectory(dst);
                    Debug.Log(di.Name + " " + di.Exists);
                    if (di.Exists)
                    {
                        src = System.IO.Path.Combine(vBase, "Android/libVText.so");
                        dst = System.IO.Path.Combine(dst, "libVText.so");
                        Debug.Log("copy " + src + " -> " + dst);
                        FileUtil.CopyFileOrDirectory(src, dst);
                        AssetDatabase.Refresh();
                        return true;
                    }
                    break;
                case "iOS":
                    dst = System.IO.Path.Combine(pluginBase, "iOS");
                    di = Directory.CreateDirectory(dst);
                    Debug.Log(di.Name + " " + di.Exists);
                    if (di.Exists)
                    {
                        src = System.IO.Path.Combine(vBase, "iOS/libVTextIOS.a");
                        dst = System.IO.Path.Combine(dst, "libVText.a");
                        Debug.Log("copy " + src + " -> " + dst);
                        FileUtil.CopyFileOrDirectory(src, dst);
                        AssetDatabase.Refresh();
                        return true;
                    }
                    break;
                case "OSX":
                    dst = System.IO.Path.Combine(pluginBase, "VText.bundle");
                    di = Directory.CreateDirectory(dst);
                    Debug.Log(di.Name + " " + di.Exists);
                    if (di.Exists)
                    {
                        dst = System.IO.Path.Combine(dst, "Contents");
                        di = Directory.CreateDirectory(dst);
                        if (di.Exists)
                        {
                            string d1 = System.IO.Path.Combine(dst, "MacOS");
                            di = Directory.CreateDirectory(d1);
                            if (di.Exists)
                            {
                                string d2 = System.IO.Path.Combine(dst, "Resources");
                                di = Directory.CreateDirectory(d2);
                                if (di.Exists)
                                {

                                    src = System.IO.Path.Combine(vBase, "VText.bundle/Contents/Info.plist");
                                    dst = System.IO.Path.Combine(dst, "Info.plist");
                                    Debug.Log("copy " + src + " -> " + dst);
                                    FileUtil.CopyFileOrDirectory(src, dst);

                                    src = System.IO.Path.Combine(vBase, "VText.bundle/Contents/MacOS/VText");
                                    dst = System.IO.Path.Combine(d1, "VText");
                                    Debug.Log("copy " + src + " -> " + dst);
                                    FileUtil.CopyFileOrDirectory(src, dst);

                                    src = System.IO.Path.Combine(vBase, "VText.bundle/Contents/Resources/VTextProj.xcconfig");
                                    dst = System.IO.Path.Combine(d2, "VTextProj.xcconfig");
                                    Debug.Log("copy " + src + " -> " + dst);
                                    FileUtil.CopyFileOrDirectory(src, dst);

                                    src = System.IO.Path.Combine(vBase, "VText.bundle/Contents/Resources/VTextTarget.xcconfig");
                                    dst = System.IO.Path.Combine(d2, "VTextTarget.xcconfig");
                                    Debug.Log("copy " + src + " -> " + dst);
                                    FileUtil.CopyFileOrDirectory(src, dst);

                                    AssetDatabase.Refresh();
                                    return true;
                                }
                            }
                        }
                    }
                    break;
                case "L32":
                    dst = System.IO.Path.Combine(pluginBase, "x86");
                    di = new DirectoryInfo(dst);
                    if (!di.Exists)
                    {
                        di = Directory.CreateDirectory(dst);
                    }
                    Debug.Log(di.Name + " " + di.Exists);
                    if (di.Exists)
                    {
                        src = System.IO.Path.Combine(vBase, "x86/libVText.so");
                        dst = System.IO.Path.Combine(dst, "libVText.so");
                        Debug.Log("copy " + src + " -> " + dst);
                        FileUtil.CopyFileOrDirectory(src, dst);
                        AssetDatabase.Refresh();
                        return true;
                    }
                    break;
                case "L64":
                    dst = System.IO.Path.Combine(pluginBase, "x86_64");
                    di = new DirectoryInfo(dst);
                    if (!di.Exists)
                    {
                        di = Directory.CreateDirectory(dst);
                    }
                    Debug.Log(di.Name + " " + di.Exists);
                    if (di.Exists)
                    {
                        src = System.IO.Path.Combine(vBase, "x86_64/libVText.so");
                        dst = System.IO.Path.Combine(dst, "libVText.so");
                        Debug.Log("copy " + src + " -> " + dst);
                        FileUtil.CopyFileOrDirectory(src, dst);
                        AssetDatabase.Refresh();
                        return true;
                    }
                    break;
                case "W32":
                    dst = System.IO.Path.Combine(pluginBase, "x86");
                    di = new DirectoryInfo(dst);
                    if (!di.Exists)
                    {
                        di = Directory.CreateDirectory(dst);
                    }
                    Debug.Log(di.Name + " " + di.Exists);
                    if (di.Exists)
                    {
                        src = System.IO.Path.Combine(vBase, "x86/VText.dll");
                        dst = System.IO.Path.Combine(dst, "VText.dll");
                        Debug.Log("copy " + src + " -> " + dst);
                        FileUtil.CopyFileOrDirectory(src, dst);
                        AssetDatabase.Refresh();
                        return true;
                    }
                    break;
                case "W64":
                    dst = System.IO.Path.Combine(pluginBase, "x86_64");
                    di = new DirectoryInfo(dst);
                    if (!di.Exists)
                    {
                        di = Directory.CreateDirectory(dst);
                    }
                    Debug.Log(di.Name + " " + di.Exists);
                    if (di.Exists)
                    {
                        src = System.IO.Path.Combine(vBase, "x86_64/VText.dll");
                        dst = System.IO.Path.Combine(dst, "VText.dll");
                        Debug.Log("copy " + src + " -> " + dst);
                        FileUtil.CopyFileOrDirectory(src, dst);
                        AssetDatabase.Refresh();
                        return true;
                    }
                    break;
            }
        }
        else
        {
            switch (name)
            {
                case "Android":
                    dst = System.IO.Path.Combine(pluginBase, "Android");
                    Debug.Log("delete " + dst);
                    FileUtil.DeleteFileOrDirectory(dst);
                    AssetDatabase.Refresh();
                    return false;
                case "iOS":
                    dst = System.IO.Path.Combine(pluginBase, "iOS");
                    Debug.Log("delete " + dst);
                    FileUtil.DeleteFileOrDirectory(dst);
                    AssetDatabase.Refresh();
                    return false;
                case "OSX":
                    dst = System.IO.Path.Combine(pluginBase, "VText.bundle");
                    Debug.Log("delete " + dst);
                    FileUtil.DeleteFileOrDirectory(dst);
                    AssetDatabase.Refresh();
                    return false;
                case "L32":
                    dst = System.IO.Path.Combine(System.IO.Path.Combine(pluginBase, "x86"), "libVText.so");
                    Debug.Log("delete " + dst);
                    FileUtil.DeleteFileOrDirectory(dst);
                    AssetDatabase.Refresh();
                    return false;
                case "L64":
                    dst = System.IO.Path.Combine(System.IO.Path.Combine(pluginBase, "x86_64"), "libVText.so");
                    Debug.Log("delete " + dst);
                    FileUtil.DeleteFileOrDirectory(dst);
                    AssetDatabase.Refresh();
                    return false;
                case "W32":
                    dst = System.IO.Path.Combine(System.IO.Path.Combine(pluginBase, "x86"), "VText.dll");
                    Debug.Log("delete " + dst);
                    FileUtil.DeleteFileOrDirectory(dst);
                    AssetDatabase.Refresh();
                    return false;
                case "W64":
                    dst = System.IO.Path.Combine(System.IO.Path.Combine(pluginBase, "x86_64"), "VText.dll");
                    Debug.Log("delete " + dst);
                    FileUtil.DeleteFileOrDirectory(dst);
                    AssetDatabase.Refresh();
                    return false;
            }
        }
        return false;
    }
#endif
    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent(AssetDatabase.LoadAssetAtPath("Assets/Virtence/Logo/logo_virtence_Dialog.png", typeof(Texture)) as Texture)
            , GUILayout.Height(116f));
        EditorGUILayout.EndHorizontal();
        if (checkDone)
        {
#if UNITY_5
            // No plugin copy required
#else
            if (pluginBaseExists)
            {
                GUILayout.Label("Plugins");
                bool nv = GUILayout.Toggle(_plugAndroid, "Android");
                if (nv != _plugAndroid)
                {
                    _plugAndroid = SetupPlugin("Android" , nv);
                }
                nv = GUILayout.Toggle(_plugIOS, "iOS");
                if (nv != _plugIOS)
                {
                    _plugIOS = SetupPlugin("iOS", nv);
                }
                nv = GUILayout.Toggle(_plugLinux32, "Linux 32");
                if (nv != _plugLinux32)
                {
                    _plugLinux32 = SetupPlugin("L32", nv);
                }
                nv = GUILayout.Toggle(_plugLinux64, "Linux 64");
                if (nv != _plugLinux64)
                {
                    _plugLinux64 = SetupPlugin("L64", nv);
                }
                nv = GUILayout.Toggle(_plugOSX, "OSX");
                if (nv != _plugOSX)
                {
                    _plugOSX = SetupPlugin("OSX", nv);
                }
                nv = GUILayout.Toggle(_plugWin32, "Windows 32");
                if (nv != _plugWin32)
                {
                    _plugWin32 = SetupPlugin("W32", nv);
                }
                nv = GUILayout.Toggle(_plugWin64, "Windows 64");
                if (nv != _plugWin64)
                {
                    _plugWin64 = SetupPlugin("W64", nv);
                }
            }
            else
            {
                if (GUILayout.Button("Create Plugins Folder"))
                {
                    string pluginBase = System.IO.Path.Combine(Application.dataPath, "Plugins");
                    DirectoryInfo di = Directory.CreateDirectory(pluginBase);
                    if (di.Exists)
                    {
                        pluginBaseExists = true;
                    }
                    else
                    {
                        logString += "Creating " + di.Name + " failed\n";
                        Debug.LogError("Creating " + di.Name + " failed");
                    }
                }
            }
#endif
            if (!streamingAssetsExists)
            {
                if (GUILayout.Button("Create StreamingAssets Folder"))
                {
                    DirectoryInfo di = Directory.CreateDirectory(Application.streamingAssetsPath);
                    if (di.Exists)
                    {
                        streamingAssetsExists = true;
                    }
                    else 
                    {
                        logString += "Creating " + di.Name + " failed\n";
                        Debug.LogError("Creating " + di.Name + " failed");
                    }
                }
            }
            else
            {
                if (streamingAssetsFontsExists)
                {
					if (GUILayout.Button("Install all provided fonts"))
					{
						DirectoryInfo di = new  DirectoryInfo("Assets/Virtence/VText/Fonts/");
						FileInfo[] fiarray = di.GetFiles ("*.*");

						foreach (FileInfo fi in fiarray) {
							if (".ttf" == fi.Extension || ".otf" == fi.Extension) {
								string fileName = fi.FullName;
								fileName = fileName.Replace('\\','/');
								Debug.Log(fileName);
								InstallFont(fileName);
							} 
						}

						AssetDatabase.Refresh();
					}

					if (GUILayout.Button("Install fonts individually"))
					{
						string fname = EditorUtility.OpenFilePanel("Font", "Assets/Virtence/VText/Fonts", "ttf; *.otf");
						if("" != fname)
						{
							InstallFont(fname);
							Debug.Log (fname);
							AssetDatabase.Refresh();
						}
					}
					
				}
				else
				{
					if (GUILayout.Button("Create StreamingAssets/Fonts Folder"))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(System.IO.Path.Combine(Application.streamingAssetsPath, "Fonts"));
                        if (di.Exists)
                        {
                            streamingAssetsFontsExists = true;
                        }
                        else
                        {
                            logString += "Creating " + di.Name + " failed\n";
                        }
                    }
                }
            }
            if (GUILayout.Button("Check Again"))
            {
                // Debug.Log("Again");
                CheckSetup();
            }
        }
        else
        {
            if (GUILayout.Button("Check"))
            {
                CheckSetup();
            }
        }

        GUILayout.Label("Log");
        logScroll = EditorGUILayout.BeginScrollView(logScroll);
        EditorGUILayout.TextArea(logString);
        EditorGUILayout.EndScrollView();
    }

    void InstallFont(string fontname)
    {
        try
        {
            FileInfo fi = new FileInfo(fontname);
            FileUtil.CopyFileOrDirectory(fontname, System.IO.Path.Combine(System.IO.Path.Combine(Application.streamingAssetsPath, "Fonts"), fi.Name));
            logString += "installed " + fi.Name + "\n";
        }
        catch (IOException e)
        {
            logString += e.Message;
        }
    }

    void CheckSetup()
    {
        logString = "";
#if UNITY_5
        logString += "----- Unity 5 -----\n";
#else
        logString = "----- Unity 4 -----\n";
#endif
        CheckPluginsValid();
        CheckFontFolderValid();
        checkDone = true;
    }

    #region SETUP_CHECK
    private bool CheckPluginsValid()
    {
#if UNITY_5
        // Debug.Log("Application.dataPath " + Application.dataPath);
        string pluginBase = System.IO.Path.Combine(Application.dataPath, "Virtence/VText/Plugins");
#else
        string pluginBase = System.IO.Path.Combine(Application.dataPath, "Plugins");
#endif
        // Debug.Log("pluginBase " + pluginBase);
        DirectoryInfo di = new DirectoryInfo(pluginBase);
        if (di.Exists)
        {
#if UNITY_5
            PluginImporter[] plugs = PluginImporter.GetAllImporters();
            if (null != plugs)
            {
                logString += "Check Adjust " + plugs.Length + " native plugins\n";
                foreach (PluginImporter plug in plugs)
                {
                    FileInfo fi = new FileInfo(plug.assetPath);
                    bool refresh = false;
                    bool anyPlatform = false;
                    // Debug.Log(plug.assetPath + " dir: " + fi.Directory.Name);
                    switch (fi.Directory.Name)
                    {
                        case "Plugins":
                            if ("VText.bundle" == fi.Name)
                            {
                                anyPlatform = plug.GetCompatibleWithAnyPlatform();
                                if (anyPlatform)
                                {
                                    logString += "Adjusting OSX\n";
                                    plug.SetCompatibleWithAnyPlatform(false);
                                    plug.SetCompatibleWithPlatform(BuildTarget.iOS, false);
                                    plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXUniversal, true);
                                    Debug.Log(Application.platform);
                                    switch (Application.platform)
                                    {
                                        case RuntimePlatform.OSXEditor:
                                            plug.SetCompatibleWithEditor(true);
                                            break;
                                        default:
                                            plug.SetCompatibleWithEditor(false);
                                            break;
                                    }
                                    refresh = true;
                                }
                            }
                            break;
                        case "Android":
                            if ("libVText.so" == fi.Name)
                            {
                                anyPlatform = plug.GetCompatibleWithAnyPlatform();
                                // Debug.Log("Android CPU " + plug.GetPlatformData(BuildTarget.Android, "CPU") + " | " + plug.userData + " |");
                                if (anyPlatform)
                                {
                                    logString += "Adjusting Android\n";
                                    plug.SetCompatibleWithAnyPlatform(false);
                                    plug.SetCompatibleWithEditor(false);
                                    plug.SetCompatibleWithPlatform(BuildTarget.iOS, false);
                                    plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXUniversal, false);
                                    plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel, false);
                                    plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel64, false);
                                    plug.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows, false);
                                    plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux, false);
                                    plug.SetCompatibleWithPlatform(BuildTarget.Android, true);
                                    plug.SetPlatformData(BuildTarget.Android, "CPU", "ARMv7");
                                    refresh = true;
                                }
                            }
                            break;
                        case "iOS":
                            if ("libVTextIOS.a" == fi.Name)
                            {
                                anyPlatform = plug.GetCompatibleWithAnyPlatform();
                                // Debug.Log("iOS CPU " + plug.GetPlatformData(BuildTarget.Android, "CPU") + " | " + plug.userData + " |");
                                if (anyPlatform)
                                {
                                    logString += "Adjusting iOS\n";
                                    plug.SetCompatibleWithAnyPlatform(false);
                                    plug.SetCompatibleWithEditor(false);
                                    plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXUniversal, false);
                                    plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel, false);
                                    plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel64, false);
                                    plug.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows, false);
                                    plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux, false);
                                    plug.SetCompatibleWithPlatform(BuildTarget.Android, false);
                                    plug.SetCompatibleWithPlatform(BuildTarget.iOS, true);
                                    refresh = true;
                                }
                            }
                            break;
                        case "x86":
                            switch (fi.Name)
                            {
                                case "VText.dll":
                                    anyPlatform = plug.GetCompatibleWithAnyPlatform();
                                    // Debug.Log("W32 CPU " + plug.GetPlatformData(BuildTarget.Android, "CPU") + " | " + plug.userData + " |");
                                    if (anyPlatform)
                                    {
                                        logString += "Adjusting Windows 32\n";
                                        plug.SetCompatibleWithAnyPlatform(false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.iOS, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXUniversal, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel64, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux64, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinuxUniversal, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows64, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows, true);
                                        switch (Application.platform)
                                        {
                                            case RuntimePlatform.WindowsEditor:
#if UNITY_EDITOR_32
                                                plug.SetCompatibleWithEditor(true);
#else
                                                plug.SetCompatibleWithEditor(false);
#endif
                                                break;
                                            default:
                                                plug.SetCompatibleWithEditor(false);
                                                break;
                                        }
                                        refresh = true;
                                        // Debug.Log("W32 CPU " + plug.GetPlatformData(BuildTarget.StandaloneWindows, "CPU"));
                                        plug.SetPlatformData(BuildTarget.StandaloneWindows, "CPU", "x86");
                                    }
                                    break;
                                case "libVText.so":
                                    anyPlatform = plug.GetCompatibleWithAnyPlatform();
                                    // Debug.Log("L32 CPU " + plug.GetPlatformData(BuildTarget.Android, "CPU") + " | " + plug.userData + " |");
                                    if (anyPlatform)
                                    {
                                        logString += "Adjusting Linux 32\n";
                                        plug.SetCompatibleWithAnyPlatform(false);
                                        plug.SetCompatibleWithEditor(false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.iOS, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXUniversal, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel64, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows64, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux64, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinuxUniversal, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux, true);
                                        // Debug.Log("L32 CPU " + plug.GetPlatformData(BuildTarget.StandaloneLinux, "CPU"));
                                        plug.SetPlatformData(BuildTarget.StandaloneLinux, "CPU", "x86");
                                        refresh = true;
                                    }
                                    break;
                            }
                            break;
                        case "x86_64":
                            switch (fi.Name)
                            {
                                case "VText.dll":
                                    anyPlatform = plug.GetCompatibleWithAnyPlatform();
                                    // Debug.Log("W64 CPU " + plug.GetPlatformData(BuildTarget.Android, "CPU") + " | " + plug.userData + " |");
                                    if (anyPlatform)
                                    {
                                        logString += "Adjusting Windows 64\n";
                                        plug.SetCompatibleWithAnyPlatform(false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.iOS, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXUniversal, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel64, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinuxUniversal, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux64, true);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows64, true);
                                        switch (Application.platform)
                                        {
                                            case RuntimePlatform.WindowsEditor:
#if UNITY_EDITOR_32
                                                plug.SetCompatibleWithEditor(false);
#else
                                                plug.SetCompatibleWithEditor(true);
#endif
                                                break;
                                            default:
                                                plug.SetCompatibleWithEditor(false);
                                                break;
                                        }
                                        plug.SetPlatformData(BuildTarget.StandaloneLinux, "CPU", "x86_64");
                                        refresh = true;
                                    }
                                    break;
                                case "libVText.so":
                                    anyPlatform = plug.GetCompatibleWithAnyPlatform();
                                    // Debug.Log("L64 CPU " + plug.GetPlatformData(BuildTarget.Android, "CPU") + " | " + plug.userData + " |");
                                    if (anyPlatform)
                                    {
                                        logString += "Adjusting Linux 64\n";
                                        plug.SetCompatibleWithAnyPlatform(false);
                                        plug.SetCompatibleWithEditor(false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.iOS, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXUniversal, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel64, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinuxUniversal, false);
                                        plug.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux64, true);
                                        plug.SetPlatformData(BuildTarget.StandaloneLinux, "CPU", "x86_64");
                                        refresh = true;
                                    }
                                    break;
                            }
                            break;
                    }
                    if (refresh)
                    {
                        plug.SaveAndReimport();
                    }
                }
                return true;
            }
            else
            {
                Debug.LogError("Unable to fetch plugins");
            }
#else
            pluginBaseExists = true;
            DirectoryInfo[] diarray = di.GetDirectories();
            foreach (DirectoryInfo dip in diarray)
            {
                Debug.Log(dip.Name);
                switch (dip.Name)
                {
                    case "Android":
                        if (dip.GetFiles("libVText.so").Length > 0)
                        {
                            _plugAndroid = true;
                        }
                        break;
                    case "iOS":
                        _plugIOS = true;
                        break;
                    case "VText.bundle":
                        _plugOSX = true;
                        break;
                    case "x86":
                        if (dip.GetFiles("VText.dll").Length > 0)
                        {
                            _plugWin32 = true;
                        }
                        if (dip.GetFiles("libVText.so").Length > 0)
                        {
                            _plugLinux32 = true;
                        }
                        break;
                    case "x86_64":
                        if (dip.GetFiles("VText.dll").Length > 0)
                        {
                            _plugWin64 = true;
                        }
                        if (dip.GetFiles("libVText.so").Length > 0)
                        {
                            _plugLinux64 = true;
                        }
                        break;
                }
            }
#endif
        }
        else
        {
            Debug.LogError("No plugins Folder available at " + pluginBase);
            logString += "No plugins Folder available at " + pluginBase;
        }
        return false;
    }

    private bool CheckFontFolderValid()
    {
        DirectoryInfo di = new DirectoryInfo(Application.streamingAssetsPath);
        if (!di.Exists)
        {
            Debug.LogWarning("No StreamingAssets available");
        }
        if (di.Exists)
        {
            streamingAssetsExists = true;
            Debug.Log("SA " + Application.streamingAssetsPath);
            DirectoryInfo dif = new DirectoryInfo(System.IO.Path.Combine(Application.streamingAssetsPath, "Fonts"));
            if (dif.Exists)
            {
                streamingAssetsFontsExists = true;
                FileInfo[] fiarray = dif.GetFiles("*.*");
                numFonts = 0;
                logString += "----- installed Fonts -----\n";
                foreach (FileInfo fi in fiarray)
                {
                    if (".ttf" == fi.Extension)
                    {
                        // Debug.Log(numFonts + " " + fi.Name + " ext: " + fi.Extension);
                        logString += "[" + numFonts + "]: " + fi.Name + "\n";
                        numFonts++;
                    }
                    else if (".otf" == fi.Extension)
                    {
                        // Debug.Log(numFonts + " " + fi.Name + " ext: " + fi.Extension);
                        logString += "[" + numFonts + "]: " + fi.Name + "\n";
                        numFonts++;
                    }
                }
                if (numFonts > 0)
                {
                    // Debug.Log("Found " + numFonts + " Fonts in " + dif.Name);
                    logString += numFonts + " Fonts installed\n";
                    return true;
                }
                else
                {
                    Debug.LogWarning("Please install some Fonts in " + dif.Name);
                    logString += "No Fonts installed\n";
                }
            }
        }
        return false;
    }
    #endregion // SETUP_CHECK
}
