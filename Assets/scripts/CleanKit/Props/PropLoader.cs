using UnityEngine;
using System.Collections.Generic;

namespace CleanKit
{
	public static class PropLoader
	{
		public static GameObject CreateTestProp ()
		{
			GameObject testProp = GameObject.Instantiate (Resources.Load ("TestProp"), new Vector3 (), new Quaternion ()) as GameObject;
			return testProp;
		}

		public static GameObject CreateProp (PropInfo propInfo, Transform parentTransform)
		{
			GameObject resource = Resources.Load<GameObject> ("Props/" + propInfo.name);
			Quaternion rotation = propInfo.Rotation;

			float displacement = 5.0f;
			Vector3 position = new Vector3 (
				                   Random.Range (-1, 1) * displacement,
				                   1.0f, 
				                   Random.Range (-1, 1) * displacement);			
				
			GameObject prop = GameObject.Instantiate (resource, position, rotation) as GameObject;
			prop.transform.localEulerAngles = new Vector3 (-90.0f, 0.0f, 0.0f);
			prop.AddComponent<Rigidbody> ();
			prop.AddComponent<NavMeshObstacle> ();
			prop.name = resource.name;

			MeshCollider collider = prop.AddComponent<MeshCollider> ();
			collider.convex = true;

			prop.transform.SetParent (parentTransform);

			Interactable interactable = prop.AddComponent<Interactable> ();
			interactable.SetPreferredPosition (propInfo.Position);

			return prop;
		}
	}
}