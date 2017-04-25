using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceController : MonoBehaviour {
	
	private WaterForceControl wfc;
	private Vector2 lastpos2D;

	public float forcefactor = 100f;

	public bool touchdown = false;

	private bool mobileControls = false;

	// Use this for initialization
	void Start () {
		wfc = GameObject.FindGameObjectWithTag ("fluidsim").GetComponent<WaterForceControl>();

		mobileControls = false;
		#if UNITY_ANDROID
		#if UNITY_EDITOR
		#else
		mobileControls = true;
		#endif
		#endif
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 mousepos = Vector3.zero;
		bool touching = false;
		bool wastouchdown = touchdown;
		touchdown = false;

		if (!mobileControls) {
			mousepos = Input.mousePosition;
			touching = Input.GetKey (KeyCode.Mouse0);
			touchdown = true;
		} else {
			if (Input.touchCount > 0) {
				Touch touch = Input.GetTouch (0); // get first touch since touch count is greater than zero

				if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved) {
					mousepos = touch.position;
					touchdown = true;
					if (wastouchdown) {
						touching = true;
					}
				}
			}
		}

		Vector3 worldpos = Camera.main.ScreenToWorldPoint (mousepos);
		Vector2 worldpos2D = new Vector2 (worldpos.x, worldpos.y);

		if (touching) {
			wfc.PushAt (lastpos2D, (worldpos2D - lastpos2D) * forcefactor);
		}
		lastpos2D = worldpos2D;
	}
}
