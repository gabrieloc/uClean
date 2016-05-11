using UnityEngine;
using System.Collections;

public class PropController : MonoBehaviour 
{
	public int initialSpawn = 10;
	public float displacement = 2.0f;

	void Start()
	{
		for (int index = 0; index < initialSpawn; index++) {
			GameObject prop = CleanKit.CreateTestProp();
			prop.transform.SetParent(transform, false);
			prop.transform.position = new Vector3((Random.value + 1) * displacement * (Random.value > 0.5 ? 1 : -1), 5, (Random.value + 1) * displacement * (Random.value > 0.5 ? 1 : -1));
		}
	}

	void Update()
	{
		
	}
}
