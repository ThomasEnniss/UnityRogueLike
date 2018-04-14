using UnityEngine;
using System.Collections;

public class RoomGraph{

	int number_of_nodes;
	public RoomNode[] G;
	public int next_insert_index;


	public RoomGraph(int number_of_rooms){
		number_of_nodes = number_of_rooms;
		G = new RoomNode[number_of_nodes];
		next_insert_index = 0;
	}

	public void AddRoomNode(RoomNode new_room_node){
		G [next_insert_index] = new_room_node;
		next_insert_index++;
	}

	public int GetLastRoomIndex(){
		return next_insert_index - 1;
	}
}
