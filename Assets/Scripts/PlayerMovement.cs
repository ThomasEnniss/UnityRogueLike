using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	// Update is called once per frame
	void Update () {
	
		Vector3 move_direction = new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical"));
		gameObject.transform.position += move_direction;
	}
}
