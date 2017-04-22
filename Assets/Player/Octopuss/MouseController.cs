using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MouseController : MonoBehaviour {

    private Rigidbody2D _rigid;

	// Use this for initialization
	void Start () {
        _rigid = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0))
        {
            Vector3 targetPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            _rigid.MovePosition(targetPos);
        }
    }
}
