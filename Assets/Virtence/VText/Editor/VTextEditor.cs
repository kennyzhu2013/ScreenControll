/*
 * $Id: VTextEditor.cs 174 2015-03-16 09:55:03Z dirk $
 * 
 * Virtence VFont package
 * Copyright 2014 .. 2015 by Virtence GmbH
 * http://www.virtence.com
 * 
 */


using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
#if UNITY_5
using UnityEngine.Rendering;
#endif

[CustomEditor (typeof (VTextInterface))] 
public class VTextEditor : Editor {
	[MenuItem ("GameObject/Virtence/VText")]
	static void Create() {
		GameObject go = new GameObject("VText");
		VTextInterface s = go.AddComponent<VTextInterface>();
		string defMatPath = "Assets/Virtence/VText/Materials/UnlitWhite.mat";
		Material m  = (Material)AssetDatabase.LoadAssetAtPath(defMatPath, typeof(Material));
		if(null != m) {
			s.materials[0] = new Material(m);
			s.materials[1] = new Material(m);
			s.materials[2] = new Material(m);
		} else {
			Debug.LogWarning("No default Material");
		}
		s.Rebuild();
		Selection.activeGameObject = go;
	}

	private string[] fontnames;
	private int fontChoice;
	private bool showMeshParam = true;
	private bool showLayoutParam = true;
	private bool showBend = true;
	private bool showCircular = true;
	private Vector2 textScroll = new Vector2(0f,0f);
	protected void appendFontname(string fn)
	{
		string [] nfn = new string[fontnames.Length+1];
		for(int k=0; k < fontnames.Length; k++) {
			nfn[k] = fontnames[k];
		}
		nfn[fontnames.Length] = fn;
		fontnames = nfn;
	}

	protected void fillFonts(string oldname)
	{
		DirectoryInfo di = new DirectoryInfo(System.IO.Path.Combine(Application.streamingAssetsPath, "Fonts"));
		FileInfo[] fiarray = di.GetFiles("*.*");
		fontnames = new string[] { "(none)" };
		int fc = 0;
		// fontChoice = 0;

		foreach(FileInfo fi in fiarray) {
			// Debug.Log(fi.Name + " ext: " + fi.Extension);
			if(".ttf" == fi.Extension) {
				if(oldname == fi.Name) {
					fc = fontnames.Length;
				}
				appendFontname(fi.Name);
			} else if(".otf" == fi.Extension) {
				if(oldname == fi.Name) {
					fc = fontnames.Length;
				}
				appendFontname(fi.Name);
			}
		}
		if(fc != fontChoice) {
			// Debug.Log(" fc: " + fc);
			fontChoice = fc;
		}
		// Debug.Log(fontnames);
	}

