using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CleanKit
{
	
	public class Surface : MonoBehaviour
	{
		public static int LayerMask { get { return 1 << UnityEngine.LayerMask.NameToLayer ("Surface"); } }

		Material surfaceMaterial;
		Vector3? disclosurePoint;
		Vector3? disclosureSize;
		bool disclosureValid;

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
			if (disclosurePoint.HasValue && disclosureSize.HasValue) {
				
				highlightColor = disclosureValid ? Color.blue : Color.red;

				Vector3 position = disclosurePoint.Value;
				Vector4 highlightPosition = new Vector4 (position.x, position.y, position.z);
				surfaceMaterial.SetVector ("_highlightPosition", highlightPosition);

				Vector3 size = disclosureSize.Value;
				Vector4 highlightSize = new Vector4 (size.x, size.y, size.z);
				surfaceMaterial.SetVector ("_highlightSize", highlightSize);
			} else {
				highlightColor = Color.gray;
				surfaceMaterial.SetVector ("_highlightPosition", Vector4.zero);
				surfaceMaterial.SetVector ("_highlightSize", Vector4.zero);
			}
			surfaceMaterial.SetColor ("_color", highlightColor);
		}

		public Vector3 DisclosePoint (Vector3 point, bool valid)
		{
			disclosurePoint = Grid.ClosestIntersectingPoint (point);
			disclosureValid = valid;
			return disclosurePoint.Value;
		}

		public Vector3 DiscloseCells (Vector3 point, bool valid, Vector3 size)
		{
			disclosureSize = Grid.ClosestIntersectingCell (size);
			return DisclosePoint (point, valid);
		}

		public void Undisclose ()
		{
			disclosurePoint = null;
		}
	}
}
