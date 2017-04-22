using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterAreaAffector : MonoBehaviour {

	private WaterForceControl wfc;

	public Collider2D area;
	public Vector2 velocity;

	private WaterForceControl.Pixel[] pixels;

	// Use this for initialization
	void Start () {
		wfc = GameObject.FindGameObjectWithTag ("fluidsim").GetComponent<WaterForceControl>();
		wfc.affectors += DoAffector;
	}

	// Update is called once per frame
	void DoAffector () {
		if (pixels == null) {
			pixels = wfc.ToPixels (area);
		}

		foreach(WaterForceControl.Pixel pix in pixels) {
			wfc.AddVelocity (pix, velocity);
		}
	}
}