	public override void OnInspectorGUI ()
	{
		VTextInterface obj;
		obj = target as VTextInterface;
		if (obj == null) {
			Debug.LogError("null object");
			return;
		}

		SerializedProperty param = serializedObject.FindProperty("parameter"),
		layout = serializedObject.FindProperty("layout"),
		materials = serializedObject.FindProperty("materials");
		// text
		SerializedProperty rtext = serializedObject.FindProperty("RenderText");
		// mesh
		SerializedProperty pDepth = param.FindPropertyRelative("m_depth");
		SerializedProperty pBevel = param.FindPropertyRelative("m_bevel");
		SerializedProperty pNeedTangents = param.FindPropertyRelative("m_needTangents");
		SerializedProperty pBackface = param.FindPropertyRelative("m_backface");
#if CREASE_OK
		SerializedProperty pCrease = param.FindPropertyRelative("m_crease");
#endif
		SerializedProperty pFontname = param.FindPropertyRelative("m_fontname");
		// layout
		SerializedProperty lHorizontal = layout.FindPropertyRelative("m_horizontal");
		SerializedProperty lMajor = layout.FindPropertyRelative("m_major");
		SerializedProperty lMinor = layout.FindPropertyRelative("m_minor");
		SerializedProperty lSize = layout.FindPropertyRelative("m_size");
		SerializedProperty lSpacing = layout.FindPropertyRelative("m_spacing");
		SerializedProperty curveXZ = layout.FindPropertyRelative("m_curveXZ");
		SerializedProperty curveXY = layout.FindPropertyRelative("m_curveXY");
		SerializedProperty orientXZ = layout.FindPropertyRelative("m_orientXZ");
		SerializedProperty orientXY = layout.FindPropertyRelative("m_orientXY");

		SerializedProperty isCircular = layout.FindPropertyRelative("m_isCircular");
		SerializedProperty startRadius = layout.FindPropertyRelative("m_startRadius");
		SerializedProperty endRadius = layout.FindPropertyRelative("m_endRadius");
		SerializedProperty circleRadius = layout.FindPropertyRelative("m_circleRadius");
		SerializedProperty animateRadius = layout.FindPropertyRelative("m_animateRadius");
		SerializedProperty radiusCurve = layout.FindPropertyRelative("m_curveRadius");

#if UNITY_5
        SerializedProperty pShadowCast = layout.FindPropertyRelative("m_shadowCastMode");
#else
        SerializedProperty pCastShadows = layout.FindPropertyRelative("m_castShadows");
#endif
		SerializedProperty pReceiveShadows = layout.FindPropertyRelative("m_receiveShadows");
		SerializedProperty pUseLightProbes = layout.FindPropertyRelative("m_useLightProbes");

		serializedObject.Update();

		bool updateMesh = false;
		bool updateLayout = false;

		GUILayout.Label("Text");
		textScroll = EditorGUILayout.BeginScrollView(textScroll, GUILayout.MinHeight(150), GUILayout.MaxHeight(150.0f));
		// the text to show by default
		string rt = EditorGUILayout.TextArea(rtext.stringValue, GUILayout.MinHeight(140));
		if(rt != rtext.stringValue) {
			rtext.stringValue = rt;
			updateMesh = true;
		}
		EditorGUILayout.EndScrollView();

		// Mesh parameter
		showMeshParam = EditorGUILayout.Foldout(showMeshParam, "Mesh Parameter");
		if (showMeshParam) {
			float nDepth = EditorGUILayout.FloatField (new GUIContent ("Depth", "The size of glyph contour side(s)"), pDepth.floatValue);
			if (nDepth < 0.0f) {
					nDepth = 0.0f;
			}
			if (nDepth != pDepth.floatValue) {
					pDepth.floatValue = nDepth;
					updateMesh = true;
			}
			float nBevel = Mathf.Clamp01 (EditorGUILayout.FloatField (new GUIContent ("Bevel", "Delta of bevel frame"), pBevel.floatValue));
			if (nBevel != pBevel.floatValue) {
					pBevel.floatValue = nBevel;
					updateMesh = true;
			}
#if UNITY_5
            if (EditorGUILayout.PropertyField(pShadowCast, new GUIContent("ShadowCastingMode")))
            {
                updateLayout = true;
            }
#else
            bool nCastShadow = EditorGUILayout.Toggle (new GUIContent ("Cast Shadows", "Flag passed to glyph children"), pCastShadows.boolValue);
			if (nCastShadow != pCastShadows.boolValue) {
					pCastShadows.boolValue = nCastShadow;
					updateLayout = true;
			}
#endif
			bool nReceiveShadows = EditorGUILayout.Toggle (new GUIContent ("Receive Shadows", "Flag passed to glyph children"), pReceiveShadows.boolValue);
			if (nReceiveShadows != pReceiveShadows.boolValue) {
					pReceiveShadows.boolValue = nReceiveShadows;
					updateLayout = true;
			}
			bool nUseLightProbes = EditorGUILayout.Toggle (new GUIContent ("Use Light Probes", "Flag passed to glyph children"), pUseLightProbes.boolValue);
			if (nUseLightProbes != pUseLightProbes.boolValue) {
					pUseLightProbes.boolValue = nUseLightProbes;
					updateLayout = true;
			}
			bool nNeedTangents = EditorGUILayout.Toggle (new GUIContent ("Need Tangents", "Set if shader requires tangents"), pNeedTangents.boolValue);
			if (nNeedTangents != pNeedTangents.boolValue) {
					pNeedTangents.boolValue = nNeedTangents;
					updateMesh = true;
			}
			bool nBackface = EditorGUILayout.Toggle (new GUIContent ("Backface", "Set if you need a visible backface"), pBackface.boolValue);
			if (nBackface != pBackface.boolValue) {
					pBackface.boolValue = nBackface;
					updateMesh = true;
			}
	    }
#if CREASE_OK
		float nCrease = EditorGUILayout.FloatField("Crease Angle", pCrease.floatValue);
		if(nCrease != pCrease.floatValue) {
			if(nCrease >= 0f) {
				if(nCrease < 90f) {
					pCrease.floatValue = nCrease;
					updateMesh = true;
				}
			}
		}
#endif
		// Layout
		showLayoutParam = EditorGUILayout.Foldout(showLayoutParam, "Layout Parameter");
		if (showLayoutParam) {
			bool nHorizontal = EditorGUILayout.Toggle (new GUIContent ("Horizontal", "Major string direction(Vertical if off)"), lHorizontal.boolValue);
			if (nHorizontal != lHorizontal.boolValue) {
					lHorizontal.boolValue = nHorizontal;
					updateLayout = true;
			}

			int nMajor = (int)(VTextLayout.align)EditorGUILayout.EnumPopup (new GUIContent ("Major", "Major layout mode"),
                                                       (VTextLayout.align)System.Enum.GetValues (typeof(VTextLayout.align)).GetValue (lMajor.enumValueIndex));
			if (lMajor.enumValueIndex != nMajor) {
					lMajor.enumValueIndex = nMajor;
					updateLayout = true;
			}
			int nMinor = (int)(VTextLayout.align)EditorGUILayout.EnumPopup (new GUIContent ("Minor", "Minor layout mode"),
                                                  (VTextLayout.align)System.Enum.GetValues (typeof(VTextLayout.align)).GetValue (lMinor.enumValueIndex));
			if (lMinor.enumValueIndex != nMinor) {
					lMinor.enumValueIndex = nMinor;
					updateLayout = true;
			}
			float nSize = EditorGUILayout.FloatField ("Size", lSize.floatValue);
			if (nSize != lSize.floatValue) {
					lSize.floatValue = nSize;
					updateLayout = true;
			}
			float nSpacing = EditorGUILayout.FloatField ("Spacing", lSpacing.floatValue);
			if (nSpacing != lSpacing.floatValue) {
					lSpacing.floatValue = nSpacing;
					updateLayout = true;
			}
		}

		/*
		 * Materials
		 */
		Object faceMat = materials.GetArrayElementAtIndex(0).objectReferenceValue;
		Object nFaceMat = EditorGUILayout.ObjectField("Face Material", faceMat, typeof(Material), false);
		if(nFaceMat != faceMat) {
			Debug.Log("faceMat change " + nFaceMat);
			materials.GetArrayElementAtIndex(0).objectReferenceValue = nFaceMat;
			updateMesh = true;
		}

		Object sideMat = materials.GetArrayElementAtIndex(1).objectReferenceValue;
		Object nSideMat = EditorGUILayout.ObjectField("Side Material", sideMat, typeof(Material), false);
		if(nSideMat != sideMat) {
			Debug.Log("sideMat change " + nSideMat);
			materials.GetArrayElementAtIndex(1).objectReferenceValue = nSideMat;
			updateMesh = true;
		}

		Object frameMat = materials.GetArrayElementAtIndex(2).objectReferenceValue;
		Object nFrameMat = EditorGUILayout.ObjectField("Frame Material", frameMat, typeof(Material), false);
		if(nFrameMat != frameMat) {
			Debug.Log("frameMat change " + nFrameMat);
			materials.GetArrayElementAtIndex(2).objectReferenceValue = nFrameMat;
			updateMesh = true;
		}

		/*
		 * Font selector
		 */
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label(new GUIContent("Select Font","from Fonts available in \'StreamingAssets/Fonts\'"));
		fillFonts(obj.parameter.Fontname);
		// Debug.Log(" OnInspectorGUI(): " + fontnames.Length);
		int fc = EditorGUILayout.Popup(fontChoice, fontnames);
		if(fc != fontChoice) {
			// Debug.Log("fontChoice " + fc);
			fontChoice = fc;
			if(fc > 0) {
				obj.parameter.Fontname = fontnames[fc];
				pFontname.stringValue = fontnames[fc];
			} else {
				obj.parameter.Fontname = "";
				pFontname.stringValue = "";
			}
			updateMesh = true;
		}
		EditorGUILayout.EndHorizontal ();

		/*
		 * Bend Curves
		 */
		showBend = EditorGUILayout.Foldout(showBend, "Curve Bending");
		if (showBend) {
			EditorGUILayout.BeginHorizontal ();
			AnimationCurve cxz = curveXZ.animationCurveValue;
			GUILayout.Label (new GUIContent ("Bend XZ", "bend Z-Axis along max. line width"));
			AnimationCurve ncxz = EditorGUILayout.CurveField (cxz);
			if (!ncxz.Equals (cxz)) {
					curveXZ.animationCurveValue = ncxz;
					updateLayout = true;
			}
			bool oxz = EditorGUILayout.ToggleLeft (new GUIContent ("T.", "Align Tangent Z"), orientXZ.boolValue, GUILayout.MaxWidth (35));
			if (oxz != orientXZ.boolValue) {
					orientXZ.boolValue = oxz;
					updateLayout = true;
			}
			if (GUILayout.Button ("Reset")) {
					curveXZ.animationCurveValue = new AnimationCurve (new Keyframe (0, 0), new Keyframe (1, 0));
					updateLayout = true;
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			AnimationCurve cxy = curveXY.animationCurveValue;
			GUILayout.Label (new GUIContent ("Bend XY", "bend Y-Axis along max. line width"));
			AnimationCurve ncxy = EditorGUILayout.CurveField (cxy);
			if (!ncxy.Equals (cxy)) {
					curveXY.animationCurveValue = ncxy;
					updateLayout = true;
			}
			bool oxy = EditorGUILayout.ToggleLeft (new GUIContent ("T.", "Align Tangent Y"), orientXY.boolValue, GUILayout.MaxWidth (35));
			if (oxy != orientXY.boolValue) {
					orientXY.boolValue = oxy;
					updateLayout = true;
			}
			if (GUILayout.Button ("Reset")) {
					curveXY.animationCurveValue = new AnimationCurve (new Keyframe (0, 0), new Keyframe (1, 0));
					updateLayout = true;
			}
			EditorGUILayout.EndHorizontal ();
		}


		/*
		 * Circular Bending
		 */
		showCircular = EditorGUILayout.Foldout(showCircular, "Circular Bending");
		if (showCircular) {
			bool isCircle = EditorGUILayout.ToggleLeft (new GUIContent ("Activate", "Should the text align on a circular area?"), isCircular.boolValue);
			if (isCircle != isCircular.boolValue) {
				isCircular.boolValue = isCircle;
				updateLayout = true;
			}
			if( isCircle)
			{
				float start = EditorGUILayout.FloatField (new GUIContent ("Start", "The starting radius"), startRadius.floatValue);
				if (start != startRadius.floatValue) {
					startRadius.floatValue = start;
					updateLayout = true;
				}
				float end = EditorGUILayout.FloatField (new GUIContent ("End", "The ending radius"), endRadius.floatValue);
				if (end != endRadius.floatValue) {
					endRadius.floatValue = end;
					updateLayout = true;
				}


				EditorGUILayout.BeginHorizontal ();

				float radius = EditorGUILayout.FloatField (new GUIContent ("Radius", "The size of the circle"), circleRadius.floatValue);
				if (radius != circleRadius.floatValue) {
					circleRadius.floatValue = radius;
					updateLayout = true;
				}



				bool animR = EditorGUILayout.ToggleLeft (new GUIContent ("T.", "Animate Radius?"), animateRadius.boolValue, GUILayout.MaxWidth (35));
				if (animR != animateRadius.boolValue) {
					animateRadius.boolValue = animR;
					updateLayout = true;
				}
				if(animR)
				{
					AnimationCurve cR = radiusCurve.animationCurveValue;
					AnimationCurve ncR = EditorGUILayout.CurveField (cR);
					if (!ncR.Equals (cR)) {
						radiusCurve.animationCurveValue = ncR;
						updateLayout = true;
					}
					if (GUILayout.Button ("Reset")) {
						radiusCurve.animationCurveValue = new AnimationCurve (new Keyframe (0, 0), new Keyframe (1, 0));
						updateLayout = true;
					}
				}
				EditorGUILayout.EndHorizontal ();


			}
		}

		EditorGUILayout.BeginHorizontal ();
		// Rebuild mesh when user click the Rebuild button
		if (GUILayout.Button("Rebuild")){
			obj.Rebuild();
		}
		EditorGUILayout.EndHorizontal ();

		if(serializedObject.ApplyModifiedProperties()) {
			// Debug.Log("modified");
			obj.CheckRebuild(updateMesh, updateLayout);
		}
	}
}
