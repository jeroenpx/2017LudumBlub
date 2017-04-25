using UnityEngine;
using System.Collections;

public class ObjectRotator : MonoBehaviour {
	
	[SerializeField]
	private float speed=1f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Quaternion additionalRot = Quaternion.AngleAxis(speed*Time.deltaTime, new Vector3(0f, 1f, 0f));
		transform.rotation*=additionalRot;
	}
}
