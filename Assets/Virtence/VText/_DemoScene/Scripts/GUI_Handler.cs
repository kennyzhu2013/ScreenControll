using UnityEngine;
using System.Collections;

public class GUI_Handler : MonoBehaviour {
	public GameObject VTI_handler_object;
	private bool enableLightProbes;

	private string[] HeadingTXT = {"Left", "Center", "Right"};
	private string[]FontTXT = {"Font1", "Font2", "Font3"};


	void Awake(){
		VTI_handler_object = GameObject.Find ("_VTextHandlerScript");
		enableLightProbes = true;
		VTI_handler_object.SendMessage("MessageLightprobes", enableLightProbes);
		}

	void OnGUI () {
		//VtextHandler vtextHandler = VTI_handler_object.GetComponent<VtextHandler> ();

		GUILayout.BeginArea(new Rect(5, Screen.height - 260, 180, 300));
		GUILayout.BeginVertical ("Box");

		if(GUILayout.Button("Light probes", GUILayout.Width(100))) {
			if(VTI_handler_object != null){
				enableLightProbes = !enableLightProbes;
				// Debug.Log("Enable Light Probes: " + enableLightProbes);
				VTI_handler_object.SendMessage("MessageLightprobes", enableLightProbes);
			}
		}


		GUILayout.Label ("Alignment");
		VtextHandler.headingValue = GUILayout.Toolbar (VtextHandler.headingValue, HeadingTXT);

		GUILayout.Label ("Fonts");
		VtextHandler.fontValue = GUILayout.Toolbar (VtextHandler.fontValue, FontTXT);

		GUILayout.Label ("Size");
		VtextHandler.sizeValue = GUILayout.HorizontalSlider (VtextHandler.sizeValue, 0.0f, 1.0f);

		GUILayout.Label ("Depth");
		VtextHandler.depthValue = GUILayout.HorizontalSlider (VtextHandler.depthValue, 0.0f, 1.0f);

		GUILayout.Label ("Bevel");
		VtextHandler.bevelValue = GUILayout.HorizontalSlider (VtextHandler.bevelValue, 0.0f, 1.0f);

		GUILayout.EndVertical ();
		GUILayout.EndArea();
	




	}

}
