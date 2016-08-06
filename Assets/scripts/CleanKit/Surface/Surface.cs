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

			for (int i = 0; i < vertList.Count; i++) {
				int i2 = (i + 1) < vertList.Count ? i + 1 : 0;
				Debug.DrawLine (vertList [i], vertList [i2], Color.green);
			}
		}

		public Vector3 DisclosePoint (Vector3 point)
		{
			disclosurePoint = Grid.ClosestIntersectingPoint (point);
			return disclosurePoint.Value;
		}

		List<Vector3> vertList = new List<Vector3> ();

		public Vector3 DiscloseCells (Vector3 point, Bounds bounds)
		{
			Vector3 s = bounds.size;
			Vector3[] verts = new Vector3[4];
			verts [0] = bounds.center + new Vector3 (s.x, -s.y, s.z) * 0.5f;
			verts [1] = bounds.center + new Vector3 (-s.x, -s.y, s.z) * 0.5f;
			verts [2] = bounds.center + new Vector3 (-s.x, -s.y, -s.z) * 0.5f;
			verts [3] = bounds.center + new Vector3 (s.x, -s.y, -s.z) * 0.5f;
			vertList = verts.ToList ();

			return DisclosePoint (point);
		}

		public void Undisclose ()
		{
			disclosurePoint = null;
		}
	}
}
