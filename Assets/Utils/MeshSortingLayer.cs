using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSortingLayer : MonoBehaviour {

	public string sortingLayer;
	public int orderInLayer;

	// Use this for initialization
	[ContextMenu("Apply")]
	void Apply () {
		GetComponent<Renderer> ().sortingLayerName = sortingLayer;
		GetComponent<Renderer> ().sortingOrder = orderInLayer;
	}
}
