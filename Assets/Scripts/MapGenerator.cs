using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

	public GameObject player;

	public GameObject wall_prefab;
	public GameObject floor_prefab;
	public GameObject chest;

	public int map_width_x;
	public int map_length_z;
	public int number_of_rooms;

	public int min_room_width;
	public int max_room_width;

	public int min_room_length;
	public int max_room_length;

	public int max_chest_count;

	int[,] map_generation_grid;

	List<Room> current_room_list;
	RoomGraph current_map_graph;

	int actual_room_count;

	void Start () {
		//player_in_map = false;
		map_generation_grid = new int[map_width_x,map_length_z];
		current_room_list = new List<Room> ();
		actual_room_count = 0;
		GenerateMap ();
		CreateRoomGraph ();
		current_map_graph.GenerateEdgeConnections ();
		DrawMap ();
		current_map_graph.DisplayGraph ();
	}

	void GenerateMap(){
		CreateBlankMapArray ();
		CreateRooms ();
	}

	void CreateBlankMapArray(){

		for(int lengthZ = 0;lengthZ<map_length_z;lengthZ++)
		{
			for(int widthX = 0;widthX<map_width_x;widthX++)
			{
				map_generation_grid[widthX,lengthZ] = 0;
			}
		}
	}

	void CreateRooms(){

		/*These change based on the current rooms position and the roomnode.*/
		int initial_x_min_constraint = 0;
		int initial_x_max_constraint = map_width_x - 1;
		int initial_z_min_constraint = 0;
		int initial_z_max_constraint = map_length_z - 1;

		int fail_count = 0;
		int MAX_FAIL_COUNT = 3;

		for (int i = 0; i < number_of_rooms; i++) {

			int room_width_x = (int)Random.Range (min_room_width,max_room_width);
			int room_length_z = (int)Random.Range (min_room_length,max_room_length);

			/*We need to make width odd so we can center things.*/
			/*Center width first*/
			if (room_width_x % 2 == 0) {

				if (room_width_x > min_room_width) {

					room_width_x--;

				} else {

					room_width_x++;

				}
			} 

			/*Center Length second*/
			if(room_length_z % 2 == 0){

				if(room_length_z > min_room_length){

					room_length_z--;

				}else{
				
					room_length_z++;

				}
			}				

			int top_x = (int)Random.Range (initial_x_min_constraint,initial_x_max_constraint - room_width_x);
			int top_z = (int)Random.Range (initial_z_min_constraint,initial_z_max_constraint - room_length_z);

			Room new_room = new Room (i,top_x,top_z,room_width_x,room_length_z);

			if (!RoomCollision (new_room)) {			

				new_room.CalculateCenter ();
				current_room_list.Add (new_room);
				fail_count = 0;
				actual_room_count++;
				PlotRoom (new_room);
				print ("Plotting room: " + i);

			}
			else
			{
				
				fail_count++;
				print ("Room Failed");

				if(fail_count<MAX_FAIL_COUNT){
					i--;
				}
			}
		}
	}

	bool RoomCollision(Room room){

		bool collision = false;

		for(int lengthZ = room.top_z; lengthZ < room.top_z + room.length_z; lengthZ++)
		{
			for(int widthX = room.top_x; widthX < room.top_x + room.width_x; widthX++)
			{
				if (map_generation_grid [widthX, lengthZ] != 0)
				{
					collision = true;
				}
			}
		}
		return collision;
	}

	/*Fills the room area on the map.*/
	void PlotRoom(Room room){

		for(int lengthZ = room.top_z; lengthZ < room.top_z + room.length_z; lengthZ++)
		{
			for(int widthX = room.top_x; widthX < room.top_x + room.width_x; widthX++)
			{
				if (lengthZ == room.top_z || lengthZ == room.top_z + room.length_z - 1 || widthX == room.top_x || widthX == room.top_x + room.width_x - 1)
				{					
					map_generation_grid [widthX, lengthZ] = 2;
				}
				else 
				{					
					map_generation_grid [widthX, lengthZ] = 1;		
				}
			}
		}
	}

	void CreateRoomGraph(){

		current_map_graph = new RoomGraph (actual_room_count);

		foreach(Room current_room in current_room_list){
			
			RoomNode new_node = new RoomNode (current_room);
			current_map_graph.AddRoomNode (new_node);

		}
	}

	void GenerateDoors(){





	}

	void DrawMap(){

		for(int lengthZ = 0;lengthZ<map_length_z;lengthZ++)
		{
			for(int widthX = 0;widthX<map_width_x;widthX++)
			{
				/*Instantiate a wall and floor tile.*/
				if (map_generation_grid [widthX, lengthZ]==2) {
					GameObject wall_tile = (GameObject)Instantiate (wall_prefab, new Vector3 (widthX,0.75f,lengthZ), Quaternion.identity);
					GameObject floor_tile = (GameObject)Instantiate (floor_prefab, new Vector3 (widthX,0.0f,lengthZ), Quaternion.identity);
				}

				if(map_generation_grid [widthX, lengthZ]==1){
					GameObject floor_tile = (GameObject)Instantiate (floor_prefab, new Vector3 (widthX,0.0f,lengthZ), Quaternion.identity);				
				}
			}
		}
		print (Time.time);
	}
}