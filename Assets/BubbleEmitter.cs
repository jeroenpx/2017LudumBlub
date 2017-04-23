using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleEmitter : MonoBehaviour {

	private WaterForceControl wfc;

	public float howmuch = 1f;
	public float secdelay = 1f;

	// Use this for initialization
	void Start () {
		wfc = GameObject.FindGameObjectWithTag ("fluidsim").GetComponent<WaterForceControl>();
		StartCoroutine (Run());
	}

	IEnumerator Run() {
		while (true) {
			wfc.MakeBubbles(transform.position, howmuch);
			yield return new WaitForSeconds(Random.value*secdelay);
		}
	}
}
