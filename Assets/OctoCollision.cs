using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctoCollision : MonoBehaviour {

	public float live = 1f;
	public float inktrigger = 0.1f;
	public int emitcount = 10;

	private ParticleSystem psys;

	// Use this for initialization
	void Start () {
		psys = transform.Find ("InkEmiter").GetComponent<ParticleSystem> ();
	}

	void OnCollisionEnter2D(Collision2D coll) {
		float collspeed = coll.relativeVelocity.magnitude;
		if (collspeed > inktrigger) {
			live -= collspeed;
			psys.Emit (emitcount);
		}
	}
}
