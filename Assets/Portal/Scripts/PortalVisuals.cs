using UnityEngine;
using System.Collections;

public class PortalVisuals : MonoBehaviour {
	
	private Transform target;
	
	[SerializeField]
	private float distancelimit = 6f;
	
	[SerializeField]
	private float effectSpeed=.6f;
	
	[SerializeField]
	private float percentForActivation=.8f;

	[SerializeField]
	private float percentDelay=0;
	
	private float currentPos = 0f;
	
	// Use this for initialization
	void Start () {
		target = ((OctoCollision)GameObject.FindObjectOfType(typeof(OctoCollision))).transform;
	}
	
	// Update is called once per frame
	void Update () {
		bool active = false;
		if(Vector3.Distance(target.transform.position, transform.position)<distancelimit){
			active = true;
		}
		
		currentPos=Mathf.Clamp01(currentPos+(active?1:-1)*Time.deltaTime*effectSpeed);
		
		//GetComponent<FadeInController>().enableParticles(currentPos>.95f);
		//GetComponent<FadeInController>().SetPercent(Mathf.Clamp01((currentPos-percentForActivation)/(1f-percentForActivation)));
		
		GetComponent<Renderer>().material.SetFloat("_TransPos", Mathf.Clamp01((currentPos-percentDelay)/percentForActivation));

		if (currentPos > 0.99f) {
			SendMessage("MessagePortalActive", SendMessageOptions.DontRequireReceiver);
		}
	}
}
