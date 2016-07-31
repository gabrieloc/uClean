using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	
	public class Surface : MonoBehaviour
	{
		// Use this for initialization
		void Start ()
		{
			Material surfaceMaterial = new Material (Shader.Find ("CleanKit/Surface"));


			Renderer renderer = GetComponent<Renderer> ();
			List<Material> materials = new List<Material> (renderer.materials);
			materials.Add (surfaceMaterial);
			renderer.materials = materials.ToArray ();
		}
	
		// Update is called once per frame
		void Update ()
		{
			
		}
	}
}
