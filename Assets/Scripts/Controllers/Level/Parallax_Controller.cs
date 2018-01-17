using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax_Controller : MonoBehaviour {

	public float parallaxSpeed = 1;
	public float distanceFromAnchor = 2;
	public Transform anchor;
	Vector2 desiredPosition;
	Vector2 heading;

	public void SetAnchor(Transform _anchor){
		anchor = _anchor;
	}
	private void Update() {
		if (anchor == null)
			return;
		InterpretAnchor();
		ParallaxMove();	
	}
	void InterpretAnchor(){
		float x = anchor.transform.position.x < 0 ?
				  anchor.transform.position.x - distanceFromAnchor :
				  anchor.transform.position.x + distanceFromAnchor;	 
		float y = anchor.transform.position.y < 0 ?
				  anchor.transform.position.y - distanceFromAnchor :
				  anchor.transform.position.y + distanceFromAnchor;	 
		desiredPosition = new Vector2(-anchor.transform.position.x, -anchor.transform.position.y);//new Vector2(-x, -y);
		heading = (desiredPosition - (Vector2)transform.position);
	}
	void ParallaxMove(){
		if (heading.sqrMagnitude < distanceFromAnchor * distanceFromAnchor)
			return;
		
		transform.position += (Vector3)heading * parallaxSpeed * Time.deltaTime;
		
	}

}
