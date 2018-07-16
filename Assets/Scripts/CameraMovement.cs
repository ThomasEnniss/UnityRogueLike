using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

	GameObject player;
    public float height;

	// Use this for initialization
	void Start () {
        height = 10;
		player = GameObject.FindGameObjectWithTag ("Player");
	}

	// Update is called once per frame
	void Update () {

		if(player==null){
			player = GameObject.FindGameObjectWithTag ("Player");
		}

		gameObject.transform.position = new Vector3 (player.transform.position.x, height , player.transform.position.z);
	}
}
