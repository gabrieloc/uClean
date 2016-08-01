using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	
	public class Surface : MonoBehaviour
	{
		public static int LayerMask { get { return 1 << UnityEngine.LayerMask.NameToLayer ("Surface"); } }

		Vector3 disclosurePoint = Vector3.zero;

		void Start ()
		{
			gameObject.layer = UnityEngine.LayerMask.NameToLayer ("Surface");

			Material surfaceMaterial = new Material (Shader.Find ("CleanKit/Surface"));
			Renderer renderer = GetComponent<Renderer> ();
			List<Material> materials = new List<Material> (renderer.materials);
			materials.Add (surfaceMaterial);
			renderer.materials = materials.ToArray ();
		}

		void Update ()
		{
			if (disclosurePoint != Vector3.zero) {
					
			}
		}

		public void DisclosePoint (Vector3 point)
		{
			disclosurePoint = point;
		}
	}
}
