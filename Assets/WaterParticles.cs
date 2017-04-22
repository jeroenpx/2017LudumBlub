using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterParticles : MonoBehaviour {

	private ParticleSystem psys;
	private WaterForceControl wfc;

	private const int MAXPARTICLE = 2000;

	public bool useAcceleration = false;
	public Vector2 acceleration;
	public float maxSpeed;

	private ParticleSystem.Particle[] particles = new ParticleSystem.Particle[MAXPARTICLE];

	// Use this for initialization
	void Start () {
		psys = GetComponent<ParticleSystem> ();
		wfc = GameObject.FindGameObjectWithTag ("fluidsim").GetComponent<WaterForceControl>();

	}
	
	// Update is called once per frame
	void Update () {
		//psys.Emit (1);
		int size = psys.GetParticles(particles);

		Vector2 speed = Vector2.zero;
		for (int i = 0; i < size && i<MAXPARTICLE; i++) {
			
			if(useAcceleration) {
				// Innefficient
				float lifedone = particles [i].startLifetime - particles [i].remainingLifetime;
				speed = lifedone*acceleration;
				float totalspeed = speed.magnitude;
				if (totalspeed > maxSpeed) {
					speed *= maxSpeed / totalspeed;
				}
			}
			//acceleration
			Vector3 pos3D = particles [i].position;
			particles [i].velocity = wfc.GetVelocityAt (new Vector2(pos3D.x, pos3D.y))*20f+speed;
		}


		psys.SetParticles (particles, size);
	}
}
