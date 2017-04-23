using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if (UNITY_EDITOR) 
using UnityEditor;
#endif

public class PlantBuilder : MonoBehaviour {

	public int count;
	public GameObject plantPiecePrefab;

#if (UNITY_EDITOR) 

	[ContextMenu("Make Plant")]
	void Make() {
		// Destroy all children
		for (int i = transform.childCount-1; i >= 0; i--) {
			GameObject.DestroyImmediate (transform.GetChild (i));
		}

		// Now, make all children again
		Vector3 lastPoint = transform.position;
		Rigidbody2D lastRigidBody = GetComponent<Rigidbody2D> ();
		for (int i = 0; i < count; i++) {
			GameObject piece = (GameObject) PrefabUtility.InstantiatePrefab (plantPiecePrefab);// GameObject.Instantiate(plantPiecePrefab);
			Transform origin = piece.transform.Find ("Origin");
			Transform endpoint = piece.transform.Find ("EndPoint");
			piece.transform.SetParent (transform);
			piece.transform.position = piece.transform.position - (origin.position - lastPoint);
			lastPoint = endpoint.position;

			// Configure Hinge Joint
			HingeJoint2D hinge = piece.GetComponent<HingeJoint2D>();
			hinge.anchor = piece.transform.InverseTransformPoint (origin.position);
			hinge.connectedBody = lastRigidBody;
			hinge.connectedAnchor = lastRigidBody.transform.InverseTransformPoint (origin.position);


			// Configure Relative ... Joint
			RelativeJoint2D reljoint = piece.GetComponent<RelativeJoint2D>();
			reljoint.connectedBody = lastRigidBody;

			// Next...
			lastRigidBody = piece.GetComponent<Rigidbody2D> ();
		}


	}

	#endif

}
