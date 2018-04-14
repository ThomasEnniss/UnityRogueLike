using UnityEngine;
using System.Collections;

public class RoomNode{
	
	/*This is the number of pathways we have leading to and from a room. For the moment we will allow only 1 per side in a NESW pattern.*/
	public int number_of_edges;
	public int x_min_constraint;
	public int x_max_constraint;
	public int z_min_constraint;
	public int z_max_constraint;
	public Room room;
	bool[] suitable_direction  = {true,true,true,true};
	NodeEdge[] edges = new NodeEdge[4];

	public RoomNode(int min_x,int max_x,int min_z,int max_z){
		x_min_constraint = min_x;
		x_max_constraint = max_x;
		z_min_constraint = min_z;
		z_max_constraint = max_z;
	}

	public void AddRoom(Room new_room){
		room = new_room;
	}

	public void AddEdge(NodeEdge new_edge,CardinalDirections direction){
		edges[(int)direction] = new_edge;
	}

	public void MarkDirectionUnSuitable(CardinalDirections direction){
		suitable_direction [(int)direction] = false;
	}

	public bool CheckDirection(CardinalDirections direction){
		return suitable_direction [(int)direction];
	}
}
