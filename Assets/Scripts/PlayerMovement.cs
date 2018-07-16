using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public float speed = 10f;
	Vector3 velocity;

	Rigidbody rigidbody;

	void Start(){
		rigidbody = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 direction = new Vector3 (Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical")).normalized;

		velocity = direction * speed * Time.deltaTime;
	}

	void FixedUpdate(){		
		rigidbody.position += velocity;
	}
}
