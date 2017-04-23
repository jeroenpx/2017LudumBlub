using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

	public float timebetween;

	public GameObject prefab;

	public Vector2 initVelocity;

	// Use this for initialization
	void Start () {
		StartCoroutine (Run ());
	}
	
	// Update is called once per frame
	IEnumerator Run () {
		while (true) {
			GameObject go = GameObject.Instantiate (prefab, transform.position, Quaternion.identity);
			go.GetComponent<Rigidbody2D> ().velocity = initVelocity;
			yield return new WaitForSeconds (timebetween);
		}
	}
}
