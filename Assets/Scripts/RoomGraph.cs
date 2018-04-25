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

	public void GenerateEdgeConnections(){

		for(int i = 0;i<number_of_nodes;i++){

			RoomNode top_node = G [i];

			for(int j = 0;j<number_of_nodes;j++){

				RoomNode current_node = G [j];

				/*We create the weight and add an edge from the current i room to the next j room*/
				if (i != j) 
				{
					Edge new_edge = new Edge (j);
					new_edge.SetWeight (Vector3.Distance (top_node.room.center,current_node.room.center));
					top_node.AddEdge (new_edge);
				}
			}
		}
	}

	public void DisplayGraph()
	{
		for(int i = 0;i<number_of_nodes;i++)
		{			
			RoomNode current_node = G [i];
			Debug.Log("<color=red>RoomNode No: " + i + "</color>");
			for (int j = 0; j < current_node.number_of_edges; j++) 
			{
				Debug.Log("<color=blue>\t\t( Edge: " + j + " Destination: " + current_node.exits [j].destination + " Weight: " + current_node.exits [j].weight + " )</color>");
			}
		}
	}
}