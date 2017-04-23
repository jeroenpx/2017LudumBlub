using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceController : MonoBehaviour {
	
	private WaterForceControl wfc;
	private Vector2 lastpos2D;

	public float forcefactor = 100f;

	// Use this for initialization
	void Start () {
		wfc = GameObject.FindGameObjectWithTag ("fluidsim").GetComponent<WaterForceControl>();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 mousepos = Input.mousePosition;
		bool touching = Input.GetKey (KeyCode.Mouse0);

		if (Input.touchCount > 0) {
			Touch touch = Input.GetTouch (0); // get first touch since touch count is greater than zero

			if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved) {
				mousepos = touch.position;
				touching = true;
			}
		}

		Vector3 worldpos = Camera.main.ScreenToWorldPoint (mousepos);
		Vector2 worldpos2D = new Vector2 (worldpos.x, worldpos.y);
		//Debug.Log (worldpos2D);

		if (touching) {
			wfc.PushAt (worldpos2D, (worldpos2D - lastpos2D) * forcefactor);//new Vector2(100, 0));//;
		}
		lastpos2D = worldpos2D;
	}
}
