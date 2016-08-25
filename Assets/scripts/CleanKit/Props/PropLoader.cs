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

		public static GameObject CreateProp (PropInfo propInfo)
		{
			GameObject resource = Resources.Load<GameObject> ("Props/" + propInfo.name);
			Vector3 position = propInfo.Position;
			Quaternion rotation = propInfo.Rotation;

			GameObject prop = GameObject.Instantiate (resource, position, rotation) as GameObject;
			prop.transform.localEulerAngles = new Vector3 (-90.0f, 0.0f, 0.0f);
			prop.AddComponent<Rigidbody> ();
			prop.AddComponent<NavMeshObstacle> ();
			prop.name = resource.name;

			MeshCollider collider = prop.AddComponent<MeshCollider> ();
			collider.convex = true;

			// Add the Interactable component last, since it assumes a configured gameObject
			Interactable interactable = prop.AddComponent<Interactable> ();
			interactable.CreateDestination (propInfo.DestinationPosition);

			return prop;
		}
	}
}