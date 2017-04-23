using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodTrigger : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D coll) {
		GameObject.Destroy (gameObject);
	}
}
