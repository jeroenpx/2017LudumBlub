using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterParticles : MonoBehaviour {

	private ParticleSystem psys;
	private WaterForceControl wfc;

	private ParticleSystem.Particle[] particles = new ParticleSystem.Particle[1000];

	// Use this for initialization
	void Start () {
		psys = GetComponent<ParticleSystem> ();
		wfc = GameObject.FindGameObjectWithTag ("fluidsim").GetComponent<WaterForceControl>();

	}
	
	// Update is called once per frame
	void Update () {
		//psys.Emit (1);
		int size = psys.GetParticles(particles);

		for (int i = 0; i < size && i<1000; i++) {
			Vector3 pos3D = particles [i].position;
			particles [i].velocity = wfc.GetVelocityAt (new Vector2(pos3D.x, pos3D.y))*20f;
		}


		psys.SetParticles (particles, size);
	}
}
