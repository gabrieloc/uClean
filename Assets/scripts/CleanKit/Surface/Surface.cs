using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	
	public class Surface : MonoBehaviour
	{
		public static int LayerMask { get { return 1 << UnityEngine.LayerMask.NameToLayer ("Surface"); } }

		Material surfaceMaterial;
		Vector3? disclosurePoint;

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
			float cellSize = Grid.CellSize;
			surfaceMaterial.SetFloat ("_size", cellSize);


			Color highlightColor;
			if (disclosurePoint.HasValue) {
				Vector3 point = disclosurePoint.Value;

				highlightColor = Color.blue;

				Vector4 highlightVector = new Vector4 (
					                          point.x, 
					                          point.y,
					                          point.z);
				surfaceMaterial.SetVector ("_highlight", highlightVector);
			} else {
				highlightColor = Color.clear;
			}
			surfaceMaterial.SetColor ("_color", highlightColor);
		}

		public Vector3 DisclosePoint (Vector3 point)
		{
			disclosurePoint = point;

			Vector3 disclosePoint = Grid.ClosestIntersectingPoint (point);
			return disclosePoint;
		}

		public void Undisclose ()
		{
			disclosurePoint = null;
		}
	}
}
