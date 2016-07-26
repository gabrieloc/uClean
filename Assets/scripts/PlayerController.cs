using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
	public GameObject bulletPrefab;
	public Transform bulletSpawn;

	public override void OnStartLocalPlayer ()
	{
		GetComponent<MeshRenderer> ().material.color = Color.blue;
	}

	void Update ()
	{
		if (!isLocalPlayer) {
			return;	
		}

		float x = Input.GetAxis ("Horizontal") * Time.deltaTime * 150.0f;
		float z = Input.GetAxis ("Vertical") * Time.deltaTime * 3.0f;

		transform.Rotate (0, x, 0);
		transform.Translate (0, 0, z);

		if (Input.GetKeyDown (KeyCode.Space)) {
			CmdFire ();
		}
	}

	public const int maxHealth = 100;
	public int currentHealth = maxHealth;
	public RectTransform healthBar;

	public void TakeDamage (int amount)
	{
		currentHealth -= amount;
		if (currentHealth <= 0) {
			currentHealth = 0;
			print ("Dead");
		}

		healthBar.sizeDelta = new Vector2 (currentHealth, healthBar.sizeDelta.y);
	}

	[Command]
	void CmdFire ()
	{
		GameObject bullet = Instantiate (bulletPrefab, bulletSpawn.position, bulletSpawn.rotation) as GameObject;
		bullet.GetComponent<Rigidbody> ().velocity = bullet.transform.forward * 6.0f;
		NetworkServer.Spawn (bullet);
		Destroy (bullet, 2.0f);
	}
}
