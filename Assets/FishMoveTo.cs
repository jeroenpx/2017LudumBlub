using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMoveTo : MonoBehaviour {

	private Rigidbody2D rigid;

	public Vector3 goal;

	public FishLook fishLook;

	public float lazyDist;

	private bool active = false;

	public float distanceAcceleration = 10f;
	public float maxAcceleration = 10f;
	public float slowDownSidewaysFactor = 1f;

	public float sleepVelocity = 0.1f;
	public float sleepDist = 0.1f;

	// Use this for initialization
	void Start () {
		rigid = GetComponent<Rigidbody2D> ();
		goal = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 goal3D = goal - transform.position;
		Vector2 deltaGoal = new Vector2(goal3D.x, goal3D.y);
		float goalDist = deltaGoal.magnitude;
		Vector2 goalNormalized = deltaGoal.normalized;
		Vector2 normalizedOrtho = new Vector2 (goalNormalized.y, goalNormalized.x);
		if (goalDist > lazyDist) {
			// We are too far away... Start swimming.
			active = true;
		}
		if (active) {
			// Check whether we have a correct velocity
			Vector2 velocity = rigid.velocity;
			float velocityInSameDirection = Vector2.Dot (goalNormalized, velocity);
			float horizontalVelocity = Vector2.Dot (normalizedOrtho, velocity);

			float acceleration = goalDist * distanceAcceleration - velocityInSameDirection;
			if (acceleration < 0) {
				acceleration = 0;
			}
			if (acceleration > maxAcceleration) {
				acceleration = maxAcceleration;
			}

			float orthoAcceleration = -slowDownSidewaysFactor * goalDist * distanceAcceleration * horizontalVelocity;

			Vector2 totalForce = goalNormalized * acceleration + normalizedOrtho * orthoAcceleration;
			rigid.AddForce (totalForce);

			float speed = velocity.magnitude;
			if (speed < sleepVelocity && goalDist < sleepDist) {
				active = false;
			}

			fishLook.lookDirection = goalNormalized;
		} else {
			fishLook.lookDirection = Vector2.zero;
		}


	}
}
