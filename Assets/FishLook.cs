using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishLook : MonoBehaviour {

	private Vector3 lastPos;
	private Quaternion prepareForLookAt;

	public Vector3 lookDirection;

	public float anglesPerSecond = 0.1f;
	public float minSpeed = 0.2f;

	public bool auto;

	// Use this for initialization
	void Start () {
		lastPos = transform.position;
		prepareForLookAt = Quaternion.Inverse (Quaternion.LookRotation (Vector3.left, Vector3.back));
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 currentPos = transform.position;

		if (auto) {
			lookDirection = currentPos - lastPos;
		}
		float speed = lookDirection.magnitude;
		if (speed < minSpeed * Time.deltaTime) {
			return;// Do not change rotation
		}

		Quaternion lookAt = Quaternion.LookRotation (lookDirection, Vector3.back)*prepareForLookAt;

		float angle = Quaternion.Angle (lookAt, transform.localRotation);
		float percent = Mathf.Min(angle, anglesPerSecond* Time.deltaTime)/angle;

		#if UNITY_WEBGL
		transform.localRotation = lookAt;
		#elif UNITY_ANDROID
		transform.localRotation = lookAt;
		#else
		transform.localRotation = Quaternion.Slerp(transform.localRotation, lookAt, percent);
		#endif



		lastPos = currentPos;
	}
}
