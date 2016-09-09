using UnityEngine;
using System.Collections;

namespace CleanKit
{
	[ExecuteInEditMode]
	public class Cutout : MonoBehaviour
	{
		public Shader ReplacementShader;

		void OnEnable ()
		{
			if (ReplacementShader != null) {
				GetComponent<Camera> ().SetReplacementShader (ReplacementShader, "RenderType");
			}		
		}

		void OnDisable ()
		{
			GetComponent<Camera> ().ResetReplacementShader ();
		}
	}
}
