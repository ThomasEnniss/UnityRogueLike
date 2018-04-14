using UnityEngine;
using System.Collections;

public class NodeEdge{

	public RoomNode destination_node;
	public int edge_weight;

	public NodeEdge(RoomNode destination, int weight){
		destination_node = destination;
		edge_weight = weight;
	}
}
