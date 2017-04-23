using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishGroup : MonoBehaviour {

	public List<FishMoveTo> fishes;
	public List<Transform> points;
	private List<Vector3> prev;
	private List<Vector3> next;
	public float timeBetween;
	public Transform initialCenter;

	public float randomSize;

	public int iter;

	// Use this for initialization
	void Start () {
		prev = new List<Vector3> ();
		next = new List<Vector3> ();
		foreach (FishMoveTo fish in fishes) {
			next.Add (fish.transform.position);
			prev.Add (fish.transform.position);
		}

		StartCoroutine (Run ());
	}

	IEnumerator Run () {
		if (iter < 1) {
			throw new System.InvalidOperationException ("Cannot have 0 iters... => will cause infinite loop");
		}

		int goalIndex = 1;
		while(true) {
			for (int i = 0; i < fishes.Count; i++) {
				prev[i] = next[i];
			}

			Vector3 goal = points [goalIndex].position;
			for (int i = 0;i<fishes.Count;i++) {
				FishMoveTo fish = fishes [i];
				//Vector3 delta = deltas [i];
				Vector2 delta2 = Random.insideUnitCircle*randomSize;
				Vector3 delta = new Vector3 (delta2.x, delta2.y, 0);
				next[i] = goal+delta;// + delta;
			}
			goalIndex++;
			if (goalIndex >= points.Count) {
				goalIndex = 0;
			}

			for (int step = 0; step < iter; step++) {
				float percent = step *1.0f / iter;

				for (int i = 0;i<fishes.Count;i++) {
					FishMoveTo fish = fishes [i];
					fish.goal = percent*next[i] + (1f-percent)*prev[i];
				}

				yield return new WaitForSeconds (timeBetween/iter);
			}

		}
	}
}
