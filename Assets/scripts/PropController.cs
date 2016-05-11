using UnityEngine;
using System.Collections;

public class PropController : MonoBehaviour 
{
	public int initialSpawn = 1;

	void Start()
	{
		for (int index = 0; index < initialSpawn; index++) {
			GameObject prop = CleanKit.CreateTestProp();
			prop.transform.SetParent(transform, false);
			prop.transform.position = new Vector3(0, 2, 2);
		}
	}

	void Update()
	{
		
	}
}
