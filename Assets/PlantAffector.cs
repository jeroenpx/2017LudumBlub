using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantAffector : MonoBehaviour {

	private WaterForceControl wfc;

	private Transform mytransf;
	private Rigidbody2D rigid;
	public float factor;

	// Use this for initialization
	void Start () {
		wfc = GameObject.FindGameObjectWithTag ("fluidsim").GetComponent<WaterForceControl>();
		mytransf = transform;
		rigid = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 velocity = wfc.GetVelocityAt (mytransf.position);
		rigid.AddForce (velocity * factor);
	}
}
