using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageController : MonoBehaviour {

	public List<Sprite> sprites;
	public List<Transform> diffs;
	public float messageShowTime = 3f;

	public static int left;

	// Use this for initialization
	void Start () {
		// Random sprite
		int spritei = Random.Range (0, sprites.Count);
		GetComponent<SpriteRenderer> ().sprite = sprites [spritei];

		// Random pos
		if (left == 0) {
			left = 1;
		} else {
			left = 0;
		}
		transform.position = diffs [left].position;
		transform.rotation = diffs [left].rotation;

		StartCoroutine (Run ());
	}
	
	// Update is called once per frame
	IEnumerator Run () {
		yield return new WaitForSeconds (messageShowTime);
		Destroy (gameObject);
	}
}
