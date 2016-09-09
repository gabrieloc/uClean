using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CleanKit
{
	[ExecuteInEditMode]
	public class Splashable : MonoBehaviour
	{

		void OnEnable ()
		{
			var renderer = GetComponent<Renderer> ();
			var materials = new List<Material> (renderer.sharedMaterials);
			var shader = Shader.Find ("CleanKit/Puddle");

			if (materials.Exists (m => m.shader.Equals (shader)) == false) {
				var puddleMaterial = new Material (shader);
				materials.Add (puddleMaterial);
				renderer.materials = materials.ToArray ();
			}
		}
	}
}
