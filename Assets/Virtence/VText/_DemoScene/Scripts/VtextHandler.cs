using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class VtextHandler : MonoBehaviour
{
		public VTextInterface vti_time = null;
		public VTextInterface vti_textOptions = null;
		public VTextInterface vti_textured = null;
		
		//heading
		private int old_headingValue;
		public static int headingValue;
		// size
		private float oldSizeValue;
		private float minSize = 0.45f;
		private float maxSize = 1.0f;
		public static float sizeValue;
		//depth
		private float oldDepthValue;
		private float minDepth = 0.0f;
		private float maxDepth = 3.0f;
		public static float depthValue;
		//bevel
		private float minBevel = 0.0f;
		private float maxBevel = 0.1f;
		private float oldBevelValue;
		public static float bevelValue;
		//font type
		public static int fontValue;
		private int oldFontValue;
		public string[] fontnames ;
		//public Font[] availiableFonts;


		/// <summary>
		/// Awake this instance.
		/// </summary>
		void Awake ()
		{ 
				fillFonts ();
				// init font type
				fontValue = 0;
				oldFontValue = -1;

				vti_time.layout.UseLightProbes = true;
				vti_textOptions.layout.UseLightProbes = true;
				vti_textured.layout.UseLightProbes = true;
		
				// intit alignment
				headingValue = 1;
				old_headingValue = headingValue;

				// init size
				sizeValue = .4f;
				oldSizeValue = -1;

				// init depth
				depthValue = .1f;
				oldDepthValue = -1;

				// init bevel
				bevelValue = .6f;
				oldBevelValue = -1;

				// init font type
				fontValue = 0;
				oldFontValue = -1;

			
		}

		void fillFonts ()
		{
				DirectoryInfo di = new DirectoryInfo (System.IO.Path.Combine (Application.streamingAssetsPath, "Fonts"));
				FileInfo[] fiarray = di.GetFiles ("*.*");
				fontnames = new string[3];
				int i = 0;
				// check if are at least 3 fonts installed
				foreach (FileInfo fi in fiarray) {
						if (".ttf" == fi.Extension || ".otf" == fi.Extension) {
								fontnames [i] = fi.Name;
								i++;
								if (i > 2) {
										return;
								}
						} 
				}
				Debug.LogError ("You must install at least 3 different fonts!");
		}


		/// <summary>
		/// Enable or disable light probes both VText odbjects in scene
		/// </summary>
		/// <value>The handle lightprobes.</value>
		void MessageLightprobes (bool lp)
		{
				if (vti_time != null) {
						vti_time.layout.UseLightProbes = lp;
						vti_textOptions.layout.UseLightProbes = lp;
						vti_textured.layout.UseLightProbes = lp;
				}
		}

		/// <summary>
		/// change font type of vti_textOptions
		/// </summary>
		void SetFontType ()
		{
				if (vti_textOptions != null) {
						if (fontValue < fontnames.Length) {
								if (fontnames [fontValue] != null) {
										vti_textOptions.parameter.Fontname = fontnames [fontValue];
								} else {
										Debug.LogError ("You must install at least 3 different fonts!");
								}
						}
						oldFontValue = fontValue;
				}
		}

		/// <summary>
		/// change size of vti_textOptions
		/// </summary>
		void SetSize ()
		{
				if (vti_textOptions != null) {
						vti_textOptions.layout.Size = minSize + sizeValue * (maxSize - minSize);
						oldSizeValue = sizeValue;
				}
		}

		/// <summary>
		/// change depth of vti_textOptions
		/// </summary>
		void SetDepth ()
		{
				if (vti_textOptions != null) {
						vti_textOptions.parameter.Depth = minDepth + depthValue * (maxDepth - minDepth);
						oldDepthValue = depthValue;
				}
		}

		/// <summary>
		/// change bevel of vti_textOptions
		/// </summary>
		void SetBevel ()
		{
				if (vti_textOptions != null) {
						vti_textOptions.parameter.Bevel = minBevel + bevelValue * (maxBevel - minBevel);
						oldBevelValue = bevelValue;
				}
		}

		/// <summary>
		/// change alignment of vti_textOptions
		/// </summary>
		void SetHeading ()
		{
				if (vti_textOptions != null) {
						switch (headingValue) {
						case 0:
								vti_textOptions.layout.Major = VTextLayout.align.Start;
								break;
						case 1:
								vti_textOptions.layout.Major = VTextLayout.align.Center;
								break;
						case 2:
								vti_textOptions.layout.Major = VTextLayout.align.End;
								break;
						default:
								Debug.Log ("HandleHeading, value not defined");
								break;
						}
						vti_textOptions.Rebuild ();
						TransformTxt (headingValue);
		
				}
		}

		/// <summary>
		/// change transformation of vti_textOptions in dependence of the current alignment to avoid shifts
		/// </summary>
		/// <param name="layout">Layout.</param>
		void TransformTxt (int layout)
		{
				float step = 0.25f * Mathf.Abs (old_headingValue - layout);
				int direction = 0;
				if (old_headingValue < layout) {
						direction = -1;
				} else {
						direction = 1;
				}
				float boundX = (vti_textOptions.GetBounds ().size.x) * step * direction;
				vti_textOptions.transform.localPosition = new Vector3 (vti_textOptions.transform.localPosition.x - (boundX),
	                                                       vti_textOptions.transform.localPosition.y,
	                                                       vti_textOptions.transform.localPosition.z);
				old_headingValue = layout;
		}

		/// <summary>
		/// Update this instance.
		/// </summary>
		void Update ()
		{

				if (old_headingValue != headingValue) {
						SetHeading ();
				}

				if (oldSizeValue != sizeValue) {
						SetSize ();
				}

				if (oldDepthValue != depthValue) {
						SetDepth ();
				}

				if (oldBevelValue != bevelValue) {
						SetBevel ();
				}

				if (oldFontValue != fontValue) {
						SetFontType ();
				}
		}
}
