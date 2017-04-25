using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodData : MonoBehaviour {

	public void Eat() {
		ScoreKeeper.CaughtFish ();
		Destroy (gameObject);
	}
}
