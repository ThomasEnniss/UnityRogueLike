using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathnode {

	public Vector3 location;
	public float distance_to_target;

	public Pathnode(Vector3 new_location_vector3, float new_distance_to_target){
		location = new_location_vector3;
		distance_to_target = new_distance_to_target;
	}
}