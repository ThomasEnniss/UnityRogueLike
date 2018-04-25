using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge {

	public int destination;
	public float weight;

	public Edge(int node_id){
		destination = node_id;
	}

	public void SetWeight(float new_weight)
	{
		weight = new_weight;
	}
}
