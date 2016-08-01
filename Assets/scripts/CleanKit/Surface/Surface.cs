using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	
	public class Surface : MonoBehaviour
	{
		public static int LayerMask { get { return 1 << UnityEngine.LayerMask.NameToLayer ("Surface"); } }

		Material surfaceMaterial;
		Vector3 disclosurePoint = Vector3.zero;
		float lastHighlightUpdate = 0.0f;

		void Start ()
		{
			gameObject.layer = UnityEngine.LayerMask.NameToLayer ("Surface");

			Shader surfaceShader = Shader.Find ("CleanKit/Surface");
			surfaceMaterial = new Material (surfaceShader);
			Renderer renderer = GetComponent<Renderer> ();
			List<Material> materials = new List<Material> (renderer.materials);
			materials.Add (surfaceMaterial);
			renderer.materials = materials.ToArray ();
		}

		void Update ()
		{
			Vector4 highlightVector = new Vector4 (
				                          disclosurePoint.x, 
				                          disclosurePoint.y,
				                          disclosurePoint.z);
			surfaceMaterial.SetVector ("_highlight", highlightVector);

			float alpha = lastHighlightUpdate / 20.0f;
			Color highlightColor = new Color (1, 0, 1, alpha);
			surfaceMaterial.SetColor ("_color", highlightColor);

			lastHighlightUpdate--;
		}

		public void DisclosePoint (Vector3 point)
		{
			disclosurePoint = point;
			lastHighlightUpdate = 20.0f;
		}
	}
}
