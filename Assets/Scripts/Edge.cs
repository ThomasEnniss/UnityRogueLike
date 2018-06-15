using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge{

	public int start_room;
	public int destination_room;
	public float weight;

	public Edge(int new_start, int new_destination, float new_weight){
		start_room = new_start;
		destination_room = new_destination;
		weight = new_weight;
	}
}