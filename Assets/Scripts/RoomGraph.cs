using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomGraph{

	public List<Room> room_graph;
	public int number_of_rooms;
	public List<Edge> edges;

	public RoomGraph(int room_count){

		number_of_rooms = room_count;
		room_graph = new List<Room> ();
		edges = new List<Edge> ();

	}

	public void AddRoom(Room new_room){
		room_graph.Add (new_room);
	}

	public void GenerateEdgeConnections(){

		for (int i = 0; i < number_of_rooms; i++) {
			
			Room current = room_graph [i];

			/*TE: j+1. No point in repeating edges. They are bi directional*/
			for (int j = i+1; j < number_of_rooms; j++) {

				Room other = room_graph [j];

				float euclidian_distance = Vector3.Distance (current.center,other.center);

				Edge new_edge = new Edge (i,j,euclidian_distance); 

				edges.Add (new_edge);
			}
		}
		SortEdges();
	}

	/*TE: Sorts edges so that the shortest edges are first.
	  BUBBLESORT Since it is simple for now.
    */
	void SortEdges(){
		int number_of_edges = edges.Count;
		for(int i = number_of_edges-1;i>0;i-- ){
			for(int j = 1;j<=i;j++){
				if (edges[j].weight < edges[j-1].weight) {
					Edge temp = edges [j - 1];
					edges [j - 1] = edges [j];
					edges [j] = temp;
				}
			}
		}
	}

	public void EMST(){

		List<Edge> EMST = new List<Edge>();
		int number_of_edges = edges.Count;
		int[] root = new int[number_of_rooms];

		for (int r = 0; r < number_of_rooms; r++) {			
			root[r] = r;
		}

		for (int i = 0; i < number_of_edges; i++) {
			
			Edge current = edges [i];
			int start_room_id = current.start_room;
			int destination_room_id = current.destination_room;

			if(root[start_room_id] != root[destination_room_id]){				

				int old_destination_parent = root [destination_room_id];
				int new_parent_id = root [start_room_id];

				for (int r2 = 0; r2 < number_of_rooms; r2++) {
					if (root [r2] == old_destination_parent) {
						root [r2] = new_parent_id;
					}
				}
				EMST.Add (current);
			}
		}
		edges = EMST;
	}

	public void DisplayGraph(){

		foreach(Edge current in edges){

			Debug.Log ("Room ID:"+current.start_room+"Connects to Room ID:"+current.destination_room+". Distance:  "+current.weight);

		}
	}
}