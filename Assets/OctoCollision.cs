using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctoCollision : MonoBehaviour {

	public float live = 1f;
	public float inktrigger = 0.1f;
	public int emitcount = 10;

	private ParticleSystem psys;

	public GameObject messageMmm;
	public GameObject messageOw;

	private float lastMessageTime;

	public float timeBetweenMessages = 1f;

	// Use this for initialization
	void Start () {
		psys = transform.Find ("InkEmiter").GetComponent<ParticleSystem> ();
		lastMessageTime = Time.timeSinceLevelLoad;
	}

	void OnCollisionEnter2D(Collision2D coll) {
		FoodData foodData = coll.gameObject.GetComponent<FoodData>();
		if (foodData != null) {
			// Eat fish!
			foodData.Eat();

			if (lastMessageTime + timeBetweenMessages < Time.timeSinceLevelLoad) {
				GameObject.Instantiate (messageMmm, transform.position, Quaternion.identity);
				lastMessageTime = Time.timeSinceLevelLoad;
			}
		} else {
			float collspeed = coll.relativeVelocity.magnitude;
			if (collspeed > inktrigger) {
				live -= collspeed;
				psys.Emit (emitcount);

				if(lastMessageTime + timeBetweenMessages < Time.timeSinceLevelLoad) {
					GameObject.Instantiate (messageOw, transform.position, Quaternion.identity);
					lastMessageTime = Time.timeSinceLevelLoad;
				}

			}
		}
	}
}
