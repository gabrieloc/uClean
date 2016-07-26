using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
	
	void OnCollisionEnter (Collision collision)
	{
		Destroy (gameObject);

		var hit = collision.gameObject;
		var player = hit.GetComponent<PlayerController> ();
		if (player != null) {
			player.TakeDamage (10);
		}
	}
}
