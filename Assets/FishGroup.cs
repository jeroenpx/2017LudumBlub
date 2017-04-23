using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishGroup : MonoBehaviour {

	public List<FishMoveTo> fishes;
	public List<Transform> points;
	private List<Vector3> deltas;
	public float timeBetween;
	public Transform initialCenter;

	public float randomSize;

	// Use this for initialization
	void Start () {
		deltas = new List<Vector3> ();
		foreach (FishMoveTo fish in fishes) {
			deltas.Add (fish.transform.position - initialCenter.position);
		}

		StartCoroutine (Run ());
	}

	IEnumerator Run () {
		int goalIndex = 1;
		while(true) {
			Vector3 goal = points [goalIndex].position;
			for (int i = 0;i<fishes.Count;i++) {
				FishMoveTo fish = fishes [i];
				//Vector3 delta = deltas [i];
				Vector2 delta2 = Random.insideUnitCircle*randomSize;
				Vector3 delta = new Vector3 (delta2.x, delta2.y, 0);
				fish.goal = goal+delta;// + delta;
			}
			goalIndex++;
			if (goalIndex >= points.Count) {
				goalIndex = 0;
			}
			yield return new WaitForSeconds (timeBetween);
		}
	}
}
