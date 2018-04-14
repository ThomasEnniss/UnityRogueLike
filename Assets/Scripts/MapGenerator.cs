using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

	public GameObject parent_map;
	public GameObject wall_prefab;
	public GameObject floor_prefab;
	public int map_width_x;
	public int map_length_z;
	public int number_of_rooms;

	public int min_room_width;
	public int max_room_width;

	public int min_room_length;
	public int max_room_length;



	int[,] map_structure;

	// Update is called once per frame
	void Start () {
		map_structure = new int[map_width_x,map_length_z];

		GenerateMap ();
		DrawMap ();
	}

	void GenerateMap(){
		FillMap ();
		CreateRooms ();
	}

	void FillMap(){

		for(int lengthZ = 0;lengthZ<map_length_z;lengthZ++)
		{
			for(int widthX = 0;widthX<map_width_x;widthX++)
			{
				map_structure[widthX,lengthZ] = 1;
			}
		}
	}

	void CreateRooms(){

		RoomGraph room_graph = new RoomGraph (number_of_rooms);
		Queue<int> room_queue = new Queue<int> ();

		/*These change based on the current rooms position and the roomnode.*/
		int initial_x_min_constraint = 1;
		int initial_x_max_constraint = map_width_x-1-max_room_width;
		int initial_z_min_constraint = 1;
		int initial_z_max_constraint = map_length_z - 1 - max_room_length;			

		RoomNode new_node = new RoomNode (initial_x_min_constraint,initial_x_max_constraint,initial_z_min_constraint,initial_z_max_constraint);
		room_graph.AddRoomNode (new_node);
		room_queue.Enqueue (room_graph.GetLastRoomIndex ());
		print (room_queue);
		for (int i = 0; i < number_of_rooms; i++) {
			
			int current_room_index = room_queue.Peek ();

			RoomNode current_node = room_graph.G[current_room_index];

			int room_width = (int)Random.Range (min_room_width,max_room_width);
			int room_length = (int)Random.Range (min_room_length,max_room_length);

			int room_top_x = (int)Random.Range (current_node.x_min_constraint,current_node.x_max_constraint);
			int room_top_z = (int)Random.Range (current_node.z_min_constraint,current_node.z_max_constraint);

			Room new_room = new Room (room_top_x,room_top_z,room_width,room_length);

			if (!RoomCollision (new_room)) {
				
				current_node.AddRoom (new_room);
				PlotRoom (new_room);

				/*We Check North First. +2 Represents the walls rooms*/
				if(new_room.top_z < (min_room_length + 2)){
					current_node.MarkDirectionUnSuitable (CardinalDirections.NORTH);
					RoomNode north_node = new RoomNode (new_room.top_x,new_room.top_x + max_room_width,1,new_room.top_z );
					room_graph.AddRoomNode (north_node);
					room_queue.Enqueue (room_graph.GetLastRoomIndex());
				}

				/*We Check East*/
				if((new_room.top_x+new_room.width_x) > (map_width_x - min_room_width + 2)){
					current_node.MarkDirectionUnSuitable (CardinalDirections.EAST);
					RoomNode north_node = new RoomNode (new_room.top_x + new_room.width_x + 1,map_width_x - 1,new_room.top_z,new_room.top_z + new_room.length_z);
					room_graph.AddRoomNode (north_node);
					room_queue.Enqueue (room_graph.GetLastRoomIndex());
				}

				/*We Check South*/
				if((new_room.top_z+new_room.length_z) > (map_length_z - min_room_length + 2)){
					current_node.MarkDirectionUnSuitable (CardinalDirections.SOUTH);
					RoomNode north_node = new RoomNode  (new_room.top_x,new_room.top_x + max_room_width,new_room.top_z + new_room.length_z + 1,map_length_z);
					room_graph.AddRoomNode (north_node);
					room_queue.Enqueue (room_graph.GetLastRoomIndex());
				}

				/*We Check West Last*/
				if((new_room.top_x) < (min_room_width + 2)){
					current_node.MarkDirectionUnSuitable (CardinalDirections.EAST);
					RoomNode north_node = new RoomNode (1,new_room.top_x - 1,new_room.top_z,new_room.top_z + new_room.length_z);
					room_graph.AddRoomNode (north_node);
					room_queue.Enqueue (room_graph.GetLastRoomIndex());
				}
			} 
			else
			{
				i--;
			}
		}
	}



	/*Checks to see if one of the four room corners falls into already plotted territory*/
	bool RoomCollision(Room room){

		bool collision_detected = false;

		/*Top Left Corner*/
		if(map_structure[room.top_x,room.top_z]==0){
			collision_detected = true;
		}
		/*Top Right Corner*/
		if(map_structure[room.top_x + room.width_x,room.top_z]==0){
			collision_detected = true;
		}
		/*Bottom Right Corner*/
		if(map_structure[room.top_x + room.width_x,room.top_z + room.length_z]==0){
			collision_detected = true;
		}
		/*Bottom Left Corner*/
		if(map_structure[room.top_x,room.top_z + room.length_z]==0){
			collision_detected = true;
		}			
		return collision_detected;
	}

	/*Fills the room area on the map. Marks it as 0*/
	void PlotRoom(Room room){
		
		for(int lengthZ = room.top_z; lengthZ < room.top_z + room.length_z; lengthZ++)
		{
			for(int widthX = room.top_x; widthX < room.top_x + room.width_x; widthX++)
			{
				map_structure[widthX,lengthZ] = 0;
			}
		}
	}

	void DrawMap(){

		for(int lengthZ = 0;lengthZ<map_length_z;lengthZ++)
		{
			for(int widthX = 0;widthX<map_width_x;widthX++)
			{
				if(map_structure[widthX,lengthZ] ==1){
					GameObject wall_tile = (GameObject)Instantiate (wall_prefab, new Vector3 (widthX,0.75f,lengthZ), Quaternion.identity);
					wall_tile.transform.SetParent (parent_map.transform);
				}
				GameObject floor_tile = (GameObject)Instantiate (floor_prefab, new Vector3 (widthX,0.0f,lengthZ), Quaternion.identity);
				floor_tile.transform.SetParent (parent_map.transform);
			}
		}
	}
}
