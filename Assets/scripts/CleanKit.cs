using UnityEngine;
using System.Collections.Generic;

public static class CleanKit
{
	public static GameObject CreateTestProp()
	{
		GameObject testProp = GameObject.CreatePrimitive(PrimitiveType.Cube);
		testProp.AddComponent<Rigidbody>();
		testProp.GetComponent<Renderer>().material.color = Color.cyan;
		testProp.transform.localScale = new Vector3(2, 5, 2);
		testProp.name = "Prop";
		testProp.tag = "liftable";
		return testProp;
	}
}

