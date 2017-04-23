using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaterForceControl : MonoBehaviour {

	// Based on https://mikeash.com/pyblog/fluid-simulation-for-dummies.html


	public float h = 10;
	public int N = 10;
	public int solveriter = 4;

	// How fast stuff spreads throughout the fluid
	public float diffusion = .1f;

	// How "thick" the fluid is
	public float viscosity = .1f;

	// The pixels, calculated...
	private float halfh;
	private float[,] pixels_density;
	private float[,] pixels_s;
	private float[,] pixels_velocityx;
	private float[,] pixels_velocityy;
	private float[,] pixels_velocityInitialx;
	private float[,] pixels_velocityInitialy;
	private int[,] pixels_collidercount;
	private int[,] pixels_dynamiccount;
	private Vector2[,] pixels_initialvelocity;
	//private Vector2[,] pixels_deltavelocity;
	private Vector2[,] pixels_point3D;

	private float[,] tempdiv;
	private float[,] tempp;

	private DynObj[] dynamics;

	// Unity velocity => water velocity
	public float magicnumber = 0.001f;
	public float forcefeedback = 0.8f;

	//
	// Position Logic
	//

	public struct Pixel
	{
		public int x;
		public int y;

		public Pixel(int x, int y) {
			this.x = x;
			this.y = y;
		}
	}

	public struct DynObj
	{
		public Collider2D coll;
		public Rigidbody2D rig;
	}

	private Pixel PointToPixel(Vector2 point){
		int x = (int)((point.x - halfh) / h * N);
		int y = (int)((point.y - halfh) / h * N);
		if (x < 0) {
			x = 0;
		}
		if (x >= N) {
			x = N - 1;
		}
		if (y < 0) {
			y = 0;
		}
		if (y >= N) {
			y = N - 1;
		}

		return new Pixel (x, y);
	}

	//
	// Collider Logic
	//

	private void UpdateCollider(Collider2D coll, bool add) {
		if (coll.isTrigger || coll.gameObject.CompareTag("ignorecolliderfluid")) {
			return;
		}

		Bounds bounds = coll.bounds;

		Pixel pixmin = PointToPixel (new Vector2 (bounds.min.x, bounds.min.y));
		Pixel pixmax = PointToPixel (new Vector2 (bounds.max.x, bounds.max.y));
		for (int x = pixmin.x; x <= pixmax.x; x++) {
			for (int y = pixmin.y; y <= pixmax.y; y++) {
				Vector2 point = pixels_point3D[x,y];
				if (coll.OverlapPoint (point)) {
					if (add) {
						pixels_collidercount[x,y]++;
					} else {
						pixels_collidercount[x,y]--;
					}
				}
			}
		}
	}

	private void UpdateDynamic(Collider2D coll, Rigidbody2D r, bool add){
		if (coll.isTrigger) {
			return;
		}

		Bounds bounds = coll.bounds;

		Pixel pixmin = PointToPixel (new Vector2 (bounds.min.x, bounds.min.y));
		Pixel pixmax = PointToPixel (new Vector2 (bounds.max.x, bounds.max.y));

		int cellcount=0;
		bool foundone;

		// Normalize pixels_deltavelocity => divide by the number of cells
		for (int x = pixmin.x; x <= pixmax.x; x++) {
			for (int y = pixmin.y; y <= pixmax.y; y++) {
				Vector2 point = pixels_point3D [x, y];
				if (coll.OverlapPoint (point)) {
					cellcount++;
				}
			}
		}
		foundone = cellcount > 0;
		cellcount = (pixmax.x - pixmin.x + 1) * (pixmax.y - pixmin.y + 1);

		for (int x = pixmin.x; x <= pixmax.x; x++) {
			for (int y = pixmin.y; y <= pixmax.y; y++) {
				Vector2 point = pixels_point3D[x,y];
				if (coll.OverlapPoint (point) || !foundone) {
					Vector2 pointVelocity = r.GetPointVelocity (point)*h/N*magicnumber;
					if (add) {
						pixels_dynamiccount[x,y]++;
						pixels_initialvelocity[x, y] = pointVelocity/r.mass;
						//pixels_deltavelocity[x, y] = new Vector2();
						pixels_velocityx [x, y] = pixels_initialvelocity [x, y].x;
						pixels_velocityy [x, y] = pixels_initialvelocity [x, y].y;
					} else {
						pixels_dynamiccount[x,y]--;
						float xdiff = pixels_velocityx [x, y] - pixels_initialvelocity [x, y].x;
						float ydiff = pixels_velocityy [x, y] - pixels_initialvelocity [x, y].y;

						Vector2 deltavelocity = new Vector2 (xdiff*N/h/magicnumber, ydiff*N/h/magicnumber);
						r.AddForceAtPosition (deltavelocity/cellcount*r.mass*forcefeedback, point);
					}
				}
			}
		}
	}

	//
	// Initialisation
	//
	private void InitGrid () {
		halfh = -h/2f;

		float density = 1;

		pixels_density = new float[N, N];
		pixels_s = new float[N, N];
		pixels_velocityx = new float[N, N];
		pixels_velocityy = new float[N, N];
		pixels_velocityInitialx = new float[N, N];
		pixels_velocityInitialy = new float[N, N];
		pixels_collidercount = new int[N, N];
		pixels_dynamiccount =  new int[N, N];
		pixels_initialvelocity =  new Vector2[N, N];
		pixels_point3D = new Vector2[N, N];
		tempdiv = new float[N, N];
		tempp = new float[N, N];
		for (int x = 0; x < N; x++) {
			for (int y = 0; y < N; y++) {
				pixels_collidercount [x, y] = 0;
				pixels_density [x, y] = density;
				pixels_point3D [x, y] = new Vector2 (halfh+x*h/N, halfh+y*h/N);

				if (pixels_point3D [x, y].y < 0) {
					//pixels_velocityx [x, y] = 1f;
				}
			}
		}

		// Find all collider in the scene
		Collider2D[] colls = GameObject.FindObjectsOfType<Collider2D>();
		foreach (Collider2D coll in colls) {
			if (coll.gameObject.CompareTag ("dynamic")) {
				continue;
			}
			UpdateCollider (coll, true);
		}

		// Handle all dynamics
		GameObject[] dynamicsObj = GameObject.FindGameObjectsWithTag("dynamic");
		List<DynObj> collsDyn = new List<DynObj>();
		foreach(GameObject go in dynamicsObj) {
			DynObj dynObj = new DynObj ();
			dynObj.coll = go.GetComponent<Collider2D> ();
			dynObj.rig = go.GetComponent<Rigidbody2D> ();
			collsDyn.Add(dynObj);
		}
		dynamics = collsDyn.ToArray();
	}

	// what = 0 => diffuse
	// what = 1 => velocity x
	// what = 2 => velocity y
	private void UpdateBoundary(int what, float[,] items) {
		for (int x = 0; x < N; x++) {
			for (int y = 0; y < N; y++) {
				if (pixels_collidercount [x, y] > 0) {
					items [x, y] = 0f;
				}
			}
		}

		for (int x = 0; x < N; x++) {
			for (int y = 0; y < N; y++) {
				bool inwall = pixels_collidercount [x, y] > 0;
				bool indynamic = pixels_dynamiccount [x, y] > 0;
				if (inwall) {
					for (int xd = -1; xd <= 1; xd++) {
						for (int yd = -1; yd <= 1; yd++) {
							if (xd == 0 && yd == 0) {
								// Ignore
							} else {
								int xother = x + xd;
								int yother = y + yd;
								if (xother >= 0 && xother < N && yother >=0 && yother<N
									&& (xd == 0 || yd == 0) // ONLY HORIZONTAL & VERTICAL FLOW!
								) {
									// In bounds...
									if (pixels_collidercount[xother, yother] == 0) {
										// So, other is a water pixel and we are a wall
										float valuediff = 0f;
										int count = 0;
										if (xd != 0) {
											if(what == 1){// velocity x
												valuediff += -items [xother, yother];
												count++;
											} else {// other stuff (same value)
												valuediff += items[xother, yother];
												count++;
											}
										}
										if (yd != 0) {
											if (what == 2) {// velocity y
												valuediff += -items [xother, yother];
												count++;
											} else {// other stuff
												valuediff += items[xother, yother];
												count++;
											}
										}

										// Force back...
										items [x, y] += valuediff / count;
									}
								}
							}
						}
					}
				}
			}
		}
	}

	private void lin_solve(int b, float[,] xarr, float[,] xarr0, float a)
	{
		float cRecip = 1f / (1 + 4 * a);
		for (int k = 0; k < solveriter; k++) {
			for (int x = 1; x < N-1; x++) {
				for (int y = 1; y < N-1; y++) {
					xarr[x, y] =
						(xarr0[x, y]
							+ a*(xarr[x+1, y  ]
								+xarr[x-1, y  ]
								+xarr[x  , y+1]
								+xarr[x  , y-1]
								)) * cRecip;
				}
			}
			UpdateBoundary(b, xarr);
		}
	}

	private void diffuse(int b, float[,] initialData, float[,] data, float factor, float dt) {
		// TODO: WUT ?????? 
		float a = dt * factor * (N - 2);// * (N - 2);
		lin_solve(b, initialData, data, a);
	}

	// Fix the velocities
	private void project(float[,] velocX, float[,] velocY)
	{
		for (int x = 1; x < N-1; x++) {
			for (int y = 1; y < N-1; y++) {
				tempdiv[x, y] = -0.5f*(
					velocX[x+1, y  ]
					-velocX[x-1, y  ]
					+velocY[x  , y+1]
					-velocY[x  , y-1]
				)/N;
				tempp[x, y] = 0;
			}
		}
		UpdateBoundary(0, tempdiv); 
		UpdateBoundary(0, tempp);
		lin_solve(0, tempp, tempdiv, 1);

		for (int x = 1; x < N-1; x++) {
			for (int y = 1; y < N-1; y++) {
				velocX[x, y] -= 0.5f * (  tempp[x+1, y]
					-tempp[x-1, y]) * N;
				velocY[x, y] -= 0.5f * (  tempp[x, y+1]
					-tempp[x, y-1]) * N;
			}
		}
		UpdateBoundary(1, pixels_velocityx);
		UpdateBoundary(2, pixels_velocityy);
	}

	private void advect(int b, float[,] d, float[,] d0, float[,] velocX, float[,] velocY, float dt)
	{
		float i0, i1, j0, j1;

		float dtx = dt * (N - 2);
		float dty = dt * (N - 2);

		float s0, s1, t0, t1;
		float tmp1, tmp2, x, y;

		float Nfloat = N;
		float ifloat, jfloat;
		int i, j;

		for(j = 1, jfloat = 1; j < N - 1; j++, jfloat++) { 
			for(i = 1, ifloat = 1; i < N - 1; i++, ifloat++) {
				tmp1 = dtx * velocX[i, j];
				tmp2 = dty * velocY[i, j];
				x    = ifloat - tmp1; 
				y    = jfloat - tmp2;

				if(x < 0.5f) x = 0.5f; 
				if(x > Nfloat + 0.5f) x = Nfloat + 0.5f; 
				i0 = Mathf.Floor(x); 
				i1 = i0 + 1.0f;
				if(y < 0.5f) y = 0.5f; 
				if(y > Nfloat + 0.5f) y = Nfloat + 0.5f; 
				j0 = Mathf.Floor(y);
				j1 = j0 + 1.0f;

				s1 = x - i0; 
				s0 = 1.0f - s1; 
				t1 = y - j0; 
				t0 = 1.0f - t1;

				int i0i = (int)i0;
				int i1i = (int)i1;
				int j0i = (int)j0;
				int j1i = (int)j1;

				if (i0i < 0) {
					i0i = 0;
				}
				if (i1i < 0) {
					i1i = 0;
				}
				if (j0i < 0) {
					j0i = 0;
				}
				if (j1i < 0) {
					j1i = 0;
				}
				if (i0i >= N) {
					i0i = N-1;
				}
				if (i1i >= N) {
					i1i = N-1;
				}
				if (j0i >= N) {
					j0i = N-1;
				}
				if (j1i >= N) {
					j1i = N-1;
				}


				try{

					d[i, j] = 

						s0 * ( t0 * d0[i0i, j0i]
							+( t1 * d0[i0i, j1i]))
						+s1 * ( t0 * d0[i1i, j0i]
							+( t1 * d0[i1i, j1i]));
				} catch (IndexOutOfRangeException ioe){
					//Debug.Log ("Error at "+i+", "+j);
				}
			}
		}
		UpdateBoundary(b, d);
	}

	private void FluidStep () {
		foreach (DynObj dyn in dynamics) {
			Collider2D coll = dyn.coll;
			Rigidbody2D rig = dyn.rig;
			UpdateDynamic (coll, rig, true);
		}

		float dt = Time.deltaTime;

		if (affectors != null) {
			affectors ();
		}

		diffuse(1, pixels_velocityInitialx, pixels_velocityx, viscosity, dt);
		diffuse(2, pixels_velocityInitialy, pixels_velocityy, viscosity, dt);

		project(pixels_velocityInitialx, pixels_velocityInitialx);

		advect(1, pixels_velocityx, pixels_velocityInitialx, pixels_velocityInitialx, pixels_velocityInitialy, dt);
		advect(2, pixels_velocityy, pixels_velocityInitialy, pixels_velocityInitialx, pixels_velocityInitialy, dt);

		project(pixels_velocityx, pixels_velocityy);

		diffuse(0, pixels_s, pixels_density, diffusion, dt);
		advect(0, pixels_density, pixels_s, pixels_velocityx, pixels_velocityy, dt);


		foreach (DynObj dyn in dynamics) {
			Collider2D coll = dyn.coll;
			Rigidbody2D rig = dyn.rig;
			UpdateDynamic (coll, rig, false);
		}
	}

	// Use this for initialization
	void Start () {
		InitGrid ();

	}

	// Update is called once per frame
	void Update () {
		float crossx = 0.1f;
		for (int x = 0; x < N; x++) {
			for (int y = 0; y < N; y++) {
				Vector2 p = pixels_point3D [x, y];
				if (pixels_collidercount [x, y] > 0) {
					Debug.DrawLine (new Vector3 (p.x - crossx, p.y - crossx, 0), new Vector3 (p.x + crossx, p.y + crossx, 0), Color.red);
					Debug.DrawLine (new Vector3 (p.x + crossx, p.y - crossx, 0), new Vector3 (p.x - crossx, p.y + crossx, 0), Color.red);
				} else {
					float vx = pixels_velocityx [x, y];
					float vy = pixels_velocityy[x, y];
					Debug.DrawRay (new Vector3 (p.x, p.y, 0), new Vector3 (vx, vy, 0f));
				}
			}
		}
		FluidStep ();
	}

	//
	// Outside logic
	//
	public Vector2 GetVelocityAt(Vector2 pos) {
		Pixel pospix = PointToPixel (pos);
		float velox = pixels_velocityx [pospix.x, pospix.y];
		float veloy = pixels_velocityy [pospix.x, pospix.y];
		return new Vector2 (velox, veloy);
	}

	public void PushAt(Vector2 pos, Vector2 velocity) {
		Pixel pospix = PointToPixel (pos);
		pixels_velocityx [pospix.x, pospix.y] = velocity.x;
		pixels_velocityy [pospix.x, pospix.y] = velocity.y;

		MakeBubbles (pos-velocity.normalized*0.3f, 0.3f*velocity.magnitude);
	}

	//
	// Bubble Psystem
	//
	public ParticleSystem bubbleEmit;
	public Transform bubblePos;
	public float bubbleamount;

	public void MakeBubbles(Vector2 v, float howmuch) {
		bubblePos.position = new Vector3 (v.x, v.y, 0);
		bubbleEmit.Emit (UnityEngine.Random.Range(0, (int)(howmuch * bubbleamount)));
	}

	//
	// Area affectors
	//

	public Pixel[] ToPixels(Collider2D coll) {
		List<Pixel> pixels = new List<Pixel>();

		Bounds bounds = coll.bounds;

		Pixel pixmin = PointToPixel (new Vector2 (bounds.min.x, bounds.min.y));
		Pixel pixmax = PointToPixel (new Vector2 (bounds.max.x, bounds.max.y));
		for (int x = pixmin.x; x <= pixmax.x; x++) {
			for (int y = pixmin.y; y <= pixmax.y; y++) {
				Vector2 point = pixels_point3D[x,y];
				if (coll.OverlapPoint (point)) {
					pixels.Add (new Pixel (x, y));
				}
			}
		}

		return pixels.ToArray();
	}

	//
	// Change the fluid  - ONLY as part of an affector
	//
	public void AddVelocity (Pixel pix, Vector2 velocity) {
		pixels_velocityx [pix.x, pix.y] += velocity.x;
		pixels_velocityy [pix.x, pix.y] += velocity.y;
	}

	public void AddDensity (Pixel pix, float density) {
		pixels_density [pix.x, pix.y] += density;
	}

	public delegate void DoAffect();
	public event DoAffect affectors;
}
