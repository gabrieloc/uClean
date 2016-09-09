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

			Vector3 initial = propInfo.Position;
			Quaternion rotation = new Quaternion (
				                      Random.Range (0.0f, 1.0f),
				                      Random.Range (0.0f, 1.0f),
				                      Random.Range (0.0f, 1.0f),
				                      Random.Range (0.0f, 1.0f));
				
			var prop = GameObject.Instantiate (resource, initial, rotation) as GameObject;
			prop.AddComponent<Rigidbody> ();

			var obstacle = prop.AddComponent<NavMeshObstacle> ();
			obstacle.carving = true;

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