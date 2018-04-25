using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomNode{
	
	/*This is the number of pathways we have leading to and from a room. For the moment we will allow only 1 per side in a NESW pattern.*/
	public int number_of_edges;
	public Room room;

	bool[] door_on_side = new bool[4];
	public List<Edge> exits;

	public RoomNode(Room new_room){
		room = new_room;
		exits = new List<Edge> ();
	}

	public void AddEdge(Edge new_edge){
		number_of_edges++;
		exits.Add (new_edge);
	}
}
