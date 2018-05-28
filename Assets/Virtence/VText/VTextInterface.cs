/*
 * $Id: VTextInterface.cs 172 2015-03-13 14:05:02Z dirk $
 * 
 * Virtence VFont package
 * Copyright 2014 .. 2015 by Virtence GmbH
 * http://www.virtence.com
 * 
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Virtence.VText;
#if UNITY_5
using UnityEngine.Rendering;
#endif

[System.Serializable]
public class VTextLayout
{
		//! alignment
		public enum align
		{
				Base,    //! baseline
				Start,   //! bounding box start
				Center,  //! bounding box center
				End      //! bounding box end
	}
		;

		[SerializeField]
		private bool
				m_horizontal = true;
		[SerializeField]
		private align
				m_major = align.Base;
		[SerializeField]
		private align
				m_minor = align.Base;
		[SerializeField]
		private float
				m_size = 1.0f;
		[SerializeField]
		private float
				m_spacing = 1.0f;
		[SerializeField]
		private AnimationCurve
				m_curveXZ;
		[SerializeField]
		private AnimationCurve
				m_curveXY;
		[SerializeField]
		private bool
				m_orientXZ = false;
		[SerializeField]
		private bool
				m_orientXY = false;
		[SerializeField]
		private bool
				m_isCircular = false;
		[SerializeField]
		private float
				m_startRadius = 0.0f;
		[SerializeField]
		private float
				m_endRadius = 180.0f;
		[SerializeField]
		private float
				m_circleRadius = 10.0f;
		[SerializeField]
		private bool
			m_animateRadius = false;
		[SerializeField]
		private AnimationCurve
			m_curveRadius;
		[HideInInspector]
		private bool
			m_modified = false;


	/// <summary>
	/// The cast shadows property
	/// 
	/// will be passed to children  Mesh Renderer.
	/// </summary>
	[SerializeField]
#if UNITY_5
    private ShadowCastingMode m_shadowCastMode = ShadowCastingMode.On;
#else
	private bool	m_castShadows = true;
#endif
    /// <summary>
	/// The receive shadows property
	/// 
	/// will be passed to children Mesh Renderer.
	/// </summary>
	[SerializeField]
    private bool m_receiveShadows = true;
	/// <summary>
	/// The use light probes property
	/// 
	/// will be passed to children Mesh Renderer.
	/// </summary>
	[SerializeField]
	private bool m_useLightProbes = false;

	public bool CheckClearModified ()
	{
			if (m_modified) {
					m_modified = false;
					return true;
			}
			return false;
	}

	/// <summary>
	/// Main layout direction.
	/// If false the text will be layout vertical.
	/// </summary>
	public bool Horizontal {
			get {
					return m_horizontal;
			}
			set {
					m_horizontal = value;
					m_modified = true;
			}
	}

	/// <summary>
	/// The major aligment.
	/// </summary>
	public align Major {
			get {
					return m_major;
			}
			set {
					if (value != m_major) {
							m_modified = true;
					}
					m_major = value;
			}
	}

	/// <summary>
	/// The minor aligment.
	/// </summary>
	public align Minor {
			get {
					return m_minor;
			}
			set {
					if (value != m_minor) {
							m_modified = true;
					}
					m_minor = value;
			}
	}

	/// <summary>
	/// The font size scale factor.
	/// </summary>
	public float Size {
			get {
					return m_size;
			}
			set {
					if (value != m_size) {
							m_modified = true;
					}
					m_size = value;
			}
	}
	/// <summary>
	/// The line spacing factor.
	/// </summary>
	public float Spacing {
			get {
					return m_spacing;
			}
			set {
					if (value != m_spacing) {
							m_modified = true;
							m_spacing = value;
					}
			}
	}

	public VTextLayout ()
	{
			m_curveXZ = new AnimationCurve (new Keyframe (0, 0), new Keyframe (1, 0));
			m_curveXY = new AnimationCurve (new Keyframe (0, 0), new Keyframe (1, 0));
			m_curveRadius = new AnimationCurve (new Keyframe (0, 0), new Keyframe (1, 0));
	}

	/// <summary>
	/// The XZ Curve
	/// </summary>
	public AnimationCurve CurveXZ {
			get {
					return m_curveXZ;
			}
			set {
					m_modified = true;
					m_curveXZ = value;
			}
	}
	/// <summary>
	/// The XY Curve
	/// </summary>
	public AnimationCurve CurveXY {
			get {
					return m_curveXY;
			}
			set {
					m_modified = true;
					m_curveXY = value;
			}
	}
	/// <summary>
	/// adjust orientation for XZ Curve
	/// </summary>
	public bool OrientationXZ {
			get {
					return m_orientXZ;
			}
			set {
					if (value != m_orientXZ) {
							m_modified = true;
							m_orientXZ = value;
					}
			}
	}
	/// <summary>
	/// adjust orientation for XY Curve
	/// </summary>
	public bool OrientationXY {
			get {
					return m_orientXY;
			}
			set {
					if (value != m_orientXY) {
							m_modified = true;
							m_orientXY = value;
					}
			}
	}
	/// <summary>
	/// bend the text circular
	/// </summary>
	public bool OrientationCircular {
		get {
			return m_isCircular;
		}
		set {
		if (value != m_isCircular) {
				m_modified = true;
				m_isCircular = value;
			}
		}
	}

	/// <summary>
	/// Gets or sets the start radius.
	/// </summary>
	/// <value>The start radius.</value>
	public float StartRadius {
		get {
			return m_startRadius;
		}
		set {
			if (value != m_startRadius) {
				m_modified = true;
			}
			m_startRadius = value;
		}
	}

	/// <summary>
	/// Gets or sets the end radius.
	/// </summary>
	/// <value>The end radius.</value>
	public float EndRadius {
		get {
			return m_endRadius;
		}
		set {
			if (value != m_endRadius) {
				m_modified = true;
			}
			m_endRadius = value;
		}
	}

	/// <summary>
	/// Gets or sets the radius of the circle.
	/// </summary>
	/// <value>The circle radius.</value>
	public float CircleRadius {
		get {
			return m_circleRadius;
		}
		set {
			if (value != m_circleRadius) {
				m_modified = true;
			}
			m_circleRadius = value;
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether radius should be determined by the AnimationCurve CurveRadius
	/// </summary>
	/// <value><c>true</c> if animate radius; otherwise, <c>false</c>.</value>
	public bool AnimateRadius {
		get {
			return m_animateRadius;
		}
		set {
			if (value != m_animateRadius) {
				m_modified = true;
			}
			m_animateRadius = value;
		}
	}

	/// <summary>
	/// Gets or sets the CurveRadius Animationcurve.
	/// </summary>
	/// <value>The curve radius.</value>
	public AnimationCurve CurveRadius {
		get {
			return m_curveRadius;
		}
		set {
			if (value != m_curveRadius) {
				m_modified = true;
				m_curveRadius = value;
			}
		}
	}

#if UNITY_5
    /// <summary>
    /// shadow casting Mode
    /// </summary>
    public ShadowCastingMode ShadowCastMode
    {
        get
        {
            return m_shadowCastMode ;
        }
        set
        {
            m_shadowCastMode = value;
            m_modified = true;
        }
    }
#else
	/// <summary>
	/// flag cast shadows
	/// </summary>
	public bool CastShadows {
		get {
			return m_castShadows;
		}
		set {
			m_castShadows = value;
			m_modified = true;
		}
	}
#endif
	/// <summary>
	/// flag receive shadows
	/// </summary>
	public bool ReceiveShadows {
		get {
			return m_receiveShadows;
		}
		set {
			m_receiveShadows = value;
			m_modified = true;
		}
	}

	/// <summary>
	/// flag use Lightprobes
	/// </summary>
	public bool UseLightProbes {
		get {
			return m_useLightProbes;
		}
		set {
			m_useLightProbes = value;
			m_modified = true;
		}
	}
}

/// <summary>
/// VText parameter.
/// 
/// change requires rebuild of glyp meshes
/// </summary>
[System.Serializable]
public class VTextParameter
{
	#region parameter

		/// <summary>
		/// The depth of the glyphs.
		/// </summary>
		[SerializeField]
		private float m_depth = 0.0f;
		/// <summary>
		/// The bevel frame of the glyphs.
		/// 
		/// range [0..1] where 1 is max factor of 1/10 width of glyph
		/// </summary>
		[SerializeField]
		private float m_bevel = 0.0f;
		/// <summary>
		/// The need tangents property
		/// 
		/// If set, tangents will be generated for Mesh
		/// </summary>
		[SerializeField]
		private bool m_needTangents = false;
		/// <summary>
		/// create backface
		/// 
		/// If set, backface will be generated for Mesh
		/// </summary>
		[SerializeField]
		private bool m_backface = false;
		/// <summary>
		/// crease angle
		/// 
		/// in degree for smoothing sides and bevel.
		/// </summary>
		[SerializeField]
		private float m_crease = 35.0f;
		/// <summary>
		/// The fontname must specify a font available in StreamingAsset
		/// folder.
		/// Accepted formats are:
		///  - ttf
		///  - otf
		///  - ps (Postscript)
		/// </summary>
	    [SerializeField]
	    private string m_fontname = "mittelschrift.otf";
	#endregion

		[HideInInspector]
		private bool m_modified = false;
	
		public bool CheckClearModified ()
		{
				if (m_modified) {
						m_modified = false;
						return true;
				}
				return false;
		}

		/// <summary>
		/// The depth of the glyphs.
		/// 
		/// getter setter
		/// </summary>
		public float Depth {
				get {
						return m_depth;
				}
				set {
						float v = (value < 0.0f) ? 0.0f : value;
						if (m_depth != v) {
								m_depth = v;
								m_modified = true;
						}
				}
		}

		/// <summary>
		/// The crease angle to generate sides and bevel
		/// 
		/// getter setter
		/// range [10..45]
		/// </summary>
		public float Crease {
			get {
				return m_crease;
			}
			set {
				float v = Mathf.Clamp(value,10f,45f);
				if (m_crease != v) {
					m_crease = v;
					m_modified = true;
				}
			}
		}
	/// <summary>
	/// The bevel frame of the glyphs.
	/// 
	/// getter setter
	/// range [0..1] where 1 is max factor of 1/10 width of glyph
	/// </summary>
	public float Bevel {
		get {
			return m_bevel;
		}
		set {
			float v = Mathf.Clamp01 (value);
			if (m_bevel != v) {
				m_bevel = v;
				m_modified = true;
			}
		}
	}
	/// <summary>
	/// Flag generate backface
	/// 
	/// getter setter
	/// </summary>
	public bool Backface {
		get {
			return m_backface;
		}
		set {
			if (m_backface != value) {
				m_backface = value;
				m_modified = true;
			}
		}
	}
	/// <summary>
	/// Flag generate tangents
	/// 
	/// getter setter
	/// </summary>
	public bool GenerateTangents {
		get {
			return m_needTangents;
		}
		set {
			if (m_needTangents != value) {
				m_needTangents = value;
				m_modified = true;
			}
		}
	}
	/// <summary>
	/// Fontname
	/// 
	/// getter setter
	/// </summary>
	public string Fontname {
		get {
			return m_fontname;
		}
		set {
			if (m_fontname != value) {
				m_fontname = value;
				m_modified = true;
			}
		}
	}
}

/// <summary>
/// Virtence polygon text interface
/// </summary>
public class VTextInterface : MonoBehaviour
{
		[SerializeField]
		public  VTextParameter
				parameter;
		[SerializeField]
		public  VTextLayout
				layout;
		/// <summary>
		/// The text to render.
		/// might be overridden by external script for dynamic update.
		/// Line breaks by '\n'
		/// </summary>
		[SerializeField]
		public string
				RenderText = "Hello world";
		/// <summary>
		/// Check change on update
		/// </summary>
		private string m_oldText;


		/// <summary>
		/// Select your Materials.
		/// The meshes produced will have valid uv.
		/// </summary>
		public Material[] materials = new Material[3];
		private VFontInfo m_fontInfo = null;
		private List<MonoBehaviour> m_changeListener = null;

		public VTextInterface ()
		{
				parameter = new VTextParameter ();
				layout = new VTextLayout ();
		}
	
		public bool Is3D ()
		{
				if ((parameter.Depth > 0.0f) ||
						(parameter.Bevel > 0.0f)) {
						return true;
				}
				return false;
		}

		public void RegisterListener (MonoBehaviour go)
		{
				if (null == m_changeListener) {
						m_changeListener = new List<MonoBehaviour> ();
				}
				m_changeListener.Add (go);
		}

		public void UnRegisterListener (MonoBehaviour go)
		{
				if (null != m_changeListener) {
						if (m_changeListener.Contains (go)) {
								m_changeListener.Remove (go);
						}
				}
		}

		private void clearChildren ()
		{
				// Debug.Log("clearChildren()");
				for (int k=transform.childCount -1; k >= 0; k--) {
						GameObject go = transform.GetChild (k).gameObject;
						Renderer r = go.GetComponent<Renderer> ();
						r.enabled = false;
						MeshFilter mf = go.GetComponent<MeshFilter> ();
						mf.sharedMesh = null;
						if (Application.isPlaying) {
								Destroy (r);
								Destroy (mf);
								Destroy (go);
						} else {
								DestroyImmediate (r);
								DestroyImmediate (mf);
								DestroyImmediate (go);
						}
				}
				Resources.UnloadUnusedAssets ();
		}

		private void UpdateGlyphs ()
		{
				clearChildren ();
				if (parameter.Fontname.Length > 4) {
						if (Is3D ()) {
								if (null == m_fontInfo) {
										m_fontInfo = new VFontInfo (parameter.Fontname);
								}
								m_fontInfo.CreateText3D (this, RenderText);
						} else {
								// use common fontinfo
								VFontInfo fi = VFontHash.GetFontInfo (parameter.Fontname);
								fi.CreateText3D (this, RenderText);
						}
						if (null != m_changeListener) {
								foreach (MonoBehaviour mb in m_changeListener) {
										mb.SendMessage ("VTextChanged");
								}
						}
				}
		}

		private void UpdateLayout ()
		{
				// Debug.Log("ug " + parameter.fontname);
				if (parameter.Fontname.Length > 4) {
						if (Is3D ()) {
								if (null != m_fontInfo) {
										m_fontInfo.LayoutText3D (this, RenderText);
								}
						} else {
								// use common fontinfo
								VFontInfo fi = VFontHash.GetFontInfo (parameter.Fontname);
								if (null != fi) {
										fi.LayoutText3D (this, RenderText);
								}
						}
				}
		}

		public void Rebuild ()
		{
				if (null != m_fontInfo) {
						m_fontInfo = null;
				}
				UpdateGlyphs ();
		}

		public Bounds GetBounds ()
		{
				Bounds r = new Bounds ();
				if (null != m_fontInfo) {
						return m_fontInfo.GetBounds (this, RenderText);
				}
				return r;
		}

		/// <summary>
		/// Checks the rebuild.
		/// 
		/// use by Editor only
		/// </summary>
		/// <param name="updateMesh">If set to <c>true</c> update mesh.</param>
		/// <param name="updateLayout">If set to <c>true</c> update layout.</param>
		public void CheckRebuild (bool updateMesh, bool updateLayout)
		{
				parameter.CheckClearModified ();
				layout.CheckClearModified ();
				if (updateMesh) {
						// Debug.Log("update mesh");
						Rebuild ();
				} else if (updateLayout) {
						// Debug.Log("update layout");
						UpdateLayout ();
				}
		}

		void Update ()
		{
				if (parameter.CheckClearModified ()) {
						// Debug.Log("param update mesh: " + RenderText);
						layout.CheckClearModified ();
						Rebuild ();
				} else if (m_oldText != RenderText) {
						// Debug.Log("string update mesh: " + RenderText);
						layout.CheckClearModified ();
						UpdateGlyphs ();
				} else if (layout.CheckClearModified ()) {
						// Debug.Log("layout update mesh: " + RenderText);
						UpdateLayout ();
						// UpdateGlyphs();
				}
				m_oldText = RenderText;
		}
}
